using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public static class Toils_RecipeIdol
    {
        private const int LongCraftingProjectThreshold = 10000;

        public static Toil MakeUnfinishedThingIfNeeded()
        {
            Toil toil = new Toil();
            toil.initAction = (Action)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                if (!curJob.RecipeDef.UsesUnfinishedThing || curJob.GetTarget(TargetIndex.B).Thing is UnfinishedThing)
                    return;
                List<Thing> ingredients = Toils_RecipeIdol.CalculateIngredients(curJob, actor);
                Thing dominantIngredient = Toils_RecipeIdol.CalculateDominantIngredient(curJob, ingredients);
                for (int index = 0; index < ingredients.Count; ++index)
                {
                    Thing t = ingredients[index];
                    actor.Map.designationManager.RemoveAllDesignationsOn(t, false);
                    if (t.Spawned)
                        t.DeSpawn(DestroyMode.Vanish);
                }
                ThingDef stuff = curJob.RecipeDef.unfinishedThingDef.MadeFromStuff ? dominantIngredient.def : (ThingDef)null;
                UnfinishedThing thing = (UnfinishedThing)ThingMaker.MakeThing(curJob.RecipeDef.unfinishedThingDef, stuff);
                thing.Creator = actor;
                thing.BoundBill = (Bill_ProductionWithUft)curJob.bill;
                thing.ingredients = ingredients;
                GenSpawn.Spawn((Thing)thing, curJob.GetTarget(TargetIndex.A).Cell, actor.Map, WipeMode.Vanish);
                curJob.SetTarget(TargetIndex.B, (LocalTargetInfo)((Thing)thing));
                actor.Reserve((LocalTargetInfo)((Thing)thing), curJob, 1, -1, (ReservationLayerDef)null, true);
            });
            return toil;
        }

        public static Toil DoRecipeWork()
        {
            Toil toil = new Toil();
            toil.initAction = (Action)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                JobDriver_DoBillIdol curDriver = (JobDriver_DoBillIdol)actor.jobs.curDriver;
                UnfinishedThing thing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
                if (thing != null && thing.Initialized)
                {
                    curDriver.workLeft = thing.workLeft;
                }
                else
                {
                    curDriver.workLeft = curJob.bill.GetWorkAmount(thing);
                    if (thing != null)
                        thing.workLeft = curDriver.workLeft;
                }
                curDriver.billStartTick = Find.TickManager.TicksGame;
                curDriver.ticksSpentDoingRecipeWork = 0;
                curJob.bill.Notify_DoBillStarted(actor);
            });
            toil.tickAction = (Action)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                JobDriver_DoBillIdol curDriver = (JobDriver_DoBillIdol)actor.jobs.curDriver;
                UnfinishedThing thing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
                if (thing != null && thing.Destroyed)
                {
                    actor.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
                }
                else if (actor.needs.TryGetNeed<Need_FaithPool>() != null & actor.needs.TryGetNeed<Need_FaithPool>().CurLevel < .01)
                {
                    Messages.Message((string)"RH_TET_Magic_NoFaithPower".Translate(actor.Name), MessageTypeDefOf.NeutralEvent, true);
                    actor.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
                }
                else
                {
                    actor.needs.TryGetNeed<Need_FaithPool>().CurLevel -= .0001f;
                    ++curDriver.ticksSpentDoingRecipeWork;
                    curJob.bill.Notify_PawnDidWork(actor);
                    (toil.actor.CurJob.GetTarget(TargetIndex.A).Thing as IBillGiverWithTickAction)?.UsedThisTick();
                    if (curJob.RecipeDef.workSkill != null && curJob.RecipeDef.UsesUnfinishedThing)
                        actor.skills.Learn(curJob.RecipeDef.workSkill, 0.1f * curJob.RecipeDef.workSkillLearnFactor, false);
                    float num1 = curJob.RecipeDef.workSpeedStat == null ? 1f : actor.GetStatValue(curJob.RecipeDef.workSpeedStat, true);
                    if (curJob.RecipeDef.workTableSpeedStat != null)
                    {
                        Building_WorkTable billGiver = curDriver.BillGiver as Building_WorkTable;
                        if (billGiver != null)
                            num1 *= billGiver.GetStatValue(curJob.RecipeDef.workTableSpeedStat, true);
                    }
                    if (DebugSettings.fastCrafting)
                        num1 *= 30f;
                    curDriver.workLeft -= num1;

                    if (thing != null)
                        thing.workLeft = curDriver.workLeft;
                    actor.GainComfortFromCellIfPossible(1, true);
                    if ((double)curDriver.workLeft <= 0.0)
                    {
                        curDriver.ReadyForNextToil();
                    }
                    else
                    {
                        if (!curJob.bill.recipe.UsesUnfinishedThing)
                            return;
                        int num2 = Find.TickManager.TicksGame - curDriver.billStartTick;
                        if (num2 < 3000 || num2 % 1000 != 0)
                            return;
                        actor.jobs.CheckForJobOverride();
                    }
                }
            });
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.WithEffect((Func<EffecterDef>)(() => toil.actor.CurJob.bill.recipe.effectWorking), TargetIndex.A);
            toil.PlaySustainerOrSound((Func<SoundDef>)(() => toil.actor.CurJob.bill.recipe.soundWorking));
            toil.WithProgressBar(TargetIndex.A, (Func<float>)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.CurJob;
                UnfinishedThing thing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
                return (float)(1.0 - (double)((JobDriver_DoBillIdol)actor.jobs.curDriver).workLeft / (double)curJob.bill.recipe.WorkAmountTotal(thing));
            }), false, -0.5f);
            toil.FailOn<Toil>((Func<bool>)(() =>
            {
                RecipeDef recipeDef = toil.actor.CurJob.RecipeDef;
                if (recipeDef != null && recipeDef.interruptIfIngredientIsRotting)
                {
                    LocalTargetInfo target = toil.actor.CurJob.GetTarget(TargetIndex.B);
                    if (target.HasThing && target.Thing.GetRotStage() > RotStage.Fresh)
                        return true;
                }
                return toil.actor.CurJob.bill.suspended;
            }));
            toil.activeSkill = (Func<SkillDef>)(() => toil.actor.CurJob.bill.recipe.workSkill);
            return toil;
        }

        public static Toil FinishRecipeAndStartStoringProduct()
        {
            Toil toil = new Toil();
            toil.initAction = (Action)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                JobDriver_DoBillIdol curDriver = (JobDriver_DoBillIdol)actor.jobs.curDriver;
                if (curJob.RecipeDef.workSkill != null && !curJob.RecipeDef.UsesUnfinishedThing)
                {
                    float xp = (float)curDriver.ticksSpentDoingRecipeWork * 0.1f * curJob.RecipeDef.workSkillLearnFactor;
                    actor.skills.GetSkill(curJob.RecipeDef.workSkill).Learn(xp, false);
                }
                List<Thing> ingredients = Toils_RecipeIdol.CalculateIngredients(curJob, actor);
                Thing dominantIngredient = Toils_RecipeIdol.CalculateDominantIngredient(curJob, ingredients);
                List<Thing> list = GenRecipe.MakeRecipeProducts(curJob.RecipeDef, actor, ingredients, dominantIngredient, curDriver.BillGiver).ToList<Thing>();
                Toils_RecipeIdol.ConsumeIngredients(ingredients, curJob.RecipeDef, actor.Map);
                curJob.bill.Notify_IterationCompleted(actor, ingredients);
                RecordsUtility.Notify_BillDone(actor, list);
                UnfinishedThing thing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
                if ((double)curJob.bill.recipe.WorkAmountTotal(thing) >= 10000.0 && list.Count > 0)
                    TaleRecorder.RecordTale(TaleDefOf.CompletedLongCraftingProject, (object)actor, (object)list[0].GetInnerIfMinified().def);
                if (list.Any<Thing>())
                    Find.QuestManager.Notify_ThingsProduced(actor, list);
                if (list.Count == 0)
                    actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
                else if (curJob.bill.GetStoreMode() == BillStoreModeDefOf.DropOnFloor)
                {
                    for (int index = 0; index < list.Count; ++index)
                    {
                        if (!GenPlace.TryPlaceThing(list[index], actor.Position, actor.Map, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4()))
                            Log.Error(actor.ToString() + " could not drop recipe product " + (object)list[index] + " near " + (object)actor.Position);
                    }
                    actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
                }
                else
                {
                    if (list.Count > 1)
                    {
                        for (int index = 1; index < list.Count; ++index)
                        {
                            if (!GenPlace.TryPlaceThing(list[index], actor.Position, actor.Map, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4()))
                                Log.Error(actor.ToString() + " could not drop recipe product " + (object)list[index] + " near " + (object)actor.Position);
                        }
                    }
                    IntVec3 foundCell = IntVec3.Invalid;
                    if (curJob.bill.GetStoreMode() == BillStoreModeDefOf.BestStockpile)
                        StoreUtility.TryFindBestBetterStoreCellFor(list[0], actor, actor.Map, StoragePriority.Unstored, actor.Faction, out foundCell, true);
                    else if (curJob.bill.GetStoreMode() == BillStoreModeDefOf.SpecificStockpile)
                        StoreUtility.TryFindBestBetterStoreCellForIn(list[0], actor, actor.Map, StoragePriority.Unstored, actor.Faction, curJob.bill.GetSlotGroup(), out foundCell, true);
                    else
                        Log.ErrorOnce("Unknown store mode", 9158246);
                    if (foundCell.IsValid)
                    {
                        actor.carryTracker.TryStartCarry(list[0]);
                        curJob.targetB = (LocalTargetInfo)foundCell;
                        curJob.targetA = (LocalTargetInfo)list[0];
                        curJob.count = 99999;
                    }
                    else
                    {
                        if (!GenPlace.TryPlaceThing(list[0], actor.Position, actor.Map, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4()))
                            Log.Error("Bill doer could not drop product " + (object)list[0] + " near " + (object)actor.Position);
                        actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
                    }
                }
            });
            return toil;
        }

        private static List<Thing> CalculateIngredients(Job job, Pawn actor)
        {
            UnfinishedThing thing1 = job.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
            if (thing1 != null)
            {
                List<Thing> ingredients = thing1.ingredients;
                job.RecipeDef.Worker.ConsumeIngredient((Thing)thing1, job.RecipeDef, actor.Map);
                job.placedThings = (List<ThingCountClass>)null;
                return ingredients;
            }
            List<Thing> thingList = new List<Thing>();

            if (job.placedThings != null)
            {
                for (int index = 0; index < job.placedThings.Count; ++index)
                {
                    if (job.placedThings[index].Count <= 0)
                    {
                        Log.Error("PlacedThing " + (object)job.placedThings[index] + " with count " + (object)job.placedThings[index].Count + " for job " + (object)job);
                    }
                    else
                    {
                        Thing thing2 = job.placedThings[index].Count >= job.placedThings[index].thing.stackCount ? job.placedThings[index].thing : job.placedThings[index].thing.SplitOff(job.placedThings[index].Count);
                        job.placedThings[index].Count = 0;
                        if (thingList.Contains(thing2))
                        {
                            Log.Error("Tried to add ingredient from job placed targets twice: " + (object)thing2);
                        }
                        else
                        {
                            thingList.Add(thing2);
                            if (job.RecipeDef.autoStripCorpses)
                                (thing2 as IStrippable)?.Strip();
                        }
                    }
                }
            }
            job.placedThings = (List<ThingCountClass>)null;
            return thingList;
        }

        private static Thing CalculateDominantIngredient(Job job, List<Thing> ingredients)
        {
            UnfinishedThing uft = job.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
            if (uft != null && uft.def.MadeFromStuff)
                return uft.ingredients.First<Thing>((Func<Thing, bool>)(ing => ing.def == uft.Stuff));
            if (ingredients.NullOrEmpty<Thing>())
                return (Thing)null;
            if (job.RecipeDef.productHasIngredientStuff)
                return ingredients[0];
            if (job.RecipeDef.products.Any<ThingDefCountClass>((Predicate<ThingDefCountClass>)(x => x.thingDef.MadeFromStuff)))
                return ingredients.Where<Thing>((Func<Thing, bool>)(x => x.def.IsStuff)).RandomElementByWeight<Thing>((Func<Thing, float>)(x => (float)x.stackCount));
            return ingredients.RandomElementByWeight<Thing>((Func<Thing, float>)(x => (float)x.stackCount));
        }

        private static void ConsumeIngredients(List<Thing> ingredients, RecipeDef recipe, Map map)
        {
            for (int index = 0; index < ingredients.Count; ++index)
            {
                recipe.Worker.ConsumeIngredient(ingredients[index], recipe, map);
            }
        }

        public static Toil PlaceHauledThingInCell(
          TargetIndex cellInd,
          Toil nextToilOnPlaceFailOrIncomplete,
          bool storageMode,
          bool tryStoreInSameStorageIfSpotCantHoldWholeStack = false)
        {
            Toil toil = new Toil();
            toil.initAction = (Action)(() =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                IntVec3 cell = curJob.GetTarget(cellInd).Cell;
                if (actor.carryTracker.CarriedThing == null)
                {
                    Log.Error(actor.ToString() + " tried to place hauled thing in cell but is not hauling anything.");
                }
                else
                {
                    SlotGroup slotGroup = actor.Map.haulDestinationManager.SlotGroupAt(cell);
                    if (slotGroup != null && slotGroup.Settings.AllowedToAccept(actor.carryTracker.CarriedThing))
                        actor.Map.designationManager.TryRemoveDesignationOn(actor.carryTracker.CarriedThing, DesignationDefOf.Haul);
                    Action<Thing, int> placedAction = (Action<Thing, int>)null;
                    if (curJob.def == RH_TET_MagicDefOf.RH_TET_Magic_DoBillIdol || curJob.def == JobDefOf.DoBill || curJob.def == JobDefOf.RefuelAtomic || curJob.def == JobDefOf.RearmTurretAtomic)
                        placedAction = (Action<Thing, int>)((th, added) =>
                        {
                            if (curJob.placedThings == null)
                                curJob.placedThings = new List<ThingCountClass>();
                            ThingCountClass thingCountClass = curJob.placedThings.Find((Predicate<ThingCountClass>)(x => x.thing == th));
                            if (thingCountClass != null)
                                thingCountClass.Count += added;
                            else
                                curJob.placedThings.Add(new ThingCountClass(th, added));
                        });
                    Thing resultingThing;
                    if (actor.carryTracker.TryDropCarriedThing(cell, ThingPlaceMode.Direct, out resultingThing, placedAction))
                        return;
                    if (storageMode)
                    {
                        IntVec3 foundCell;
                        if (nextToilOnPlaceFailOrIncomplete != null && (tryStoreInSameStorageIfSpotCantHoldWholeStack && StoreUtility.TryFindBestBetterStoreCellForIn(actor.carryTracker.CarriedThing, actor, actor.Map, StoragePriority.Unstored, actor.Faction, cell.GetSlotGroup(actor.Map), out foundCell, true) || StoreUtility.TryFindBestBetterStoreCellFor(actor.carryTracker.CarriedThing, actor, actor.Map, StoragePriority.Unstored, actor.Faction, out foundCell, true)))
                        {
                            if (actor.CanReserve((LocalTargetInfo)foundCell, 1, -1, (ReservationLayerDef)null, false))
                                actor.Reserve((LocalTargetInfo)foundCell, actor.CurJob, 1, -1, (ReservationLayerDef)null, true);
                            actor.CurJob.SetTarget(cellInd, (LocalTargetInfo)foundCell);
                            actor.jobs.curDriver.JumpToToil(nextToilOnPlaceFailOrIncomplete);
                        }
                        else
                        {
                            Job job = HaulAIUtility.HaulAsideJobFor(actor, actor.carryTracker.CarriedThing);
                            if (job != null)
                            {
                                curJob.targetA = job.targetA;
                                curJob.targetB = job.targetB;
                                curJob.targetC = job.targetC;
                                curJob.count = job.count;
                                curJob.haulOpportunisticDuplicates = job.haulOpportunisticDuplicates;
                                curJob.haulMode = job.haulMode;
                                actor.jobs.curDriver.JumpToToil(nextToilOnPlaceFailOrIncomplete);
                            }
                            else
                            {
                                Log.Error("Incomplete haul for " + (object)actor + ": Could not find anywhere to put " + (object)actor.carryTracker.CarriedThing + " near " + (object)actor.Position + ". Destroying. This should never happen!");
                                actor.carryTracker.CarriedThing.Destroy(DestroyMode.Vanish);
                            }
                        }
                    }
                    else
                    {
                        if (nextToilOnPlaceFailOrIncomplete == null)
                            return;
                        actor.jobs.curDriver.JumpToToil(nextToilOnPlaceFailOrIncomplete);
                    }
                }
            });
            return toil;
        }
    }
}
