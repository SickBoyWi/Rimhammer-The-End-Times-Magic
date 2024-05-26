using System;
using System.Collections.Generic;
using SickAbilityUser;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_SigmarArtifactComet : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            
            if (user.IsFaithUserSigmar())
            {
                CompFaithUser compUser = ((user != null) ? user.TryGetComp<CompFaithUser>() : null);
                if (compUser != null)
                {
                    List<FaithAbility> abilityList = new List<FaithAbility>();
                    using (List<PawnAbility>.Enumerator enumerator = compUser.AbilityData.Powers.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            PawnAbility current = enumerator.Current;
                            abilityList.Add(current as FaithAbility);
                        }
                    }
                    bool alreadyHad = false;
                    foreach (PawnAbility pawnAbility in abilityList)
                    {
                        if (pawnAbility.Def.defName.Equals(RH_TET_MagicDefOf.RH_TET_Sigmar_Comet.defName))
                        { 
                            alreadyHad = true;
                            break;
                        }
                    }

                    if (alreadyHad)
                        Messages.Message("RH_TET_SigmarArtifactUsedNoEffect".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeEvent, true);
                    else
                    {
                        FaithPower newFaithPower = compUser.FaithData.PowersSigmar.Find((Predicate<FaithPower>)(x => x.abilityDef.defName.Equals(RH_TET_MagicDefOf.RH_TET_Sigmar_Comet.defName)));
                        compUser.GrantPower(newFaithPower);
                        SoundInfo info = SoundInfo.InMap(new TargetInfo(user.Position, user.Map, false), MaintenanceType.None);
                        SoundDefOf.Thunder_OnMap.PlayOneShot(info);
                        FleckMaker.ThrowLightningGlow(user.TrueCenter(), user.Map, 5f);
                        FleckMaker.Static(user.TrueCenter(), user.Map, FleckDefOf.PsycastAreaEffect, 2f);
                        Messages.Message("RH_TET_SigmarArtifactUsed".Translate(user.Name, ("RH_TET_SigmarCometAbility".Translate() + ".")), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
                    }
                }
            }
            else
            {
                Messages.Message("RH_TET_SigmarArtifactUsedNoEffect".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeEvent, true);
            }
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (p.IsFaithUserSigmar())
            {
                return base.CanBeUsedBy(p);
            }
            else
            {
                return (AcceptanceReport)"RH_TET_SigmarArtifactCannotUse".Translate("Sigmar");
            }
        }
    }
}
