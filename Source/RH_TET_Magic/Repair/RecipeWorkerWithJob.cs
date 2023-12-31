using Verse;

namespace TheEndTimes_Magic
{
    public abstract class RecipeWorkerWithJob : RecipeWorker
    {
        public abstract JobDef Job { get; }
    }
}
