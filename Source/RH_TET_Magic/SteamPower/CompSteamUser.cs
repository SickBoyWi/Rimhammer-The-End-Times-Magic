using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    [StaticConstructorOnStartup]
    public class CompSteamUser : ThingComp, HeatStore
    {
        public bool heaterConnected = true;
        public CompFlickable flickComp;
        public CompSteamPipe pipeComp;
        private Sustainer sustainerWhileOperational;

        public bool DrawOverlay { get; set; }

        public CompProperties_SteamUser Props
        {
            get
            {
                return (CompProperties_SteamUser)this.props;
            }
        }

        public float GetStoreCapacity
        {
            get
            {
                return this.flickComp.SwitchIsOn ? this.Props.SteamConsumptionAmt : 0.0f;
            }
        }

        public bool LowCap
        {
            get
            {
                return false;
            }
        }

        public bool ThermostatControlled
        {
            get
            {
                return false;
            }
        }

        public float HeaterUsage
        {
            get
            {
                return this.Props.SteamConsumptionAmt;
            }
            set
            {
                // do nothing.
                Log.Warning("Trying to set temperature on Steam User. It is unused, and this shouldn't happen.");
            }
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            if (this.pipeComp == null)
                return;
            this.pipeComp.pipeNet.MapComp.MarkHeatingForDraw = true;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.pipeComp = this.parent.GetComp<CompSteamPipe>();
            this.flickComp = this.parent.GetComp<CompFlickable>();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (!flickComp.SwitchIsOn)
            {
                CompRadiator.RenderPulsingOverlay((Thing)this.parent, GraphicsCache.PowerOff, this.parent.DrawPos, MeshPool.plane08, Quaternion.identity);
                return;
            }

            if ((this.heaterConnected && (double)this.pipeComp.pipeNet.HeatingCap > .98F) || !this.parent.Spawned)
                return;

            CompRadiator.RenderPulsingOverlay((Thing)this.parent, GraphicsCache.MissingPipes, this.parent.DrawPos, MeshPool.plane08, Quaternion.identity);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override string CompInspectStringExtra()
        {
            if (this.ParentHolder is MinifiedThing)
                return base.CompInspectStringExtra();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine((string)"RH_TET_Magic_NetworkHeatingCapacity".Translate((NamedArgument)this.pipeComp.pipeNet.HeatStoreCapacitySum.ToString("0"), (NamedArgument)this.pipeComp.pipeNet.BoilerCapacitySum.ToString("0"), (NamedArgument)this.pipeComp.pipeNet.HeatingCap.ToStringPercent("0.00")));
            stringBuilder.AppendLine((string)"RH_TET_Magic_HeatingUnits".Translate((NamedArgument)this.Props.SteamConsumptionAmt));
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public bool isOperational()
        {
            if (this.heaterConnected && ((double)this.pipeComp.pipeNet.HeatingCap > .95F) && (this.flickComp == null || this.flickComp.SwitchIsOn) && !this.parent.IsBrokenDown())
                return true;
            return false;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (this.pipeComp == null || this.pipeComp.pipeNet == null || flickComp.SwitchIsOn)
            {
                heaterConnected = false;
            }
            else
            {
                heaterConnected = true;
            }

            if (this.isOperational())
            {
                if (this.sustainerWhileOperational == null || this.sustainerWhileOperational.Ended)
                    this.sustainerWhileOperational = this.Props.soundAmbientOn.TrySpawnSustainer(SoundInfo.InMap((TargetInfo)(Thing)this.parent, MaintenanceType.None));
                this.sustainerWhileOperational.Maintain();
            }
            else
            {
                if (this.sustainerWhileOperational == null)
                    return;
                this.sustainerWhileOperational.End();
                this.sustainerWhileOperational = (Sustainer)null;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;
        }
    }
}
