using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class CompProperties_Thermostat : CompProperties
    {
        public float defaultTargetTemperature = 20f;
        public float maxTargetTemperature = 50f;
        public float minTargetTemperature = -50f;

        public CompProperties_Thermostat()
        {
            this.compClass = typeof(CompThermostat);
        }
    }
}