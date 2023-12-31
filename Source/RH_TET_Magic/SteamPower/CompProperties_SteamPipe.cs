using Verse;

namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public class CompProperties_SteamPipe : CompProperties
    {
        public PipeType mode = PipeType.Heating;
        public bool stuffed;
        public bool vertPipe;

        public CompProperties_SteamPipe()
        {
            this.compClass = typeof(CompSteamPipe);
        }
    }
}