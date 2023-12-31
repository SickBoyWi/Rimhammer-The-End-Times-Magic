using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld.Planet;
using RimWorld;

/**
 * Original code from Jecrell's Cult Mod - Tweaked to fit a Rimhammer Beastmen Use by SickBoyWI.
 * */
namespace TheEndTimes_Magic
{
    public static class DarkGodTracker
    {
        public static WorldComponent_DarkGods Get
        {
            get
            {
                return Find.World.GetComponent<WorldComponent_DarkGods>();
            }
        }

        public static DarkGod FindDarkGod(DarkGodDef darkGodDef)
        {
            List<DarkGod> darkGodsList = new List<DarkGod>(Find.World.GetComponent<WorldComponent_DarkGods>().GodCache.Keys);

            foreach (DarkGod dg in darkGodsList)
            {
                if (dg.def.Equals(darkGodDef))
                {
                    return dg;
                }
            }
            return null;
        }
    }
}