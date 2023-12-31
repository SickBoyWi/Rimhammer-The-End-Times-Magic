using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class IncidentWorker_JabberNearby : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            return !target.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && target.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(RH_TET_MagicDefOf.RH_TET_JabberslytheRaceAnimal) && this.TryFindEntryCell(target, out IntVec3 _);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            IntVec3 cell;
            if (!this.TryFindEntryCell(target, out cell))
                return false;
            PawnKindDef animal = RH_TET_MagicDefOf.RH_TET_JabberslytheAnimal;
            int num1 = 1;
            int num2 = Rand.RangeInclusive(120000, 180000);
            IntVec3 result = IntVec3.Invalid;
            if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(cell, target, 10f, out result))
                result = IntVec3.Invalid;
            Pawn pawn = (Pawn)null;
            int minAge = 800;
            int maxAge = 950;
            for (int index = 0; index < num1; ++index)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(cell, target, 10, (Predicate<IntVec3>)null);

                float biologicalAge = (float)RH_TET_MagicMod.random.Next(minAge, maxAge);

                PawnGenerationRequest pawnRequest = new PawnGenerationRequest(animal, null, PawnGenerationContext.NonPlayer, -1,
                    true, false, false,
                    false, true, 0f, 
                    false, true, false,
                    false, false, false, 
                     false, false,false,
                    0.0f, 0.0f, null, 
                    0.0f,
                    null, null, null,
                    null, null, biologicalAge,
                    biologicalAge);

                pawn = PawnGenerator.GeneratePawn(pawnRequest);
                GenSpawn.Spawn((Thing)pawn, loc, target, Rot4.Random, WipeMode.Vanish, false);
                pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num2;
                if (result.IsValid)
                    pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(result, target, 10, (Predicate<IntVec3>)null);
            }
            this.SendStandardLetter("RH_TET_LabelJabberWandersIn".Translate((NamedArgument)animal.label).CapitalizeFirst(), "RH_TET_JabberWandersIn".Translate((NamedArgument)animal.label), LetterDefOf.PositiveEvent, parms, (LookTargets)(Thing)pawn, (NamedArgument[])Array.Empty<NamedArgument>());
            return true;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f, false, (Predicate<IntVec3>)null);
        }
    }
}
