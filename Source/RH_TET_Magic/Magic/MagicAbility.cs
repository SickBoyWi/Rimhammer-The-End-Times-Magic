
using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class MagicAbility : PawnAbility
    {
        public CompMagicUser MagicUser
        {
            get
            {
                return MagicUtility.GetMagicUser(this.Pawn);
            }
        }

        public MagicAbilityDef MagicDef
        {
            get
            {
                return this.Def as MagicAbilityDef;
            }
        }

        public MagicAbility()
            : base()
        {
        }

        public MagicAbility(CompAbilityUser abilityUser)
            : base (abilityUser)
        {
            this.abilityUser = abilityUser as CompMagicUser;
        }

        public MagicAbility(AbilityData abilityData)
            : base(abilityData)
        {
            this.abilityUser = (abilityData.Pawn.AllComps.FirstOrDefault<ThingComp>((Func<ThingComp, bool>)(x => x.GetType() == abilityData.AbilityClass)) as CompMagicUser);
        }

        public MagicAbility(Pawn user, SickAbilityUser.AbilityDef pdef)
            : base (user, pdef)
        {
        }

        public float ActualMagicCost
        {
            get
            {
                // If the winds of magic are blowing strong, then the spell costs zero.
                if (this.Pawn != null && this.Pawn.Map.gameConditionManager.ConditionIsActive(RH_TET_MagicDefOf.RH_TET_Magic_GameCondition_WindsOfMagic))
                {
                    return 0f;
                }
                // If a belt of Tzeentch is worn, then all spells cost zero magic from the pawn's pool.
                else if (this.Pawn != null && this.Pawn.apparel != null && this.Pawn.apparel.WornApparel != null)
                { 
                    List<Apparel> wornApparel = this.Pawn.apparel.WornApparel;

                    foreach (Apparel a in wornApparel)
                    {
                        if (a.def.Equals(RH_TET_MagicDefOf.RH_TET_Magic_Apparel_TzeentchRibbon))
                        {
                            return 0f;
                        }
                    }
                }

                return this.MagicDef.magicPoolCost;
            }
        }

        public override void PostAbilityAttempt()
        {
            base.PostAbilityAttempt();
            MagicAbilityDef magicDef = this.MagicDef;
            this.Pawn.needs.TryGetNeed<Need_MagicPool>()?.UseMagicPower(this.ActualMagicCost);

            // Give some experience for the casting. 
            this.MagicUser.MagicUserXP += this.MagicDef.experiencePoints;
        }

        public override string PostAbilityVerbCompDesc(VerbProperties_Ability verbDef)
        {
            string str1 = "";
            if (verbDef == null)
                return str1;

            MagicAbilityDef abilityDef = verbDef?.abilityDef as MagicAbilityDef;
            if (abilityDef != null)
            {
                StringBuilder stringBuilder = new StringBuilder();

                string str4;
                str4 = abilityDef.GetMagicPoolCostDesc();

                if (str4 != "")
                    stringBuilder.AppendLine(str4);
                str1 = stringBuilder.ToString();
            }

            return str1;
        }

        public override bool CanCastPowerCheck(AbilityContext context, out string reason)
        {
            if (!base.CanCastPowerCheck(context, out reason))
                return false;
            reason = "";
            
            if (this.Def != null)
            {
                MagicAbilityDef def = this.Def as MagicAbilityDef;
                if (def != null)
                {
                    if (this.MagicUser?.MagicPool != null && def != null && (double)def.magicPoolCost > 0.0)
                    {
                        double actualMagicCost = (double)this.ActualMagicCost;
                        float? curLevel = this.Pawn.GetMagicPool()?.CurLevel;
                        double valueOrDefault = (double)curLevel.GetValueOrDefault();
                        if (actualMagicCost > valueOrDefault & curLevel.HasValue)
                        {
                            reason = "RH_TET_DrainedMagicPool".Translate((NamedArgument)this.Pawn.LabelShort);
                            return false;
                        }
                    }
                    if (this.AbilityUser != null && this.AbilityUser?.AbilityUser?.apparel != null && this.AbilityUser?.AbilityUser?.apparel?.WornApparel != null)
                    {
                        CompAbilityUser abilityUser1 = this.AbilityUser;
                        int num1;
                        if (abilityUser1 == null)
                        {
                            num1 = 0;
                        }
                        else
                        {
                            int? wornApparelCount = abilityUser1.AbilityUser?.apparel?.WornApparelCount;
                            int num2 = 0;
                            num1 = wornApparelCount.GetValueOrDefault() > num2 & wornApparelCount.HasValue ? 1 : 0;
                        }
                        if (num1 != 0)
                        {
                            CompAbilityUser abilityUser2 = this.AbilityUser;
                            Apparel apparel1;
                            if (abilityUser2 == null)
                            {
                                apparel1 = (Apparel)null;
                            }
                            else
                            {
                                Pawn abilityUser3 = abilityUser2.AbilityUser;
                                if (abilityUser3 == null)
                                {
                                    apparel1 = (Apparel)null;
                                }
                                else
                                {
                                    Pawn_ApparelTracker apparel2 = abilityUser3.apparel;
                                    if (apparel2 == null)
                                    {
                                        apparel1 = (Apparel)null;
                                    }
                                    else
                                    {
                                        List<Apparel> wornApparel = apparel2.WornApparel;
                                        apparel1 = wornApparel != null ? wornApparel.FirstOrDefault<Apparel>((Func<Apparel, bool>)(x => x.def == ThingDefOf.Apparel_ShieldBelt)) : (Apparel)null;
                                    }
                                }
                            }
                            if (apparel1 != null)
                            {
                                reason = "RH_TET_ShieldBeltFail".Translate((NamedArgument)this.Pawn.LabelShort);
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
