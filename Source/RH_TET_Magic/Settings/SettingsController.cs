using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "Rimhammer - Magic";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }

    public class Settings : ModSettings
    {
        public static bool UseMiscasts = true;
        public static bool AllowDropPodRaids = true;
        private static Vector2 scrollPosition = Vector2.zero;
        private const int GAP_SIZE = 24;
        public static bool SteamPipesVisible = true;
        public static bool showLoreIconInColonistBar = true;
        public float classIconSize = 1f; 
        public static Settings Instance;

        public Settings()
        {
            Settings.Instance = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<bool>(ref UseMiscasts, "RH_TET_UseMiscasts", true, false);
            Scribe_Values.Look<bool>(ref AllowDropPodRaids, "RH_TET_AllowDropPodRaids", true, false);
        }

        public static void DoSettingsWindowContents(Rect rect)
        {
            Rect scroll = new Rect(5f, 45f, 430, rect.height - 40);
            Rect view = new Rect(0, 45, 400, 1200);

            Widgets.BeginScrollView(scroll, ref scrollPosition, view, true);
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(view);

            ls.CheckboxLabeled("RH_TET_Magic_UseMiscasts".Translate(), ref UseMiscasts);
            ls.Label("RH_TET_Magic_UseMiscastsInfo".Translate());
            ls.Gap(GAP_SIZE);

            ls.CheckboxLabeled("RH_TET_Magic_AllowMagicDropPodRaids".Translate(), ref AllowDropPodRaids);
            ls.Label("RH_TET_Magic_AllowMagicDropPodRaidsInfo".Translate());
            ls.Gap(GAP_SIZE);

            ls.End();
            Widgets.EndScrollView();
        }
    }
}