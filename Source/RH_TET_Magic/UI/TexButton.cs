using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public static class TexButton
    {
        public static readonly Texture2D RH_TET_MagicPoint = ContentFinder<Texture2D>.Get("UI/Magic/MagicPoint", true);
        public static readonly Texture2D RH_TET_MagicPointOff = ContentFinder<Texture2D>.Get("UI/Magic/MagicPointOff", true);
        public static readonly Texture2D RH_TET_MagicPointRenew = ContentFinder<Texture2D>.Get("UI/Magic/MagicPointRenew", true);
    }
}
