using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

// TODO REMOVED LIGHT FOR 1.4 TO NOT SLAG OUT GAME.
namespace TheEndTimes_Magic
{
    public class Thing_KhorneAxe : Thing_MagicWeaponWithLight
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            //this.lightDef = RH_TET_MagicDefOf.RH_TET_KhorneLight;
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void ExposeData()
        {
            // Leave light stuff in for ref, but having a bunch of these is a real drag on performance.

            //this.lightDef = RH_TET_MagicDefOf.RH_TET_KhorneLight;
            base.ExposeData();
            //Scribe_References.Look<Thing>(ref this.light, "light", false);
            //Scribe_Values.Look<bool>(ref this.lightIsOn, "lightIsOn", false, false);
            //Scribe_Values.Look<Thing_KhorneAxe.LightMode>(ref this.lightMode, "lightMode", Thing_KhorneAxe.LightMode.Automatic, false);
        }
    }
}