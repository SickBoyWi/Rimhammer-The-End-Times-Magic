using Verse;

namespace TheEndTimes_Magic
{
    public class CompProperties_FastHealingPawn : CompProperties
    {
        public int healIntervalTicks = 60;

        public CompProperties_FastHealingPawn()
        {
            this.compClass = typeof(CompFastHealingPawn);
        }
    }
}