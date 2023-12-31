using AbilityUserAI;
using Verse;

// This code is from the Torann Magic mod. 
namespace TheEndTimes_Magic
{
    public class AbilityDecisionConditionalNode_EnemyTargetIsAnimal : AbilityDecisionNode
    {
        public override bool CanContinueTraversing(Pawn caster)
        {
            try
            { 
                Pawn p = caster.mindState.enemyTarget as Pawn;
                return p.RaceProps.Animal;
            }
            catch
            {
                // Ignore.
            }

            return false;
        }

        public AbilityDecisionConditionalNode_EnemyTargetIsAnimal()
            : base()
        {
        }
    }
}
