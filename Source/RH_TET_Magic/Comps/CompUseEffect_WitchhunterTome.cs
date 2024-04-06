using System;
using System.Collections.Generic;
using SickAbilityUser;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_WitchhunterTome : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            
            if (user.IsWitchhunter())
            {
                CompAbilityActionUser compUser = ((user != null) ? user.TryGetComp<CompAbilityActionUser>() : null);
                if (compUser != null)
                {
                    List<AbilityActionAbility> abilitiesPossesssed = new List<AbilityActionAbility>();
                    List<SickAbilityUser.AbilityDef> abilitiesNotPossesssed = new List<SickAbilityUser.AbilityDef>();
                    using (List<PawnAbility>.Enumerator enumerator = compUser.AbilityData.Powers.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            PawnAbility current = enumerator.Current;
                            abilitiesPossesssed.Add(current as AbilityActionAbility);
                        }
                    }
                    bool alreadyHad = false;

                    List<AbilityActionPower> allPowers = compUser.AbilityActionData.PowersWitchHunter;
                    List<SickAbilityUser.AbilityDef> allPowerDefs = new List<SickAbilityUser.AbilityDef>();

                    foreach (AbilityActionPower actionPower in allPowers)
                    {
                        foreach (SickAbilityUser.AbilityDef abilityDef in actionPower.abilityDefs)
                        {
                            allPowerDefs.Add(abilityDef);
                        }
                    }

                    foreach (SickAbilityUser.AbilityDef abilityDef in allPowerDefs)
                    {
                        bool matched = false;
                        foreach(AbilityActionAbility possessedAbility in abilitiesPossesssed)
                        {
                            if (possessedAbility.Def.Equals(abilityDef))
                            {
                                matched = true;
                            }
                        }

                        if (!matched)
                        {
                            abilitiesNotPossesssed.Add(abilityDef);
                        }
                    }

                    if (abilitiesNotPossesssed.NullOrEmpty())
                    {
                        Messages.Message("RH_TET_AbilityArtifactUsedNoEffect".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeEvent, true);
                        return;
                    }

                    SickAbilityUser.AbilityDef newAbility = abilitiesNotPossesssed.RandomElement();
                    AbilityActionPower newPower = compUser.AbilityActionData.PowersWitchHunter.Find((Predicate<AbilityActionPower>)(x => x.abilityDef.defName.Equals(newAbility.defName)));
                    compUser.GrantPower(newPower);
                    SoundInfo info = SoundInfo.InMap(new TargetInfo(user.Position, user.Map, false), MaintenanceType.None);
                    SoundDefOf.Thunder_OnMap.PlayOneShot(info);
                    FleckMaker.ThrowLightningGlow(user.TrueCenter(), user.Map, 5f);
                    FleckMaker.Static(user.TrueCenter(), user.Map, FleckDefOf.PsycastAreaEffect, 2f);
                    Messages.Message("RH_TET_AbilityArtifactUsed".Translate(user.Name, (newAbility.LabelCap + ".")), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
                }
            }
            else
            {
                Messages.Message("RH_TET_AbilityArtifactUsedNoEffect".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeEvent, true);
            }
        }


        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (p.IsWitchhunter())
            {
                failReason = (string)null;
                return true;
            }
            else
            {
                failReason = "RH_TET_AbilityArtifactCannotUse".Translate("Witchchunter");
                return false;
            }
        }
    }
}
