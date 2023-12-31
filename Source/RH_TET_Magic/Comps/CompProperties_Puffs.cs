using Verse;

namespace TheEndTimes_Magic
{
    public class CompProperties_Puffs : CompProperties
    {
        public int puffTicks = 100;

        public CompProperties_Puffs()
        {
            this.compClass = typeof(CompPuffs);
        }
    }
}