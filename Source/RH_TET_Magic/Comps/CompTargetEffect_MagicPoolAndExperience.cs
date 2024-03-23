using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompTargetEffect_MagicPoolAndExperience : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);

            // Good
            CompMagicUser compMagicUser = ((user != null) ? user.TryGetComp<CompMagicUser>() : null);
            if (compMagicUser != null && compMagicUser.IsMagicUser)
            {
                compMagicUser.LevelUp(true);
                compMagicUser.LevelUp(true);
                compMagicUser.LevelUp(true);

                user.needs.TryGetNeed<Need_MagicPool>()?.SetInitialLevel();

                Messages.Message(user.Name + "RH_TET_MagicLevelNeedBoost".Translate() + RH_TET_MagicDefOf.RH_TET_AddOn_Transport_Mage.LabelCap, (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
            }
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (!p.IsMagicUser())
            {
                failReason = "RH_TET_FailNotAMagicUser".Translate();
                return false;
            }

            failReason = (string)null;
            return true;
        }
    }
}
