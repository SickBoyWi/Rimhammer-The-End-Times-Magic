using Verse;

namespace TheEndTimes_Magic
{
    public class RecipeWorkerWithJob_Repair : RecipeWorkerWithJob
    {
        public override JobDef Job
        {
            get
            {
                return RH_TET_MagicDefOf.RH_TET_Magic_RepairStuff;
            }
        }
    }
}
