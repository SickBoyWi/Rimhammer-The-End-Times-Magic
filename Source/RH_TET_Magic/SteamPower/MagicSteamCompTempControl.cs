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
    public class MagicSteamCompTempControl : ThingComp
    {
        public float targetTemperature = -99999f;
        private const float DefaultTargetTemperature = 21f;

        public CompProperties_TempControl Props
        {
            get
            {
                return (CompProperties_TempControl)this.props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if ((double)this.targetTemperature >= -2000.0)
                return;
            this.targetTemperature = this.Props.defaultTargetTemperature;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<float>(ref this.targetTemperature, "targetTemperature", 21f, false);
            if (Scribe.mode != LoadSaveMode.LoadingVars || (double)this.targetTemperature >= 10.0)
                return;
            this.targetTemperature = 21f;
        }

        private float RoundedToCurrentTempModeOffset(float celsiusTemp)
        {
            return GenTemperature.ConvertTemperatureOffset((float)Mathf.RoundToInt(GenTemperature.CelsiusToOffset(celsiusTemp, Prefs.TemperatureMode)), Prefs.TemperatureMode, TemperatureDisplayMode.Celsius);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            MagicSteamCompTempControl steamCompTempControl = this;
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;
            float offset = steamCompTempControl.RoundedToCurrentTempModeOffset(-10f);
            Command_Action commandAction1 = new Command_Action();
            commandAction1.action = (Action)(() => this.InterfaceChangeTargetTemperature(offset));
            commandAction1.defaultLabel = offset.ToStringTemperatureOffset("F0");
            commandAction1.defaultDesc = (string)"CommandLowerTempDesc".Translate();
            commandAction1.hotKey = KeyBindingDefOf.Misc5;
            commandAction1.icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true);
            yield return (Gizmo)commandAction1;
            float offset4 = steamCompTempControl.RoundedToCurrentTempModeOffset(-1f);
            Command_Action commandAction2 = new Command_Action();
            commandAction2.action = (Action)(() => this.InterfaceChangeTargetTemperature(offset4));
            commandAction2.defaultLabel = offset4.ToStringTemperatureOffset("F0");
            commandAction2.defaultDesc = (string)"CommandLowerTempDesc".Translate();
            commandAction2.hotKey = KeyBindingDefOf.Misc4;
            commandAction2.icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true);
            yield return (Gizmo)commandAction2;
            Command_Action commandAction3 = new Command_Action();
            commandAction3.action = new Action(steamCompTempControl.resettemp);
            commandAction3.defaultLabel = (string)"CommandResetTemp".Translate();
            commandAction3.defaultDesc = (string)"CommandResetTempDesc".Translate();
            commandAction3.hotKey = KeyBindingDefOf.Misc1;
            commandAction3.icon = ContentFinder<Texture2D>.Get("UI/Commands/TempReset", true);
            yield return (Gizmo)commandAction3;
            float offset3 = steamCompTempControl.RoundedToCurrentTempModeOffset(1f);
            Command_Action commandAction4 = new Command_Action();
            commandAction4.action = (Action)(() => this.InterfaceChangeTargetTemperature(offset3));
            commandAction4.defaultLabel = "+" + offset3.ToStringTemperatureOffset("F0");
            commandAction4.defaultDesc = (string)"CommandRaiseTempDesc".Translate();
            commandAction4.hotKey = KeyBindingDefOf.Misc2;
            commandAction4.icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true);
            yield return (Gizmo)commandAction4;
            float offset2 = steamCompTempControl.RoundedToCurrentTempModeOffset(10f);
            Command_Action commandAction5 = new Command_Action();
            commandAction5.action = (Action)(() => this.InterfaceChangeTargetTemperature(offset2));
            commandAction5.defaultLabel = "+" + offset2.ToStringTemperatureOffset("F0");
            commandAction5.defaultDesc = (string)"CommandRaiseTempDesc".Translate();
            commandAction5.hotKey = KeyBindingDefOf.Misc3;
            commandAction5.icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true);
            yield return (Gizmo)commandAction5;
        }

        public void resettemp()
        {
            this.targetTemperature = this.Props.defaultTargetTemperature;
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera((Map)null);
            this.ThrowCurrentTemperatureText();
        }

        public void InterfaceChangeTargetTemperature(float offset)
        {
            this.targetTemperature += offset;
            this.targetTemperature = Mathf.Clamp(this.targetTemperature, this.Props.minTargetTemperature, this.Props.maxTargetTemperature);
            this.ThrowCurrentTemperatureText();
        }

        private void ThrowCurrentTemperatureText()
        {
            MoteMaker.ThrowText(this.parent.TrueCenter() + new Vector3(0.5f, 0.0f, 0.5f), this.parent.Map, this.targetTemperature.ToStringTemperature("F0"), Color.white, -1f);
        }

        public override string CompInspectStringExtra()
        {
            if (this.ParentHolder is MinifiedThing)
                return base.CompInspectStringExtra();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append((string)("TargetTemperature".Translate() + ": " + this.targetTemperature.ToStringTemperature("F0")));
            return stringBuilder.ToString();
        }
    }
}
