using Verse;

namespace TheEndTimes_Magic
{
    public class CompProperties_SteamUser : CompProperties_HeatStore
    {
        public SoundDef soundAmbientOn;

        public CompProperties_SteamUser()
        {
            this.compClass = typeof(CompSteamUser);
        }
    }
}