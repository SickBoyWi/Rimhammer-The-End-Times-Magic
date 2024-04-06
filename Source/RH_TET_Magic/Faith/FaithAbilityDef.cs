using SickAbilityUser;
using System.Text;
using Verse;

namespace TheEndTimes_Magic
{
    public class FaithAbilityDef : AbilityDef
    {
        public float faithPoolCost;
        public int abilityPoints;

        public string GetPointDesc()
        {
            StringBuilder stringBuilder = new StringBuilder(); 
            return stringBuilder.ToString();
        }

        public string GetFaithPoolCostDesc()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.faithPoolCost > 0f)
                stringBuilder.AppendLine("RH_TET_FaithPoolPercentRequired".Translate(this.faithPoolCost * 100f));
            return stringBuilder.ToString();
        }

        public FaithAbilityDef()
            : base()
        {
        }
    }
}
