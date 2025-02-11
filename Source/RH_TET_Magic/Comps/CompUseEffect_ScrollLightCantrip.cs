﻿using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_ScrollLightCantrip : CompUseEffect
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
                newAbilityDefs.Add(RH_TET_MagicDefOf.RH_TET_AddOn_LightCantrip_Mage);
                newAbilityDefs.Add(RH_TET_MagicDefOf.RH_TET_AddOn_LightCantrip_Wizard);
                newAbilityDefs.Add(RH_TET_MagicDefOf.RH_TET_AddOn_LightCantrip_Warlock);
                newAbilityDefs.Add(RH_TET_MagicDefOf.RH_TET_AddOn_LightCantrip_Master);

                MagicPower magicPower = new MagicPower(newAbilityDefs);

                addonPowers.Add(magicPower);

                Messages.Message(user.Name + "RH_TET_AddOns_NewPowerAdded".Translate() + RH_TET_MagicDefOf.RH_TET_AddOn_LightCantrip_Mage.LabelCap, (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
            }
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            string failReason;
            if (!p.IsMagicUser())
            {
                failReason = "RH_TET_FailNotAMagicUser".Translate();
            }
            else if (MagicUtility.HasReachedNonLoreSpellLimit(p))
            {
                failReason = "RH_TET_FailReachedSpellLimit".Translate();
            }
            else if (MagicUtility.DoesPawnKnowNonLoreSpell(p, RH_TET_MagicDefOf.RH_TET_AddOn_LightCantrip_Mage))
            {
                failReason = p.Name + " " + "RH_TET_FailAlreadyKnowsSpell".Translate();
            }
            else
                failReason = null;

            if (failReason is null)
                return base.CanBeUsedBy(p);
            else
                return (AcceptanceReport)failReason;
        }
    }
}
