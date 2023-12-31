namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class CompProperties_Radiator : CompProperties_HeatStore
    {
        public float PowerRate = 21f;
        public int HeatRate = 250;

        public CompProperties_Radiator()
        {
            this.compClass = typeof(CompRadiator);
        }
    }
}