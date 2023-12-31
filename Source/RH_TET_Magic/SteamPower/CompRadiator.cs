using RimWorld;
using System;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class CompRadiator : CompHeatStore
    {
        public float targetTemperature = 21.8f;
        public MagicSteamCompTempControl compTempControl;
        public float heatUsed;

        public CompProperties_Radiator Props
        {
            get
            {
                return (CompProperties_Radiator)this.props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.compTempControl = this.parent.GetComp<MagicSteamCompTempControl>();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (!flickComp.SwitchIsOn)
            {
                CompRadiator.RenderPulsingOverlay((Thing)this.parent, GraphicsCache.PowerOff, this.parent.DrawPos, MeshPool.plane08, Quaternion.identity);
                return;
            }
            if (this.heaterConnected || !this.parent.Spawned)
                return;
            CompRadiator.RenderPulsingOverlay((Thing)this.parent, GraphicsCache.MissingPipes, this.parent.DrawPos, MeshPool.plane08, Quaternion.identity);
        }

        public static void RenderPulsingOverlay(
             Thing thing,
             Material mat,
             Vector3 drawPos,
             Mesh mesh,
             Quaternion rot)
        {
            float alpha = (float)(0.300000011920929 + (Math.Sin(((double)Time.realtimeSinceStartup + 397.0 * ((thing != null ? (double)thing.thingIDNumber : (double)drawPos.ToIntVec3().UniqueHashCode()) % 571.0)) * 4.0) + 1.0) * 0.5 * 0.699999988079071);
            Material material = FadedMaterialPool.FadedVersionOf(mat, alpha);
            Graphics.DrawMesh(mesh, drawPos, rot, material, 0);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (this.pipeComp == null || this.pipeComp.pipeNet == null || (flickComp.SwitchIsOn && HeaterUsage == 0F))
            { 
                heaterConnected = false;
            }
            else
            {
                heaterConnected = true;
            }

            if (!this.parent.Spawned || !this.parent.IsHashIntervalTick(this.Props.HeatRate))
                return;
            float ambientTemperature = this.parent.AmbientTemperature;
            float num1 = (float)((double)this.Props.PowerRate * ((double)ambientTemperature >= 19.0 ? ((double)ambientTemperature <= 120.0 ? (double)Mathf.InverseLerp(120f, 20f, ambientTemperature) : 0.0) : 1.0) * (double)this.HeaterUsage * 4.16666650772095);
            IntVec3 position = this.parent.Position;
            Map map = this.parent.Map;
            double num2 = (double)num1;
            MagicSteamCompTempControl compTempControl = this.compTempControl;
            double num3 = compTempControl != null ? (double)compTempControl.targetTemperature : (double)this.targetTemperature;
            this.heatUsed = GenTemperature.ControlTemperatureTempChange(position, map, (float)num2, (float)num3);
            if (Mathf.Approximately(this.heatUsed, 0.0f))
                return;
            this.parent.GetRoom(RegionType.Set_All).Temperature += this.heatUsed;
        }

        public override string CompInspectStringExtra()
        {
            if (this.ParentHolder is MinifiedThing)
                return base.CompInspectStringExtra();
            StringBuilder stringBuilder = new StringBuilder();
            if (!this.parent.Spawned)
                return (string)null;
            if (DebugSettings.godMode)
                stringBuilder.AppendLine("Heat amount per cell:" + this.heatUsed.ToString());
            stringBuilder.AppendLine((string)"RH_TET_Magic_NetworkHeatingCapacity".Translate((NamedArgument)this.pipeComp.pipeNet.HeatStoreCapacitySum.ToString("0"), (NamedArgument)this.pipeComp.pipeNet.BoilerCapacitySum.ToString("0"), (NamedArgument)this.pipeComp.pipeNet.HeatingCap.ToStringPercent("0.00")));
            stringBuilder.AppendLine((string)"RH_TET_Magic_HeatingUnits".Translate((NamedArgument)this.Props.SteamConsumptionAmt));
            stringBuilder.Append((string)"RH_TET_Magic_RadiatorUsage".Translate((NamedArgument)this.HeaterUsage.ToStringPercent("0.0")));
            return stringBuilder.ToString().TrimEndNewlines();
        }

    }
}
