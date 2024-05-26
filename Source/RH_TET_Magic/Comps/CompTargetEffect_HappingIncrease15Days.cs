using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompTargetEffect_HappingIncrease15Days : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            
            user.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(RH_TET_MagicDefOf.RH_TET_Plus15For15Days), (Pawn)null);
            
            Messages.Message("RH_TET_GainedMoodBoost".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            return base.CanBeUsedBy(p);
        }
    }
}
