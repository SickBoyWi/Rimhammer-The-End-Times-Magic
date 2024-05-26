using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_LearnSkill : CompUseEffect
    {
        private const float XPGainAmount = 50000f;

        private SkillDef Skill
        {
            get
            {
                return this.parent.GetComp<CompLearningPotion>().skill;
            }
        }

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            SkillDef skill = this.Skill;
            int level1 = user.skills.GetSkill(skill).Level;
            user.skills.Learn(skill, 50000f, true);
            int level2 = user.skills.GetSkill(skill).Level;
            if (!PawnUtility.ShouldSendNotificationAbout(user))
                return;
            Messages.Message((string)"RH_TET_Magic_PotionSkillTrainerUsed".Translate((NamedArgument)user.LabelShort, (NamedArgument)skill.LabelCap, (NamedArgument)level1, (NamedArgument)level2, user.Named("USER")), (LookTargets)(Thing)user, MessageTypeDefOf.PositiveEvent, true);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (p.skills == null)
                return (AcceptanceReport)"SkillDisabled".Translate();

            if (!p.skills.GetSkill(this.Skill).TotallyDisabled)
                return base.CanBeUsedBy(p);

            return(AcceptanceReport)"SkillDisabled".Translate();
        }
    }
}
