using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class CompBoiler : ThingComp
    {
        public bool LowPowerMode = true;
        public bool overrideThermostat = true;
        public int powerMode = 1;
        public CompBreakdownable breakdownableComp;
        public CompRefuelable fuelComp;
        public CompPowerTrader powerComp;
        public CompSteamPipe pipeComp;
        public int ticksToRun;

        public CompProperties_CompBoiler Props
        {
            get
            {
                return (CompProperties_CompBoiler)this.props;
            }
        }

        public virtual bool WorkingNow
        {
            get
            {
                if (!FlickUtility.WantsToBeOn((Thing)this.parent) || this.powerComp != null && !this.powerComp.PowerOn || this.fuelComp != null && !this.fuelComp.HasFuel)
                    return false;
                return this.breakdownableComp == null || !this.breakdownableComp.BrokenDown;
            }
        }

        public virtual float Capacity
        {
            get
            {
                return this.WorkingNow && !this.LowPowerMode ? this.Props.BaseCapacity * (float)this.powerMode : 0.0f;
            }
        }

        public float PowerUsage
        {
            get
            {
                return this.powerComp.Props.PowerConsumption * (float)this.powerMode;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.overrideThermostat, "overrideSteamThermostat", true, false);
            Scribe_Values.Look<int>(ref this.powerMode, "powerMode", 0, false);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.pipeComp = this.parent.GetComp<CompSteamPipe>();
            this.fuelComp = this.parent.GetComp<CompRefuelable>();
            this.powerComp = this.parent.GetComp<CompPowerTrader>();
            this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
        }

        public override string CompInspectStringExtra()
        {
            if (this.ParentHolder is MinifiedThing)
                return base.CompInspectStringExtra();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine((string)"RH_TET_Magic_NetworkHeatingCapacity".Translate((NamedArgument)this.pipeComp.pipeNet.HeatStoreCapacitySum.ToString("0"), (NamedArgument)this.pipeComp.pipeNet.BoilerCapacitySum.ToString("0"), (NamedArgument)this.pipeComp.pipeNet.HeatingCap.ToStringPercent("0.00")));
            if (this.powerComp != null && this.Props.ShouldUsePowerModes)
                stringBuilder.AppendLine((string)"RH_TET_Magic_HeatingCapacity".Translate((NamedArgument)(this.Props.BaseCapacity * (float)this.powerMode), (NamedArgument)this.PowerUsage));
            if (this.ticksToRun > 0)
                stringBuilder.AppendLine("Run time: " + this.ticksToRun.ToStringTicksToPeriod(true, false, true, true));
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (this.Props.effects == null || this.LowPowerMode || (!(this.parent.Rotation == Rot4.North) || !FlickUtility.WantsToBeOn((Thing)this.parent)))
                return;
            if (this.fuelComp != null && this.fuelComp.HasFuel)
                this.Props.effects.Graphic.Draw(this.parent.DrawPos, new Rot4(), (Thing)this.parent, 0.0f);
            if (this.powerComp == null || !this.powerComp.PowerOn)
                return;
            this.Props.effects.Graphic.Draw(this.parent.DrawPos, new Rot4(), (Thing)this.parent, 0.0f);
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            this.pipeComp.pipeNet.MapComp.MarkHeatingForDraw = true;
        }

        public void ThermostatOverride()
        {
            this.overrideThermostat = !this.overrideThermostat;
        }

        public void RaisePow()
        {
            if (this.powerMode >= this.Props.PowerModes)
                return;
            ++this.powerMode;
        }

        public void ReducePow()
        {
            if (this.powerMode <= 1)
                return;
            --this.powerMode;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            CompBoiler compBoiler = this;
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;
            if (compBoiler.powerComp != null && compBoiler.Props.ShouldUsePowerModes)
            {
                yield return (Gizmo)new Gizmo_BoilerStatus()
                {
                    boiler = compBoiler
                };
                Command_Action commandAction1 = new Command_Action();
                commandAction1.action = new Action(compBoiler.ReducePow);
                commandAction1.defaultLabel = (string)"RH_TET_Magic_LowerPower".Translate();
                commandAction1.defaultDesc = (string)"RH_TET_Magic_LowerPowerDesc".Translate();
                commandAction1.icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true);
                yield return (Gizmo)commandAction1;
                Command_Action commandAction2 = new Command_Action();
                commandAction2.action = new Action(compBoiler.RaisePow);
                commandAction2.defaultLabel = (string)"RH_TET_Magic_RaisePower".Translate();
                commandAction2.defaultDesc = (string)"RH_TET_Magic_RaisePowerDesc".Translate();
                commandAction2.icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true);
                yield return (Gizmo)commandAction2;
            }
        }

        public bool overrided()
        {
            return this.overrideThermostat;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (this.fuelComp != null && !this.LowPowerMode && this.WorkingNow)
                this.fuelComp.Notify_UsedThisTick();
            if (this.powerComp != null && this.Props.ShouldUsePowerModes)
                this.powerComp.PowerOutput = !this.LowPowerMode ? -this.PowerUsage : this.Props.LowPowerMode;
            if (this.overrided())
                this.LowPowerMode = false;
            if (this.ticksToRun > 0)
                --this.ticksToRun;
            if (this.ticksToRun <= 0 && this.parent.IsHashIntervalTick(250) && !this.overrided())
            {
                this.LowPowerMode = true;
                if (this.pipeComp != null)
                {
                    if (this.pipeComp.pipeNet.ThermostatPresent())
                        this.LowPowerMode = false;
                    else if (this.pipeComp.pipeNet.SteamPresent())
                    {
                        this.LowPowerMode = false;
                        this.ticksToRun = 2500;
                    }
                }
            }
            if (this.Props.ThermostatControl)
                return;
            this.LowPowerMode = false;
        }
    }
}