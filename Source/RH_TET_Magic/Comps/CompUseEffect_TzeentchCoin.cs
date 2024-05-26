using System;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_TzeentchCoin : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);

            int randomNumber = RH_TET_MagicMod.random.Next(0, 2);

            if (randomNumber == 0)
            { 
                // Good
                CompMagicUser compMagicUser = ((user != null) ? user.TryGetComp<CompMagicUser>() : null);
                if (compMagicUser != null && compMagicUser.IsMagicUser)
                {
                    // Shaman/magic user specific rewards.

                    compMagicUser.MagicData.XP += 1800;

                    Messages.Message("RH_TET_CoinOfTzeentchGood".Translate(user.Name, (900 + " " + "RH_TET_ExperienceMagic".Translate() + ".")), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
                }
                else
                {
                    if (user.skills == null)
                    {
                        return;
                    }

                    SkillDef skill = this.GetLegitSkill(user);

                    if (skill != null)
                    { 
                        user.skills.Learn(skill, 54000, true);
                        int level2 = user.skills.GetSkill(skill).Level;

                        Messages.Message("RH_TET_CoinOfTzeentchGood".Translate(user.Name, ("54,000 " + skill.label + " " + "RH_TET_Experience".Translate() + ".")), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
                    }
                }
            }
            else
            {
                // Bad
                DamageInfo dinfo;

                DamageDef damageDef = RH_TET_MagicDefOf.RH_TET_MagicalInjury;
                
                float num2 = 999f;
                BodyPartRecord randomBodyPart = user.health.hediffSet.GetRandomNotMissingPart(damageDef);

                DamageDef def = damageDef;

                double num3 = 27D;
                double num4 = (double)num2;

                dinfo = new DamageInfo(def, (float)num3, (float)num4, -1f, (Thing)null, randomBodyPart, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Thing)null);

                user.TakeDamage(dinfo);
                FleckMaker.ThrowMicroSparks(user.DrawPos, user.Map);

                Messages.Message("RH_TET_CoinOfTzeentchBad".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeHealthEvent, true);
            }
        }

        private SkillDef GetLegitSkill(Pawn user)
        {
            if (!user.skills.GetSkill(SkillDefOf.Intellectual).TotallyDisabled)
                return SkillDefOf.Intellectual;
            else if (!user.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
                return SkillDefOf.Social;
            else if (!user.skills.GetSkill(SkillDefOf.Medicine).TotallyDisabled)
                return SkillDefOf.Medicine;
            else if (!user.skills.GetSkill(SkillDefOf.Plants).TotallyDisabled)
                return SkillDefOf.Plants;
            else if (!user.skills.GetSkill(SkillDefOf.Artistic).TotallyDisabled)
                return SkillDefOf.Artistic;
            else if (!user.skills.GetSkill(SkillDefOf.Animals).TotallyDisabled)
                return SkillDefOf.Animals;
            else if (!user.skills.GetSkill(SkillDefOf.Crafting).TotallyDisabled)
                return SkillDefOf.Crafting;
            else if (!user.skills.GetSkill(SkillDefOf.Shooting).TotallyDisabled)
                return SkillDefOf.Shooting;
            else if (!user.skills.GetSkill(SkillDefOf.Melee).TotallyDisabled)
                return SkillDefOf.Melee;
            else if (!user.skills.GetSkill(SkillDefOf.Construction).TotallyDisabled)
                return SkillDefOf.Construction;
            else if (!user.skills.GetSkill(SkillDefOf.Cooking).TotallyDisabled)
                return SkillDefOf.Cooking;
            else if (!user.skills.GetSkill(SkillDefOf.Mining).TotallyDisabled)
                return SkillDefOf.Mining;
            else
                return null;
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            return base.CanBeUsedBy(p);
        }
    }
}
