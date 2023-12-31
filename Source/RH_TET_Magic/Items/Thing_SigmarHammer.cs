using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Thing_SigmarHammer : Thing_MagicWeaponWithLight
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            //this.lightDef = RH_TET_MagicDefOf.RH_TET_SigmarLight;
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void ExposeData()
        {
            //this.lightDef = RH_TET_MagicDefOf.RH_TET_SigmarLight;
            base.ExposeData();
            //Scribe_References.Look<Thing>(ref this.light, "light", false);
            //Scribe_Values.Look<bool>(ref this.lightIsOn, "lightIsOn", false, false);
            //Scribe_Values.Look<Thing_SigmarHammer.LightMode>(ref this.lightMode, "lightMode", Thing_SigmarHammer.LightMode.Automatic, false);
        }
    }
}