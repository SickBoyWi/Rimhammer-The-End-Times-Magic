using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;

/**
 * Original code from Jecrell's Cult Mod - Tweaked to fit a Rimhammer Beastmen Use by SickBoyWI.
 * */
namespace TheEndTimes_Magic
{
    public class WorldComponent_DarkGods : WorldComponent
    {
        public Dictionary<DarkGod, int> GodCache = new Dictionary<DarkGod, int>();
        public bool AreGodsSpawned = false;

        public WorldComponent_DarkGods(World world) : base(world)
        {
        }

        public DarkGod GetCache(DarkGod god)
        {
            DarkGod result;
            bool flag1 = GodCache == null;
            if (flag1)
            {
                GodCache = new Dictionary<DarkGod, int>();
            }

            foreach (DarkGod current in GodCache.Keys)
            {
                if (current == god)
                {
                    result = current;
                    return result;
                }
            }

            DarkGod darkGod = god;
            GodCache.Add(god, 1);
            result = god;
            return result;
        }

        public void orGenerate()
        {
            if (!AreGodsSpawned)
            {
                foreach (ThingDef current in DefDatabase<ThingDef>.AllDefs)
                {
                    if (current.thingClass == typeof(DarkGod))
                    {
                        DarkGod x = new DarkGod(current);
                        ThingIDMaker.GiveIDTo(x);
                        GetCache(x);
                    }
                }
                AreGodsSpawned = true;
            }
            return;
        }

        public override void WorldComponentTick()
        {
            orGenerate();
            base.WorldComponentTick();
        }

        public void ReloadDarkGod(DarkGod god)
        {
            float currentFavor = god.PlayerFavor;

            //Remove god
            GodCache.Remove(god);

            //New deity
            DarkGod x = new DarkGod(god.def);
            x.AffectFavor(currentFavor);
            GodCache.Add(x, 1);

            //Destroy deity
            god.Destroy(0);

            return;
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<DarkGod, int>(ref this.GodCache, "DarkGods", LookMode.Deep, LookMode.Value);
            Scribe_Values.Look<bool>(ref this.AreGodsSpawned, "AreDarkGodsSpawned", false, false);
            base.ExposeData();
            if (GodCache == null)
            {
                GodCache = new Dictionary<DarkGod, int>();
            }
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                orGenerate();
            }
        }
    }
}