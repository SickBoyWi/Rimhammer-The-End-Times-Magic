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

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (!p.IsMagicUser())
            {
                failReason = (string)null;
                return true;
            }
            else
            {
                failReason = "RH_TET_FailAlreadyAMagicUser".Translate();
                return false;
            }
        }
    }
}
