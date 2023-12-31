using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Verb_ShallyaFortune : Verb_UseAbility
    {
        private static List<Thing> tmpThingsToDestroy = new List<Thing>();

        protected override bool TryCastShot()
        {
            bool outResult = false;
            Pawn casterPawn = this.CasterPawn;
            Map map = caster.Map;
            Faction faction = caster.Faction;

            IntVec3 startingLocation = caster.Position;
            FleckMaker.ThrowMicroSparks(startingLocation.ToVector3(), map);

            List<IntVec3> openCells = new List<IntVec3>();
            int num = GenRadial.NumCellsInRadius(this.UseAbilityProps.TargetAoEProperties.range);

            for (int index = 0; index < num; ++index)
            {
                IntVec3 intVec3 = startingLocation + GenRadial.RadialPattern[index];
                if (intVec3.InBounds(map) || intVec3.Impassable(map))
                {
                    openCells.Add(intVec3);
                }
            }

            FleckMaker.Static(casterPawn.Position, map, FleckDefOf.PsycastAreaEffect, 1.2f);

            for (int index1 = 0; index1 < openCells.Count; ++index1)
            {
                IntVec3 openCell = openCells[index1];

                Verb_ShallyaFortune.tmpThingsToDestroy.Clear();
                Verb_ShallyaFortune.tmpThingsToDestroy.AddRange((IEnumerable<Thing>)openCell.GetThingList(map));
                for (int index = 0; index < Verb_ShallyaFortune.tmpThingsToDestroy.Count; ++index)
                {
                    if (Verb_ShallyaFortune.tmpThingsToDestroy[index].def.IsFilth)
                    {
                        Verb_ShallyaFortune.tmpThingsToDestroy[index].Destroy(DestroyMode.Vanish);
                    }
                    else if (Verb_ShallyaFortune.tmpThingsToDestroy[index] is Pawn)
                    {
                        Pawn p = Verb_ShallyaFortune.tmpThingsToDestroy[index] as Pawn;
                        if (p.Faction != null && p.RaceProps.Humanlike && p.Faction.Equals(faction))
                        {
                            p.health.AddHediff(RH_TET_MagicDefOf.RH_TET_ShallyaFortune);
                            p.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(RH_TET_MagicDefOf.RH_TET_ShallyaFortuneThought), (Pawn)null);
                            FleckMaker.Static(p.Position, map, RH_TET_MagicDefOf.RH_TET_FleckYellowEffect, 2f);
                            FleckMaker.Static(p.Position, map, FleckDefOf.PsycastAreaEffect, .5f);
                        }
                    }
                }

                FleckMaker.ThrowLightningGlow(openCell.ToVector3(), map, 1.0f);
            }

            return outResult;
    }
  }
}
