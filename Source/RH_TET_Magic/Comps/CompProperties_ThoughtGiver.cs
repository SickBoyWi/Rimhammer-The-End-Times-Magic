using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * TODO
     * Unused. intended for apparel, but there has to be a better way to grant a 
     * thought to wearer without it always timing out and being readded in comptick.
     * */
    public class CompProperties_ThoughtGiver : CompProperties
    {
        public ThoughtDef thoughtDef;

        public CompProperties_ThoughtGiver()
        {
            this.compClass = typeof(CompThoughtGiver);
        }
    }
}
