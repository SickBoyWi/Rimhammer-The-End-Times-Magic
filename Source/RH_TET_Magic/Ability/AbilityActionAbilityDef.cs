using AbilityUser;
using System.Text;
using Verse;

namespace TheEndTimes_Magic
{
    public class AbilityActionAbilityDef : AbilityDef
    {
        public float abilityPoolCost;
        public int abilityPoints;

        public string GetPointDesc()
        {
            StringBuilder stringBuilder = new StringBuilder(); 
            return stringBuilder.ToString();
        }

        public string GetAbilityPoolCostDesc()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.abilityPoolCost > 0f)
                stringBuilder.AppendLine("RH_TET_AbilityPoolPercentRequired".Translate(this.abilityPoolCost * 100f));
            return stringBuilder.ToString();
        }

        public AbilityActionAbilityDef()
            : base()
        {
        }
    }
}
