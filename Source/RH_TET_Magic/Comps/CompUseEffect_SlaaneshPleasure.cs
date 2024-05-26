using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_SlaaneshPleasure : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            
            user.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(RH_TET_MagicDefOf.RH_TET_SlaaneshPleasure), (Pawn)null);
            
            Messages.Message("RH_TET_GainedPleasureOfSlaanesh".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            return base.CanBeUsedBy(p);
        }
    }
}
