using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Verb_GrimResolve : Verb_UseAbilityAction
    {
        private static List<Thing> tmpThingsToDestroy = new List<Thing>();

        protected override bool TryCastShot()
        {
            bool outResult = base.TryCastShot();
            Map map = caster.Map;

            IntVec3 casterPosition = caster.Position;
            FleckMaker.ThrowMicroSparks(casterPosition.ToVector3(), map);
            FleckMaker.Static(casterPosition, map, FleckDefOf.PsycastAreaEffect, 1.2f);

            //p.health.AddHediff(RH_TET_MagicDefOf.RH_TET_WH_GrimResolve);
            this.CasterPawn.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(RH_TET_MagicDefOf.RH_TET_GrimResolve_Mood), (Pawn)null);
            FleckMaker.Static(casterPosition, map, RH_TET_MagicDefOf.RH_TET_FleckYellowEffect, 2f);
            //FleckMaker.Static(casterPosition, map, FleckDefOf.PsycastAreaEffect, .5f);

            FleckMaker.ThrowLightningGlow(casterPosition.ToVector3(), map, 1.0f);

            return outResult;
        }
    }
  
}
