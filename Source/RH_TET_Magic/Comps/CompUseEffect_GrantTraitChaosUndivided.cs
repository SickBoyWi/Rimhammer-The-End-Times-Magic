using System;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_GrantTraitChaosUndivided : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            Trait trait = new Trait(RH_TET_MagicDefOf.RH_TET_LoreOfChaosUndividedTrait, 1);
            user.story.traits.GainTrait(trait);

            Messages.Message("RH_TET_LearnedMagicLoreChaosUndivided".Translate(user.Name), (LookTargets)user, MessageTypeDefOf.PositiveEvent, true);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            string failReason;
            if (p.IsMagicUser())
            {
                failReason = "RH_TET_FailAlreadyAMagicUser".Translate();
            }
            else if (p.IsFaithUser())
            {
                failReason = "RH_TET_FailAlreadyAFaithUser".Translate();
            }
            else if (p.IsAbilityUser())
            {
                failReason = "RH_TET_FailAlreadyAnAbilityUser".Translate();
            }
            else
            {
                failReason = (string)null;
            }

            if (failReason is null)
                return base.CanBeUsedBy(p);
            else
                return (AcceptanceReport)failReason;
        }
    }
}
