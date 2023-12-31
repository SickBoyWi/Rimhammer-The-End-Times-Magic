using System;
using System.Collections.Generic;
using AbilityUser;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_ShallyaArtifactRegrowth : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            
            if (user.IsFaithUserShallya())
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
                        if (pawnAbility.Def.defName.Equals(RH_TET_MagicDefOf.RH_TET_Shallya_Regrowth.defName))
                        { 
                            alreadyHad = true;
                            break;
                        }
                    }

                    if (alreadyHad)
                        Messages.Message("RH_TET_SigmarArtifactUsedNoEffect".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeEvent, true);
                    else
                    {
                        FaithPower newFaithPower = compUser.FaithData.PowersShallya.Find((Predicate<FaithPower>)(x => x.abilityDef.defName.Equals(RH_TET_MagicDefOf.RH_TET_Shallya_Regrowth.defName)));
                        compUser.GrantPower(newFaithPower);
                        SoundInfo info = SoundInfo.InMap(new TargetInfo(user.Position, user.Map, false), MaintenanceType.None);
                        SoundDefOf.Thunder_OnMap.PlayOneShot(info);
                        FleckMaker.ThrowLightningGlow(user.TrueCenter(), user.Map, 5f);
                        FleckMaker.Static(user.TrueCenter(), user.Map, FleckDefOf.PsycastAreaEffect, 2f);
                        Messages.Message("RH_TET_SigmarArtifactUsed".Translate(user.Name, ("RH_TET_ShallyaRegrowthAbility".Translate() + ".")), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
                    }
                }
            }
            else
            {
                Messages.Message("RH_TET_SigmarArtifactUsedNoEffect".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeEvent, true);
            }
        }


        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (p.IsFaithUserShallya())
            {
                failReason = (string)null;
                return true;
            }
            else
            {
                failReason = "RH_TET_SigmarArtifactCannotUse".Translate("Shallya");
                return false;
            }
        }
    }
}
