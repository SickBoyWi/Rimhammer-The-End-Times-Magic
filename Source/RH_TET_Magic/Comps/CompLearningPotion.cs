using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompLearningPotion : CompUsable
    {
        public SkillDef skill;
        public AbilityDef ability;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look<SkillDef>(ref this.skill, "skill");
            Scribe_Defs.Look<AbilityDef>(ref this.ability, "ability");
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            CompProperties_LearningPotion propertiesNeurotrainer = (CompProperties_LearningPotion)props;
            this.ability = propertiesNeurotrainer.ability;
            this.skill = propertiesNeurotrainer.skill;
        }

        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return string.Format(this.Props.useLabel, this.skill != null ? (object)this.skill.skillLabel : (object)this.ability.label);
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
                return false;
            CompLearningPotion comp = other.TryGetComp<CompLearningPotion>();
            return comp != null && comp.skill == this.skill && comp.ability == this.ability;
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompLearningPotion comp = piece.TryGetComp<CompLearningPotion>();
            if (comp == null)
                return;
            comp.skill = this.skill;
            comp.ability = this.ability;
        }
    }
}
