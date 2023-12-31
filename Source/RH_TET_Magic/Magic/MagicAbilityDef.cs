using AbilityUser;
using System.Text;
using Verse;

namespace TheEndTimes_Magic
{
    public class MagicAbilityDef : AbilityDef
    {
        public float magicPoolCost;
        public int experiencePoints;
        public int abilityPoints;
        public int nextMagicLevelPointsRequired; 

        public string GetPointDesc()
        {
            StringBuilder stringBuilder = new StringBuilder(); 
            if (this.nextMagicLevelPointsRequired > 0)
                stringBuilder.AppendLine("RH_TET_PointsRequired".Translate((object)this.nextMagicLevelPointsRequired));
            return stringBuilder.ToString();
        }

        public string GetMagicPoolCostDesc()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.magicPoolCost > 0f)
                stringBuilder.AppendLine("RH_TET_PoolPercentRequired".Translate(this.magicPoolCost * 100f));
            return stringBuilder.ToString();
        }

        public MagicAbilityDef()
            : base()
        {
        }
    }
}
