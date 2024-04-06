using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_ScrollRaiseGround : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);

            // Good
            CompMagicUser compMagicUser = ((user != null) ? user.TryGetComp<CompMagicUser>() : null);
            if (compMagicUser != null && compMagicUser.IsMagicUser)
            {
                List<MagicPower> addonPowers = compMagicUser.MagicData.PowersAddons;

                List<SickAbilityUser.AbilityDef> newAbilityDefs = new List<SickAbilityUser.AbilityDef>();
                newAbilityDefs.Add(RH_TET_MagicDefOf.RH_TET_AddOn_RaiseGround_Mage);
                newAbilityDefs.Add(RH_TET_MagicDefOf.RH_TET_AddOn_RaiseGround_Wizard);
                newAbilityDefs.Add(RH_TET_MagicDefOf.RH_TET_AddOn_RaiseGround_Warlock);
                newAbilityDefs.Add(RH_TET_MagicDefOf.RH_TET_AddOn_RaiseGround_Master);

                MagicPower magicPower = new MagicPower(newAbilityDefs);

                addonPowers.Add(magicPower);

                Messages.Message(user.Name + "RH_TET_AddOns_NewPowerAdded".Translate() + RH_TET_MagicDefOf.RH_TET_AddOn_RaiseGround_Mage.LabelCap, (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
            }
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (!p.IsMagicUser())
            {
                failReason = "RH_TET_FailNotAMagicUser".Translate();
                return false;
            }
            else if (MagicUtility.HasReachedNonLoreSpellLimit(p))
            {
                failReason = "RH_TET_FailReachedSpellLimit".Translate();
                return false;
            }
            else if (MagicUtility.DoesPawnKnowNonLoreSpell(p, RH_TET_MagicDefOf.RH_TET_AddOn_RaiseGround_Mage))
            {
                failReason = p.Name + " " + "RH_TET_FailAlreadyKnowsSpell".Translate();
                return false;
            }
            else
            {
            }
            failReason = (string)null;
            return true;
        }
    }
}
