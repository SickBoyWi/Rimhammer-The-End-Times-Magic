using AbilityUserAI;
using Verse;

// This code is from the Torann Magic mod. 
namespace TheEndTimes_Magic
{
    public class AbilityDecisionConditionalNode_EnemyTargetIsBuilding : AbilityDecisionNode
    {
        public override bool CanContinueTraversing(Pawn caster)
        {
            bool flag1;
            if (caster.mindState.enemyTarget == null)
                flag1 = false;
            else if (!(caster.mindState.enemyTarget is Building))
            {
                flag1 = false;
            }
            else
            {
                bool flag2 = true;
                flag1 = !(bool)this.invert ? flag2 : !flag2;
            }
            return flag1;
        }

        public AbilityDecisionConditionalNode_EnemyTargetIsBuilding()
            : base()
        {
        }
    }
}
