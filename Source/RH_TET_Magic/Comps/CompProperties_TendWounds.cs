using Verse;

namespace TheEndTimes_Magic
{
    public class CompProperties_TendWounds : CompProperties
    {
        public int healIntervalTicks = 60;

        public CompProperties_TendWounds()
        {
            this.compClass = typeof(CompTendWounds);
        }
    }
}