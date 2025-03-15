using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace TheEndTimes_Magic
{
    public class GameCondition_WindsOfMagic : GameCondition
    {
        public const int CheckInterval = 3451;

        public override void Init()
        {
            base.Init();
        }

        public static void CheckPawn(Pawn pawn)
        {
            if (!pawn.RaceProps.Humanlike || pawn.health.hediffSet.HasHediff(RH_TET_MagicDefOf.RH_TET_Magic_MentalSuppressionHE, false))
                return;
            pawn.health.AddHediff(RH_TET_MagicDefOf.RH_TET_Magic_MentalSuppressionHE, (BodyPartRecord)null, new DamageInfo?(), (DamageWorker.DamageResult)null);
        }

        public override void GameConditionTick()
        {
            List<Map> affectedMaps = this.AffectedMaps;
            // Confirm pawn state.
            if (Find.TickManager.TicksGame % CheckInterval == 2000)
            {
                foreach (Map affectedMap in this.AffectedMaps)
                {
                    List<Pawn> allPawns = affectedMap.mapPawns.AllPawns;
                    for (int index = 0; index < allPawns.Count; ++index)
                        GameCondition_WindsOfMagic.CheckPawn(allPawns[index]);
                }
            }
            // Do winds of magic effects.
            if (Find.TickManager.TicksGame % CheckInterval == 50)
            {
                Map map = this.AffectedMaps[0];
                IntVec3 intVec3 = CellFinder.RandomCell(map);
                bool found = false;

                while (!found)
                {
                    if (IsOpen(intVec3, map))
                    {
                        found = true;
                        continue;
                    }

                    intVec3 = CellFinder.RandomCell(map);
                }

                // Do effect at cell.
                Thing filth = ThingMaker.MakeThing(RH_TET_MagicDefOf.RH_TET_Filth_BloodDaemon);
                SickTools.DelayedEffecterSpawnerNonPawn.Spawn(filth, intVec3, map, 10, RH_TET_MagicDefOf.RH_TET_Daemons_EmergencePointSustained2X2, RH_TET_MagicDefOf.RH_TET_Daemons_EmergencePointComplete2X2, RH_TET_MagicDefOf.RH_TET_Magic_SoundWhoosh);

                // Spawn magic essence.
                if (Find.TickManager.TicksGame % CheckInterval == 8500)
                {                
                    // Drop some goodies.
                    Thing rawEssence = ThingMaker.MakeThing(RH_TET_MagicDefOf.RH_TET_Daemons_MagicEssence_Raw);
                    rawEssence.stackCount = RH_TET_MagicMod.random.Next(1, 9);
                    SickTools.DelayedEffecterSpawnerNonPawn.Spawn(null, intVec3, map, 10, RH_TET_MagicDefOf.RH_TET_Daemons_EmergencePointSustained2X2, RH_TET_MagicDefOf.RH_TET_Daemons_EmergencePointComplete2X2, RH_TET_MagicDefOf.RH_TET_Magic_SoundWhoosh);

                    Messages.Message("RH_TET_MagicEssenceCongealed".Translate(), (LookTargets)((Thing)rawEssence), MessageTypeDefOf.PositiveEvent, false);
                }
            }
        }

        private bool IsOpen(IntVec3 cell, Map map)
        {
            return cell.GetFirstItem(map) == null && cell.Standable(map) && !cell.Fogged(map);
        }
    }
}
