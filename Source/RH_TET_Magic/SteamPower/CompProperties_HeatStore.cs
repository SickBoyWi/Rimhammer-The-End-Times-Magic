using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class CompProperties_HeatStore : CompProperties
    {
        public float FallRate = 0.00016f;
        public float RiseRate = 1f / 625f;
        public float SteamConsumptionAmt = 100f;
    }
}