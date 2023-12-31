using Verse;

namespace TheEndTimes_Magic
{
    public class CompProperties_MagicShield : CompProperties
    {
        public int startingTicksToReset = 3200;
        public float minDrawSize = 1.2f;
        public float maxDrawSize = 1.55f;
        public float energyLossPerDamage = 0.033f;
        public float energyOnReset = 0.2f;
        public bool blocksRangedWeapons = true;

        public CompProperties_MagicShield()
        {
            this.compClass = typeof(CompMagicShield);
        }
    }
}
