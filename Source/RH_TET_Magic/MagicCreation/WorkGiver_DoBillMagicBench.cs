using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class WorkGiver_DoBillMagicBench : WorkGiver_DoBill
    {
        private static List<IngredientCount> missingIngredients = new List<IngredientCount>();
        private static List<Thing> tmpMissingUniqueIngredients = new List<Thing>();
        private static readonly IntRange ReCheckFailedBillTicksRange = new IntRange(500, 600);
        private static List<Thing> relevantThings = new List<Thing>();
        private static HashSet<Thing> processedThings = new HashSet<Thing>();
        private static List<Thing> newRelevantThings = new List<Thing>();
        private static List<Thing> tmpMedicine = new List<Thing>();
        private static WorkGiver_DoBillMagicBench.DefCountList availableCounts = new WorkGiver_DoBillMagicBench.DefCountList();
        private List<ThingCount> chosenIngThings = new List<ThingCount>();

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.InteractionCell;
            }
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Some;
        }

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                if (this.def.fixedBillGiverDefs != null && this.def.fixedBillGiverDefs.Count == 1)
                    return ThingRequest.ForDef(this.def.fixedBillGiverDefs[0]);
                return ThingRequest.ForGroup(ThingRequestGroup.PotentialBillGiver);
            }
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            bool shouldSkip = this.ShouldSkipCore(pawn, forced);

            if (!shouldSkip)
            {
                if (!MagicUtility.IsMagicUser(pawn))
                {
                    shouldSkip = true;
                }
                else if (pawn.needs.TryGetNeed<Need_MagicPool>() != null & pawn.needs.TryGetNeed<Need_MagicPool>().CurLevel < .01)
                {
                    shouldSkip = true;
                }
            }

            return shouldSkip;
        }

        protected bool ShouldSkipCore(Pawn pawn, bool forced)
        {
            List<Thing> thingList = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.PotentialBillGiver);
            for (int index = 0; index < thingList.Count; ++index)
            {
                if (thingList[index] is IBillGiver billGiver && this.ThingIsUsableBillGiver(thingList[index]) && billGiver.BillStack.AnyShouldDoNow)
                    return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            IBillGiver giver = thing as IBillGiver;
            if (giver == null || !this.ThingIsUsableBillGiver(thing) || (!giver.BillStack.AnyShouldDoNow || !giver.UsableForBillsAfterFueling()) || (!pawn.CanReserve((LocalTargetInfo)thing, 1, -1, (ReservationLayerDef)null, forced) || thing.IsBurning() || thing.IsForbidden(pawn)))
                return (Job)null;
            CompRefuelable comp = thing.TryGetComp<CompRefuelable>();
            if (comp != null && !comp.HasFuel)
            {
                if (!RefuelWorkGiverUtility.CanRefuel(pawn, thing, forced))
                    return (Job)null;
                return RefuelWorkGiverUtility.RefuelJob(pawn, thing, forced, (JobDef)null, (JobDef)null);
            }
            giver.BillStack.RemoveIncompletableBills();
            return this.StartOrResumeBillJob(pawn, giver);
        }

        private static UnfinishedThing ClosestUnfinishedThingForBill(
          Pawn pawn,
          Bill_ProductionWithUft bill)
        {
            Predicate<Thing> validator = (Predicate<Thing>)(t =>
            {
                if (!t.IsForbidden(pawn) && ((UnfinishedThing)t).Recipe == bill.recipe && ((UnfinishedThing)t).Creator == pawn && ((UnfinishedThing)t).ingredients.TrueForAll((Predicate<Thing>)(x => bill.IsFixedOrAllowedIngredient(x.def))))
                    return pawn.CanReserve((LocalTargetInfo)t, 1, -1, (ReservationLayerDef)null, false);
                return false;
            });
            return (UnfinishedThing)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(bill.recipe.unfinishedThingDef), PathEndMode.InteractionCell, TraverseParms.For(pawn, pawn.NormalMaxDanger(), TraverseMode.ByPawn, false), 9999f, validator, (IEnumerable<Thing>)null, 0, -1, false, RegionType.Set_Passable, false);
        }

        private static Job FinishUftJob(Pawn pawn, UnfinishedThing uft, Bill_ProductionWithUft bill)
        {
            if (uft.Creator != pawn)
            {
                Log.Error("Tried to get FinishUftJob for " + (object)pawn + " finishing " + (object)uft + " but its creator is " + (object)uft.Creator, false);
                return (Job)null;
            }
            else if (pawn.needs.TryGetNeed<Need_MagicPool>() != null & pawn.needs.TryGetNeed<Need_MagicPool>().CurLevel < .05)
            {
                return (Job)null;
            }
            Job job1 = WorkGiverUtility.HaulStuffOffBillGiverJob(pawn, bill.billStack.billGiver, (Thing)uft);
            if (job1 != null && job1.targetA.Thing != uft)
                return job1;
            Job job2 = JobMaker.MakeJob(RH_TET_MagicDefOf.RH_TET_Magic_DoBillMagicBench, (LocalTargetInfo)((Thing)bill.billStack.billGiver));
            job2.bill = (Bill)bill;
            job2.targetQueueB = new List<LocalTargetInfo>()
              {
                (LocalTargetInfo) ((Thing) uft)
              };
            job2.countQueue = new List<int>() { 1 };
            job2.haulMode = HaulMode.ToCellNonStorage;
            return job2;
        }

        private Job StartOrResumeBillJob(Pawn pawn, IBillGiver giver)
        {
            bool flag1 = FloatMenuMakerMap.makingFor == pawn;
            for (int index = 0; index < giver.BillStack.Count; ++index)
            {
                Bill bill1 = giver.BillStack[index];
                if ((bill1.recipe.requiredGiverWorkType == null || bill1.recipe.requiredGiverWorkType == this.def.workType) && (Find.TickManager.TicksGame > bill1.nextTickToSearchForIngredients || FloatMenuMakerMap.makingFor == pawn) && (bill1.ShouldDoNow() && bill1.PawnAllowedToStartAnew(pawn)))
                {
                    SkillRequirement skillRequirement = bill1.recipe.FirstSkillRequirementPawnDoesntSatisfy(pawn);
                    if (skillRequirement != null)
                    {
                        JobFailReason.Is((string)"UnderRequiredSkill".Translate((NamedArgument)skillRequirement.minLevel), bill1.Label);
                    }
                    else
                    {
                            if (bill1 is Bill_ProductionWithUft bill)
                            {
                                if (bill.BoundUft != null)
                                {
                                    if (bill.BoundWorker == pawn && pawn.CanReserveAndReach((LocalTargetInfo)(Thing)bill.BoundUft, PathEndMode.Touch, Danger.Deadly, 1, -1, (ReservationLayerDef)null, false) && !bill.BoundUft.IsForbidden(pawn))
                                        return WorkGiver_DoBillMagicBench.FinishUftJob(pawn, bill.BoundUft, bill);
                                    continue;
                                }
                                UnfinishedThing uft = WorkGiver_DoBillMagicBench.ClosestUnfinishedThingForBill(pawn, bill);
                                if (uft != null)
                                    return WorkGiver_DoBillMagicBench.FinishUftJob(pawn, uft, bill);
                            }
                            List<IngredientCount> ingredientCountList = (List<IngredientCount>)null;
                            if (flag1)
                            {
                                ingredientCountList = WorkGiver_DoBillMagicBench.missingIngredients;
                                ingredientCountList.Clear();
                                WorkGiver_DoBillMagicBench.tmpMissingUniqueIngredients.Clear();
                            }
                            bool? nullable;
                            if (bill1 is Bill_Medical billMedical)
                            {
                                List<Thing> requiredIngredients = billMedical.uniqueRequiredIngredients;
                                nullable = requiredIngredients != null ? new bool?(requiredIngredients.NullOrEmpty<Thing>()) : new bool?();
                                bool flag2 = false;
                                if (nullable.GetValueOrDefault() == flag2 & nullable.HasValue)
                                {
                                    foreach (Thing requiredIngredient in billMedical.uniqueRequiredIngredients)
                                    {
                                        if (requiredIngredient.IsForbidden(pawn) || !pawn.CanReserveAndReach((LocalTargetInfo)requiredIngredient, PathEndMode.OnCell, Danger.Deadly, 1, -1, (ReservationLayerDef)null, false))
                                        WorkGiver_DoBillMagicBench.tmpMissingUniqueIngredients.Add(requiredIngredient);
                                    }
                                }
                            }
                            if (!WorkGiver_DoBillMagicBench.TryFindBestBillIngredients(bill1, pawn, (Thing)giver, this.chosenIngThings, ingredientCountList) || !WorkGiver_DoBillMagicBench.tmpMissingUniqueIngredients.NullOrEmpty<Thing>())
                            {
                                if (FloatMenuMakerMap.makingFor != pawn)
                                    bill1.nextTickToSearchForIngredients = Find.TickManager.TicksGame + WorkGiver_DoBillMagicBench.ReCheckFailedBillTicksRange.RandomInRange;
                                else if (flag1)
                                {
                                    JobFailReason.Is((string)"MissingMaterials".Translate((NamedArgument)ingredientCountList.Select<IngredientCount, string>((Func<IngredientCount, string>)(missing => missing.Summary)).Concat<string>(WorkGiver_DoBillMagicBench.tmpMissingUniqueIngredients.Select<Thing, string>((Func<Thing, string>)(t => t.Label))).ToCommaList(false, false)), bill1.Label);
                                    flag1 = false;
                                }
                                this.chosenIngThings.Clear();
                            }
                            else
                            {
                                Job job = WorkGiver_DoBillMagicBench.TryStartNewDoBillJob(pawn, bill1, giver, this.chosenIngThings, out Job _, true);
                                this.chosenIngThings.Clear();
                                return job;
                            }
                        }
                }
            }
            this.chosenIngThings.Clear();
            return (Job)null;
        }

        public static Job TryStartNewDoBillJob(
          Pawn pawn,
          Bill bill,
          IBillGiver giver,
          List<ThingCount> chosenIngThings,
          out Job haulOffJob,
          bool dontCreateJobIfHaulOffRequired = true)
        {
            haulOffJob = WorkGiverUtility.HaulStuffOffBillGiverJob(pawn, giver, (Thing)null);
            if (haulOffJob != null & dontCreateJobIfHaulOffRequired)
                return haulOffJob;
            Job job = JobMaker.MakeJob(JobDefOf.DoBill, (LocalTargetInfo)(Thing)giver);
            job.targetQueueB = new List<LocalTargetInfo>(chosenIngThings.Count);
            job.countQueue = new List<int>(chosenIngThings.Count);
            for (int index = 0; index < chosenIngThings.Count; ++index)
            {
                job.targetQueueB.Add((LocalTargetInfo)chosenIngThings[index].Thing);
                job.countQueue.Add(chosenIngThings[index].Count);
            }
            if (bill.xenogerm != null)
            {
                job.targetQueueB.Add((LocalTargetInfo)(Thing)bill.xenogerm);
                job.countQueue.Add(1);
            }
            job.haulMode = HaulMode.ToCellNonStorage;
            job.bill = bill;
            return job;
        }

        public bool ThingIsUsableBillGiver(Thing thing)
        {
            Pawn pawn1 = thing as Pawn;
            Corpse corpse = thing as Corpse;
            Pawn pawn2 = (Pawn)null;
            if (corpse != null)
                pawn2 = corpse.InnerPawn;
            return this.def.fixedBillGiverDefs != null && this.def.fixedBillGiverDefs.Contains(thing.def) || pawn1 != null && (this.def.billGiversAllHumanlikes && pawn1.RaceProps.Humanlike || this.def.billGiversAllMechanoids && pawn1.RaceProps.IsMechanoid || this.def.billGiversAllAnimals && pawn1.RaceProps.Animal) || corpse != null && pawn2 != null && (this.def.billGiversAllHumanlikesCorpses && pawn2.RaceProps.Humanlike || this.def.billGiversAllMechanoidsCorpses && pawn2.RaceProps.IsMechanoid || this.def.billGiversAllAnimalsCorpses && pawn2.RaceProps.Animal);
        }

        private static bool IsUsableIngredient(Thing t, Bill bill)
        {
            if (!bill.IsFixedOrAllowedIngredient(t))
                return false;
            foreach (IngredientCount ingredient in bill.recipe.ingredients)
            {
                if (ingredient.filter.Allows(t))
                    return true;
            }
            return false;
        }

        public static bool TryFindBestFixedIngredients(
          List<IngredientCount> ingredients,
          Pawn pawn,
          Thing ingredientDestination,
          List<ThingCount> chosen,
          float searchRadius = 999f)
        {
            return WorkGiver_DoBillMagicBench.TryFindBestIngredientsHelper((Predicate<Thing>)(t =>
            {
                foreach (IngredientCount ingredient in ingredients)
                {
                    if (ingredient.filter.Allows(t))
                        return true;
                }
                return false;
            }), (Predicate<List<Thing>>)(foundThings => WorkGiver_DoBillMagicBench.TryFindBestIngredientsInSet_NoMixHelper(foundThings, ingredients, chosen, WorkGiver_DoBillMagicBench.GetBillGiverRootCell(ingredientDestination, pawn), false, (List<IngredientCount>)null, (Bill)null)), ingredients, pawn, ingredientDestination, chosen, searchRadius);
        }

        private static bool TryFindBestBillIngredients(
          Bill bill,
          Pawn pawn,
          Thing billGiver,
          List<ThingCount> chosen,
          List<IngredientCount> missingIngredients)
        {
            return WorkGiver_DoBillMagicBench.TryFindBestIngredientsHelper((Predicate<Thing>)(t => WorkGiver_DoBillMagicBench.IsUsableIngredient(t, bill)), (Predicate<List<Thing>>)(foundThings => WorkGiver_DoBillMagicBench.TryFindBestBillIngredientsInSet(foundThings, bill, chosen, WorkGiver_DoBillMagicBench.GetBillGiverRootCell(billGiver, pawn), billGiver is Pawn, missingIngredients)), bill.recipe.ingredients, pawn, billGiver, chosen, bill.ingredientSearchRadius);
        }

        private static bool TryFindBestIngredientsHelper(
          Predicate<Thing> thingValidator,
          Predicate<List<Thing>> foundAllIngredientsAndChoose,
          List<IngredientCount> ingredients,
          Pawn pawn,
          Thing billGiver,
          List<ThingCount> chosen,
          float searchRadius)
        {
            chosen.Clear();
            WorkGiver_DoBillMagicBench.newRelevantThings.Clear();
            if (ingredients.Count == 0)
                return true;
            Region rootReg = WorkGiver_DoBillMagicBench.GetBillGiverRootCell(billGiver, pawn).GetRegion(pawn.Map, RegionType.Set_Passable);
            if (rootReg == null)
                return false;
            WorkGiver_DoBillMagicBench.relevantThings.Clear();
            WorkGiver_DoBillMagicBench.processedThings.Clear();
            bool foundAll = false;
            float radiusSq = searchRadius * searchRadius;
            Predicate<Thing> baseValidator = (Predicate<Thing>)(t => t.Spawned && thingValidator(t) && ((double)(t.Position - billGiver.Position).LengthHorizontalSquared < (double)radiusSq && !t.IsForbidden(pawn)) && pawn.CanReserve((LocalTargetInfo)t, 1, -1, (ReservationLayerDef)null, false));
            bool billGiverIsPawn = billGiver is Pawn;
            if (billGiver is Building_MechGestator buildingMechGestator)
            {
                WorkGiver_DoBillMagicBench.relevantThings.AddRange((IEnumerable<Thing>)buildingMechGestator.innerContainer);
                if (foundAllIngredientsAndChoose(WorkGiver_DoBillMagicBench.relevantThings))
                {
                    WorkGiver_DoBillMagicBench.relevantThings.Clear();
                    return true;
                }
            }
            TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false);
            RegionEntryPredicate entryCondition = (RegionEntryPredicate)null;
            entryCondition = (double)Math.Abs(999f - searchRadius) < 1.0 ? (RegionEntryPredicate)((from, r) => r.Allows(traverseParams, false)) : (RegionEntryPredicate)((from, r) =>
            {
                if (!r.Allows(traverseParams, false))
                    return false;
                CellRect extentsClose = r.extentsClose;
                int num1 = Math.Abs(billGiver.Position.x - Math.Max(extentsClose.minX, Math.Min(billGiver.Position.x, extentsClose.maxX)));
                if ((double)num1 > (double)searchRadius)
                    return false;
                int num2 = Math.Abs(billGiver.Position.z - Math.Max(extentsClose.minZ, Math.Min(billGiver.Position.z, extentsClose.maxZ)));
                return (double)num2 <= (double)searchRadius && (double)(num1 * num1 + num2 * num2) <= (double)radiusSq;
            });
            int adjacentRegionsAvailable = rootReg.Neighbors.Count<Region>((Func<Region, bool>)(region => entryCondition(rootReg, region)));
            int regionsProcessed = 0;
            WorkGiver_DoBillMagicBench.processedThings.AddRange<Thing>(WorkGiver_DoBillMagicBench.relevantThings);
            int num = foundAllIngredientsAndChoose(WorkGiver_DoBillMagicBench.relevantThings) ? 1 : 0;
            RegionProcessor regionProcessor = (RegionProcessor)(r =>
            {
                List<Thing> thingList = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
                for (int index = 0; index < thingList.Count; ++index)
                {
                    Thing thing = thingList[index];
                    if (!WorkGiver_DoBillMagicBench.processedThings.Contains(thing) && ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, r, PathEndMode.ClosestTouch, pawn) && (baseValidator(thing) && !(thing.def.IsMedicine & billGiverIsPawn)))
                    {
                        WorkGiver_DoBillMagicBench.newRelevantThings.Add(thing);
                        WorkGiver_DoBillMagicBench.processedThings.Add(thing);
                    }
                }
                ++regionsProcessed;
                if (WorkGiver_DoBillMagicBench.newRelevantThings.Count > 0 && regionsProcessed > adjacentRegionsAvailable)
                {
                    WorkGiver_DoBillMagicBench.relevantThings.AddRange((IEnumerable<Thing>)WorkGiver_DoBillMagicBench.newRelevantThings);
                    WorkGiver_DoBillMagicBench.newRelevantThings.Clear();
                    if (foundAllIngredientsAndChoose(WorkGiver_DoBillMagicBench.relevantThings))
                    {
                        foundAll = true;
                        return true;
                    }
                }
                return false;
            });
            RegionTraverser.BreadthFirstTraverse(rootReg, entryCondition, regionProcessor, 99999, RegionType.Set_Passable);
            WorkGiver_DoBillMagicBench.relevantThings.Clear();
            WorkGiver_DoBillMagicBench.newRelevantThings.Clear();
            WorkGiver_DoBillMagicBench.processedThings.Clear();
            return foundAll;
        }

        private static IntVec3 GetBillGiverRootCell(Thing billGiver, Pawn forPawn)
        {
            Building building = billGiver as Building;
            if (building == null)
                return billGiver.Position;
            if (building.def.hasInteractionCell)
                return building.InteractionCell;
            Log.Error("Tried to find bill ingredients for " + (object)billGiver + " which has no interaction cell.", false);
            return forPawn.Position;
        }

        private static void MakeIngredientsListInProcessingOrder(
          List<IngredientCount> ingredientsOrdered,
          Bill bill)
        {
            ingredientsOrdered.Clear();
            if (bill.recipe.productHasIngredientStuff)
                ingredientsOrdered.Add(bill.recipe.ingredients[0]);
            for (int index = 0; index < bill.recipe.ingredients.Count; ++index)
            {
                if (!bill.recipe.productHasIngredientStuff || index != 0)
                {
                    IngredientCount ingredient = bill.recipe.ingredients[index];
                    if (ingredient.IsFixedIngredient)
                        ingredientsOrdered.Add(ingredient);
                }
            }
            for (int index = 0; index < bill.recipe.ingredients.Count; ++index)
            {
                IngredientCount ingredient = bill.recipe.ingredients[index];
                if (!ingredientsOrdered.Contains(ingredient))
                    ingredientsOrdered.Add(ingredient);
            }
        }

        private static bool TryFindBestBillIngredientsInSet(
          List<Thing> availableThings,
          Bill bill,
          List<ThingCount> chosen,
          IntVec3 rootCell,
          bool alreadySorted,
          List<IngredientCount> missingIngredients)
        {
            if (bill.recipe.allowMixingIngredients)
                return WorkGiver_DoBillMagicBench.TryFindBestBillIngredientsInSet_AllowMix(availableThings, bill, chosen, rootCell, missingIngredients);
            return WorkGiver_DoBillMagicBench.TryFindBestBillIngredientsInSet_NoMix(availableThings, bill, chosen, rootCell, alreadySorted, missingIngredients);
        }

        private static bool TryFindBestBillIngredientsInSet_NoMix(
          List<Thing> availableThings,
          Bill bill,
          List<ThingCount> chosen,
          IntVec3 rootCell,
          bool alreadySorted,
          List<IngredientCount> missingIngredients)
        {
            return WorkGiver_DoBillMagicBench.TryFindBestIngredientsInSet_NoMixHelper(availableThings, bill.recipe.ingredients, chosen, rootCell, alreadySorted, missingIngredients, bill);
        }

        private static bool TryFindBestIngredientsInSet_NoMixHelper(
          List<Thing> availableThings,
          List<IngredientCount> ingredients,
          List<ThingCount> chosen,
          IntVec3 rootCell,
          bool alreadySorted,
          List<IngredientCount> missingIngredients,
          Bill bill = null)
        {
            if (!alreadySorted)
            {
                Comparison<Thing> comparison = (Comparison<Thing>)((t1, t2) => ((float)(t1.PositionHeld - rootCell).LengthHorizontalSquared).CompareTo((float)(t2.PositionHeld - rootCell).LengthHorizontalSquared));
                availableThings.Sort(comparison);
            }
            chosen.Clear();
            WorkGiver_DoBillMagicBench.availableCounts.Clear();
            missingIngredients?.Clear();
            WorkGiver_DoBillMagicBench.availableCounts.GenerateFrom(availableThings);
            for (int index1 = 0; index1 < ingredients.Count; ++index1)
            {
                IngredientCount ingredient = ingredients[index1];
                bool flag = false;
                for (int index2 = 0; index2 < WorkGiver_DoBillMagicBench.availableCounts.Count; ++index2)
                {
                    float f = bill != null ? (float)ingredient.CountRequiredOfFor(WorkGiver_DoBillMagicBench.availableCounts.GetDef(index2), bill.recipe, bill) : ingredient.GetBaseCount();
                    if ((bill == null || bill.recipe.ignoreIngredientCountTakeEntireStacks || (double)f <= (double)WorkGiver_DoBillMagicBench.availableCounts.GetCount(index2)) && ingredient.filter.Allows(WorkGiver_DoBillMagicBench.availableCounts.GetDef(index2)) && (bill == null || ingredient.IsFixedIngredient || bill.ingredientFilter.Allows(WorkGiver_DoBillMagicBench.availableCounts.GetDef(index2))))
                    {
                        for (int index3 = 0; index3 < availableThings.Count; ++index3)
                        {
                            if (availableThings[index3].def == WorkGiver_DoBillMagicBench.availableCounts.GetDef(index2))
                            {
                                int num = availableThings[index3].stackCount - ThingCountUtility.CountOf(chosen, availableThings[index3]);
                                if (num > 0)
                                {
                                    if (bill != null && bill.recipe.ignoreIngredientCountTakeEntireStacks)
                                    {
                                        ThingCountUtility.AddToList(chosen, availableThings[index3], num);
                                        return true;
                                    }
                                    int countToAdd = Mathf.Min(Mathf.FloorToInt(f), num);
                                    ThingCountUtility.AddToList(chosen, availableThings[index3], countToAdd);
                                    f -= (float)countToAdd;
                                    if ((double)f < 1.0 / 1000.0)
                                    {
                                        flag = true;
                                        float val = WorkGiver_DoBillMagicBench.availableCounts.GetCount(index2) - f;
                                        WorkGiver_DoBillMagicBench.availableCounts.SetCount(index2, val);
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag)
                            break;
                    }
                }
                if (!flag)
                {
                    if (missingIngredients == null)
                        return false;
                    missingIngredients.Add(ingredient);
                }
            }
            return missingIngredients == null || missingIngredients.Count == 0;
        }

        private static bool TryFindBestBillIngredientsInSet_AllowMix(
          List<Thing> availableThings,
          Bill bill,
          List<ThingCount> chosen,
          IntVec3 rootCell,
          List<IngredientCount> missingIngredients)
        {
            chosen.Clear();
            missingIngredients?.Clear();
            availableThings.SortBy<Thing, float, int>((Func<Thing, float>)(t => bill.recipe.IngredientValueGetter.ValuePerUnitOf(t.def)), (Func<Thing, int>)(t => (t.Position - rootCell).LengthHorizontalSquared));
            for (int index1 = 0; index1 < bill.recipe.ingredients.Count; ++index1)
            {
                IngredientCount ingredient = bill.recipe.ingredients[index1];
                float baseCount = ingredient.GetBaseCount();
                for (int index2 = 0; index2 < availableThings.Count; ++index2)
                {
                    Thing availableThing = availableThings[index2];
                    if (ingredient.filter.Allows(availableThing) && (ingredient.IsFixedIngredient || bill.ingredientFilter.Allows(availableThing)))
                    {
                        float num = bill.recipe.IngredientValueGetter.ValuePerUnitOf(availableThing.def);
                        int countToAdd = Mathf.Min(Mathf.CeilToInt(baseCount / num), availableThing.stackCount);
                        ThingCountUtility.AddToList(chosen, availableThing, countToAdd);
                        baseCount -= (float)countToAdd * num;
                        if ((double)baseCount <= 9.99999974737875E-05)
                            break;
                    }
                }
                if ((double)baseCount > 9.99999974737875E-05)
                {
                    if (missingIngredients == null)
                        return false;
                    missingIngredients.Add(ingredient);
                }
            }
            return missingIngredients == null || missingIngredients.Count == 0;
        }

        private class DefCountList
        {
            private List<ThingDef> defs = new List<ThingDef>();
            private List<float> counts = new List<float>();

            public int Count
            {
                get
                {
                    return this.defs.Count;
                }
            }

            public float this[ThingDef def]
            {
                get
                {
                    int index = this.defs.IndexOf(def);
                    if (index < 0)
                        return 0.0f;
                    return this.counts[index];
                }
                set
                {
                    int index = this.defs.IndexOf(def);
                    if (index < 0)
                    {
                        this.defs.Add(def);
                        this.counts.Add(value);
                        index = this.defs.Count - 1;
                    }
                    else
                        this.counts[index] = value;
                    this.CheckRemove(index);
                }
            }

            public float GetCount(int index)
            {
                return this.counts[index];
            }

            public void SetCount(int index, float val)
            {
                this.counts[index] = val;
                this.CheckRemove(index);
            }

            public ThingDef GetDef(int index)
            {
                return this.defs[index];
            }

            private void CheckRemove(int index)
            {
                if ((double)this.counts[index] != 0.0)
                    return;
                this.counts.RemoveAt(index);
                this.defs.RemoveAt(index);
            }

            public void Clear()
            {
                this.defs.Clear();
                this.counts.Clear();
            }

            public void GenerateFrom(List<Thing> things)
            {
                this.Clear();
                for (int index = 0; index < things.Count; ++index)
                    this[things[index].def] += (float)things[index].stackCount;
            }
        }
    }
}
