using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompThoughtGiver : ThingComp
    {
        private int nextTickCheck = -1;

        private CompProperties_ThoughtGiver Props
        {
            get
            {
                return (CompProperties_ThoughtGiver)this.props;
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            int currTicks = Find.TickManager.TicksGame;
            if (nextTickCheck > currTicks)
                return;
            else
                nextTickCheck = currTicks + 500;

            Apparel a = this.parent as Apparel;
            if (a != null && a.Wearer != null)
            {
                a.Wearer.needs?.mood?.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(this.Props.thoughtDef), (Pawn)null);
            }
        }
    }
}