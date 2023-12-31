using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class CompProperties_CompBoiler : CompProperties
    {
        public float BaseCapacity = 300f;
        public string GizmoLabel = "Power Mode";
        public float LowPowerMode = -20f;
        public int PowerModes = 8;
        public bool ShouldUsePowerModes = true;
        public GraphicData effects;
        public bool ThermostatControl;

        public CompProperties_CompBoiler()
        {
            this.compClass = typeof(CompBoiler);
        }
    }
}
