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
    [StaticConstructorOnStartup]
    public class CompHeatStore : ThingComp, HeatStore
    {
        public bool heaterConnected = true;
        private bool intThermostatControlled = true;
        public CompFlickable flickComp;
        private float heaterTempInt;
        public CompSteamPipe pipeComp;
        public bool requestHeating;
        public float velo;

        public bool DrawOverlay { get; set; }

        public float HeaterUsage
        {
            get
            {
                return this.heaterTempInt;
            }
            set
            {
                this.heaterTempInt = value;
            }
        }

        public bool ThermostatControlled
        {
            get
            {
                return this.intThermostatControlled;
            }
        }

        public CompProperties_HeatStore Props
        {
            get
            {
                return (CompProperties_HeatStore)this.props;
            }
        }

        public float GetStoreCapacity
        {
            get
            {
                return this.flickComp.SwitchIsOn ? this.Props.SteamConsumptionAmt : 0.0f;
            }
        }

        public bool working
        {
            get
            {
                return (this.flickComp == null || this.flickComp.SwitchIsOn) && !this.parent.IsBrokenDown();
            }
        }

        public bool LowCap
        {
            get
            {
                return (double)this.HeaterUsage < 0.25;
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
            if (!this.DrawOverlay)
                return;
            this.DrawOverlay = false;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.heaterTempInt, "SteamWaterTemp", 0.0f, false);
            Scribe_Values.Look<bool>(ref this.intThermostatControlled, "SteamThermostatControlled", true, false);
        }

        public override string CompInspectStringExtra()
        {
            if (this.ParentHolder is MinifiedThing)
                return base.CompInspectStringExtra();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine((string)"RH_TET_Magic_NetworkHeatingCapacity".Translate((NamedArgument)this.pipeComp.pipeNet.HeatStoreCapacitySum.ToString("0"), (NamedArgument)this.pipeComp.pipeNet.BoilerCapacitySum.ToString("0"), (NamedArgument)this.pipeComp.pipeNet.HeatingCap.ToStringPercent("0.00")));
            stringBuilder.AppendLine((string)"RH_TET_Magic_HeatingCapacity".Translate((NamedArgument)this.Props.SteamConsumptionAmt));
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override void CompTick()
        {
            base.CompTick();
            float maxDelta = this.Props.FallRate;
            if (this.pipeComp?.pipeNet == null)
                return;
            if ((double)this.HeaterUsage < (double)this.pipeComp.pipeNet.HeatingCap && this.flickComp.SwitchIsOn)
                maxDelta = this.Props.RiseRate;
            float target = 0.0f;
            if (this.flickComp.SwitchIsOn)
                target = this.pipeComp.pipeNet.HeatingCap;
            this.HeaterUsage = Mathf.MoveTowards(this.HeaterUsage, target, maxDelta);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;
        }
    }
}
