
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
    public class AbilityActionAbility : PawnAbility
    {
        public CompAbilityActionUser AbilityActionUser
        {
            get
            {
                return MagicUtility.GetAbilityActionUser(this.Pawn);
            }
        }

        public AbilityActionAbilityDef AbilityActionDef
        {
            get
            {
                return this.Def as AbilityActionAbilityDef;
            }
        }

        public AbilityActionAbility()
            : base()
        {
        }

        public AbilityActionAbility(CompAbilityUser abilityUser)
            : base (abilityUser)
        {
            this.AbilityUser = abilityUser as CompAbilityActionUser;
        }

        public AbilityActionAbility(AbilityData abilityData)
            : base(abilityData)
        {
            this.AbilityUser = (abilityData.Pawn.AllComps.FirstOrDefault<ThingComp>((Func<ThingComp, bool>)(x => x.GetType() == abilityData.AbilityClass)) as CompAbilityActionUser);
        }

        public AbilityActionAbility(Pawn user, SickAbilityUser.AbilityDef pdef)
            : base (user, pdef)
        {
        }

        private float ActualAbilityCost
        {
            get
            {
                return this.AbilityActionDef.abilityPoolCost;
            }
        }

        public override void PostAbilityAttempt()
        {
            base.PostAbilityAttempt();
            AbilityActionAbilityDef abilDef = this.AbilityActionDef;
            this.Pawn.needs.TryGetNeed<Need_AbilityPool>()?.UseAbilityActionPower(this.ActualAbilityCost);
        }

        public override string PostAbilityVerbCompDesc(VerbProperties_Ability verbDef)
        {
            string str1 = "";
            if (verbDef == null)
                return str1;

            AbilityActionAbilityDef abilityDef = verbDef?.abilityDef as AbilityActionAbilityDef;
            if (abilityDef != null)
            {
                StringBuilder stringBuilder = new StringBuilder();

                string str4;
                str4 = abilityDef.GetAbilityPoolCostDesc();

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
                AbilityActionAbilityDef def = this.Def as AbilityActionAbilityDef;
                if (def != null)
                {
                    if (this.AbilityActionUser?.AbilityPool != null && def != null && (double)def.abilityPoolCost > 0.0)
                    {
                        double actualAbilityCost = (double)this.ActualAbilityCost;
                        float? curLevel = this.Pawn.GetAbilityActionPool()?.CurLevel;
                        double valueOrDefault = (double)curLevel.GetValueOrDefault();
                        if (actualAbilityCost > valueOrDefault & curLevel.HasValue)
                        {
                            reason = "RH_TET_DrainedAbilityPool".Translate((NamedArgument)this.Pawn.LabelShort);
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
                                        apparel1 = wornApparel != null ? wornApparel.FirstOrDefault<Apparel>((Func<Apparel, bool>)(x => (x.def == ThingDefOf.Apparel_ShieldBelt || x.def == RH_TET_MagicDefOf.RH_TET_Magic_Apparel_KhorneBelt))) : (Apparel)null;
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
