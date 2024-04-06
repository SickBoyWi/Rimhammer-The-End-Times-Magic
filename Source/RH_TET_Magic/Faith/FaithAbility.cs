
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
    public class FaithAbility : PawnAbility
    {
        public CompFaithUser FaithUser
        {
            get
            {
                return MagicUtility.GetFaithUser(this.Pawn);
            }
        }

        public FaithAbilityDef FaithDef
        {
            get
            {
                return this.Def as FaithAbilityDef;
            }
        }

        public FaithAbility()
            : base()
        {
        }

        public FaithAbility(CompAbilityUser abilityUser)
            : base (abilityUser)
        {
            this.abilityUser = abilityUser as CompFaithUser;
        }

        public FaithAbility(AbilityData abilityData)
            : base(abilityData)
        {
            this.abilityUser = (abilityData.Pawn.AllComps.FirstOrDefault<ThingComp>((Func<ThingComp, bool>)(x => x.GetType() == abilityData.AbilityClass)) as CompFaithUser);
        }

        public FaithAbility(Pawn user, SickAbilityUser.AbilityDef pdef)
            : base (user, pdef)
        {
        }

        private float ActualFaithCost
        {
            get
            {
                return this.FaithDef.faithPoolCost;
            }
        }

        public override void PostAbilityAttempt()
        {
            base.PostAbilityAttempt();
            FaithAbilityDef faithDef = this.FaithDef;
            this.Pawn.needs.TryGetNeed<Need_FaithPool>()?.UseFaithPower(this.ActualFaithCost);
        }

        public override string PostAbilityVerbCompDesc(VerbProperties_Ability verbDef)
        {
            string str1 = "";
            if (verbDef == null)
                return str1;

            FaithAbilityDef abilityDef = verbDef?.abilityDef as FaithAbilityDef;
            if (abilityDef != null)
            {
                StringBuilder stringBuilder = new StringBuilder();

                string str4;
                str4 = abilityDef.GetFaithPoolCostDesc();

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
                FaithAbilityDef def = this.Def as FaithAbilityDef;
                if (def != null)
                {
                    if (this.FaithUser?.FaithPool != null && def != null && (double)def.faithPoolCost > 0.0)
                    {
                        double actualFaithCost = (double)this.ActualFaithCost;
                        float? curLevel = this.Pawn.GetFaithPool()?.CurLevel;
                        double valueOrDefault = (double)curLevel.GetValueOrDefault();
                        if (actualFaithCost > valueOrDefault & curLevel.HasValue)
                        {
                            reason = "RH_TET_DrainedFaithPool".Translate((NamedArgument)this.Pawn.LabelShort);
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
