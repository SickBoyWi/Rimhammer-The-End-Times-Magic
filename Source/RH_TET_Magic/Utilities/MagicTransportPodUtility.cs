using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public static class MagicTransportPodUtility
    {

        private static readonly List<List<Thing>> tempList = new List<List<Thing>>();

        public static void DropInDropPodsNearSpawnCenter(IncidentParms parms, List<Pawn> pawns)
        {
            Map target = (Map)parms.target;
            bool flag = parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer);
            MagicTransportPodUtility.DropThingsNear(parms.spawnCenter, target, pawns.Cast<Thing>(), parms.podOpenDelay, false, true, flag || parms.raidArrivalModeForQuickMilitaryAid, true);
        }

        public static void MakeMagicTransportPodAt(
          IntVec3 c,
          Map map,
          ActiveTransporterInfo info,
          Faction faction = null)
        {
            ActiveMagicPod tmActiveDropPod = (ActiveMagicPod)ThingMaker.MakeThing(RH_TET_MagicDefOf.RH_TET_ActiveMagicPod, (ThingDef)null);
            tmActiveDropPod.Contents = info;
            ((MagicPodIncoming)SkyfallerMaker.SpawnSkyfaller(RH_TET_MagicDefOf.RH_TET_MagicPodIncoming, (Thing)tmActiveDropPod, c, map)).draftFlag = false;
            foreach (Thing thing in (IEnumerable<Thing>)tmActiveDropPod.Contents.innerContainer)
            {
                if (thing is Pawn p && p.IsWorldPawn())
                {
                    Find.WorldPawns.RemovePawn(p);
                    p.psychicEntropy?.SetInitialPsyfocusLevel();
                }
            }
        }

        public static void DropThingsNear(
          IntVec3 dropCenter,
          Map map,
          IEnumerable<Thing> things,
          int openDelay = 110,
          bool canInstaDropDuringInit = false,
          bool leaveSlag = false,
          bool canRoofPunch = true,
          bool forbid = true,
          bool allowFogged = true,
          Faction faction = null)
        {
            MagicTransportPodUtility.tempList.Clear();
            foreach (Thing thing in things)
                MagicTransportPodUtility.tempList.Add(new List<Thing>()
                    {
                      thing
                    });
            MagicTransportPodUtility.DropThingGroupsNear(dropCenter, map, MagicTransportPodUtility.tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch, forbid, allowFogged, false, faction);
            MagicTransportPodUtility.tempList.Clear();
        }

        public static void DropThingGroupsNear(
          IntVec3 dropCenter,
          Map map,
          List<List<Thing>> thingsGroups,
          int openDelay = 110,
          bool instaDrop = false,
          bool leaveSlag = false,
          bool canRoofPunch = true,
          bool forbid = true,
          bool allowFogged = true,
          bool canTransfer = false,
          Faction faction = null)
        {
            foreach (List<Thing> thingsGroup in thingsGroups)
            {
                IntVec3 result;
                if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out result, allowFogged, canRoofPunch, true, new IntVec2?(), true) && (canRoofPunch || !DropCellFinder.TryFindDropSpotNear(dropCenter, map, out result, allowFogged, true, true, new IntVec2?(), true)))
                {
                    if (dropCenter.IsValid)
                    {
                        Log.Warning("DropThingsNear failed to find a place to drop " + thingsGroup.FirstOrDefault<Thing>()?.ToString() + " near " + dropCenter.ToString() + ". Dropping on random square instead.");
                        result = CellFinderLoose.RandomCellWith((Predicate<IntVec3>)(c =>
                        {
                            if (!c.Walkable(map))
                                return false;
                            RoofDef roof = c.GetRoof(map);
                            return (roof == null ? 0 : (roof.isThickRoof ? 1 : 0)) == 0;
                        }), map, 1000);
                    }
                    else
                        continue;
                }
                if (forbid)
                {
                    for (int index = 0; index < thingsGroup.Count; ++index)
                        thingsGroup[index].SetForbidden(true, false);
                }
                if (instaDrop)
                {
                    foreach (Thing thing in thingsGroup)
                        GenPlace.TryPlaceThing(thing, result, map, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4?(), 1);
                }
                else
                {
                    ActiveTransporterInfo info = new ActiveTransporterInfo();
                    foreach (Thing thing in thingsGroup)
                        info.innerContainer.TryAdd(thing, true);
                    info.openDelay = openDelay;
                    info.leaveSlag = leaveSlag;
                    MagicTransportPodUtility.MakeMagicTransportPodAt(result, map, info, faction);
                }
            }
        }

        public static bool IsShuttle(this List<ActiveTransporterInfo> transporters)
        {
            return transporters.Count == 1 && transporters[0].GetShuttle() != null;
        }

        public static bool IsShuttle(this List<CompTransporter> transporters)
        {
            return transporters.Count == 1 && transporters[0].Shuttle != null;
        }

        public static CompShuttle AsShuttle(this List<CompTransporter> transporters)
        {
            return transporters.Count != 1 ? (CompShuttle)null : transporters[0].Shuttle;
        }

        public static void GetTransportersInGroup(
          int transportersGroup,
          Map map,
          List<CompTransporter> outTransporters)
        {
            outTransporters.Clear();
            if (transportersGroup < 0)
                return;
            List<Thing> thingList = map.listerThings.ThingsInGroup(ThingRequestGroup.Transporter);
            for (int index = 0; index < thingList.Count; ++index)
            {
                CompTransporter comp = thingList[index].TryGetComp<CompTransporter>();
                if (comp.groupID == transportersGroup)
                    outTransporters.Add(comp);
            }
        }

        public static Lord FindLord(int transportersGroup, Map map)
        {
            List<Lord> lords = map.lordManager.lords;
            for (int index = 0; index < lords.Count; ++index)
            {
                if (lords[index].LordJob is LordJob_LoadAndEnterTransporters lordJob && lordJob.transportersGroup == transportersGroup)
                    return lords[index];
            }
            return (Lord)null;
        }

        public static bool WasLoadingCanceled(Thing transporter)
        {
            CompTransporter comp = transporter.TryGetComp<CompTransporter>();
            return comp != null && !comp.LoadingInProgressOrReadyToLaunch;
        }

        public static int InitiateLoading(IEnumerable<CompTransporter> transporters)
        {
            int transporterGroupId = Find.UniqueIDsManager.GetNextTransporterGroupID();
            foreach (CompTransporter transporter in transporters)
                transporter.groupID = transporterGroupId;
            return transporterGroupId;
        }

        public static IEnumerable<Pawn> AllSendablePawns(
          List<CompTransporter> transporters,
          Map map)
        {
            CompShuttle shuttle = transporters[0].parent.TryGetComp<CompShuttle>();
            Map map1 = map;
            int num1 = !transporters[0].Props.canChangeAssignedThingsAfterStarting || !transporters[0].LoadingInProgressOrReadyToLaunch ? -1 : transporters[0].groupID;
            int num2 = shuttle != null ? 1 : 0;
            int allowLoadAndEnterTransportersLordForGroupID = num1;
            List<Pawn> pawns = CaravanFormingUtility.AllSendablePawns(map1, true, false, false, false, num2 != 0, allowLoadAndEnterTransportersLordForGroupID);
            for (int i = 0; i < pawns.Count; ++i)
            {
                if (shuttle == null || shuttle.IsRequired((Thing)pawns[i]) || shuttle.IsAllowed((Thing)pawns[i]))
                    yield return pawns[i];
            }
        }

        public static IEnumerable<Thing> AllSendableItems(
          List<CompTransporter> transporters,
          Map map)
        {
            List<Thing> items = CaravanFormingUtility.AllReachableColonyItems(map, !map.IsPlayerHome, transporters[0].Props.canChangeAssignedThingsAfterStarting && transporters[0].LoadingInProgressOrReadyToLaunch, false);
            CompShuttle shuttle = transporters[0].parent.TryGetComp<CompShuttle>();
            for (int i = 0; i < items.Count; ++i)
            {
                if (shuttle == null || shuttle.IsRequired(items[i]) || shuttle.IsAllowed(items[i]))
                    yield return items[i];
            }
        }

        public static IEnumerable<Thing> ThingsBeingHauledTo(
          List<CompTransporter> transporters,
          Map map)
        {
            IReadOnlyList<Pawn> pawns = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < ((IReadOnlyCollection<Pawn>)pawns).Count(); ++i)
            {
                if (pawns[i].CurJobDef == JobDefOf.HaulToTransporter && transporters.Contains(((JobDriver_HaulToTransporter)pawns[i].jobs.curDriver).Transporter) && pawns[i].carryTracker.CarriedThing != null)
                    yield return pawns[i].carryTracker.CarriedThing;
            }
        }

        public static void MakeLordsAsAppropriate(
          List<Pawn> pawns,
          List<CompTransporter> transporters,
          Map map)
        {
            int groupID = transporters[0].groupID;
            Lord lord = (Lord)null;
            IEnumerable<Pawn> source = pawns.Where<Pawn>((Func<Pawn, bool>)(x => (x.IsColonist || x.IsColonyMechPlayerControlled) && !x.Downed && x.Spawned));
            if (source.Any<Pawn>())
            {
                lord = map.lordManager.lords.Find((Predicate<Lord>)(x => x.LordJob is LordJob_LoadAndEnterTransporters lordJob && lordJob.transportersGroup == groupID)) ?? LordMaker.MakeNewLord(Faction.OfPlayer, (LordJob)new LordJob_LoadAndEnterTransporters(groupID), map, (IEnumerable<Pawn>)null);
                foreach (Pawn pawn in source)
                {
                    if (!lord.ownedPawns.Contains(pawn))
                    {
                        pawn.GetLord()?.Notify_PawnLost(pawn, PawnLostCondition.ForcedToJoinOtherLord, new DamageInfo?());
                        lord.AddPawn(pawn);
                        pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
                    }
                }
                for (int index = lord.ownedPawns.Count - 1; index >= 0; --index)
                {
                    if (!source.Contains<Pawn>(lord.ownedPawns[index]))
                        lord.Notify_PawnLost(lord.ownedPawns[index], PawnLostCondition.NoLongerEnteringTransportPods, new DamageInfo?());
                }
            }
            for (int index = map.lordManager.lords.Count - 1; index >= 0; --index)
            {
                if (map.lordManager.lords[index].LordJob is LordJob_LoadAndEnterTransporters lordJob && lordJob.transportersGroup == groupID && map.lordManager.lords[index] != lord)
                    map.lordManager.RemoveLord(map.lordManager.lords[index]);
            }
        }

        public static bool IncomingTransporterPreventingMapRemoval(Map map)
        {
            if (ModsConfig.OdysseyActive)
            {
                WorldComponent_GravshipController gravshipController = Find.GravshipController;
                if ((gravshipController != null ? (gravshipController.LandingAreaConfirmationInProgress ? 1 : 0) : 0) != 0 || Find.CurrentGravship != null && Find.CurrentGravship.destinationTile == map.Tile)
                    return true;
            }
            foreach (TravellingTransporters travellingTransporter in Find.WorldObjects.TravellingTransporters)
            {
                if (travellingTransporter.destinationTile == map.Tile)
                    return true;
            }
            return false;
        }

        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions<T>(
          Func<FloatMenuAcceptanceReport> acceptanceReportGetter,
          Func<T> arrivalActionGetter,
          string label,
          Action<PlanetTile, TransportersArrivalAction> launchAction,
          PlanetTile destinationTile,
          Action<Action> uiConfirmationCallback = null)
          where T : TransportersArrivalAction
        {
            FloatMenuAcceptanceReport acceptanceReport1 = acceptanceReportGetter();
            if (acceptanceReport1.Accepted || !acceptanceReport1.FailReason.NullOrEmpty() || !acceptanceReport1.FailMessage.NullOrEmpty())
            {
                if (!acceptanceReport1.Accepted && !acceptanceReport1.FailReason.NullOrEmpty())
                    label = label + " (" + acceptanceReport1.FailReason + ")";
                yield return new FloatMenuOption(label, (Action)(() =>
                {
                    FloatMenuAcceptanceReport acceptanceReport2 = acceptanceReportGetter();
                    if (acceptanceReport2.Accepted)
                    {
                        if (uiConfirmationCallback == null)
                            launchAction(destinationTile, (TransportersArrivalAction)arrivalActionGetter());
                        else
                            uiConfirmationCallback((Action)(() => launchAction(destinationTile, (TransportersArrivalAction)arrivalActionGetter())));
                    }
                    else
                    {
                        if (acceptanceReport2.FailMessage.NullOrEmpty())
                            return;
                        Messages.Message(acceptanceReport2.FailMessage, (LookTargets)new GlobalTargetInfo(destinationTile), MessageTypeDefOf.RejectInput, false);
                    }
                }), MenuOptionPriority.Default, (Action<Rect>)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null, true, 0);
            }
        }

        public static bool AnyNonDownedColonist(IEnumerable<IThingHolder> pods)
        {
            if (CaravanShuttleUtility.IsCaravanShuttle(pods.First<IThingHolder>() as CompTransporter))
                return true;
            foreach (IThingHolder pod in pods)
            {
                ThingOwner directlyHeldThings = pod.GetDirectlyHeldThings();
                for (int index = 0; index < directlyHeldThings.Count; ++index)
                {
                    if (directlyHeldThings[index] is Pawn pawn && pawn.IsColonist && !pawn.Downed)
                        return true;
                }
            }
            return false;
        }

        public static bool AnyPotentialCaravanOwner(IEnumerable<IThingHolder> pods, Faction faction)
        {
            foreach (IThingHolder pod in pods)
            {
                if (pod is CompTransporter transporter && CaravanShuttleUtility.IsCaravanShuttle(transporter))
                    return true;
                ThingOwner directlyHeldThings = pod.GetDirectlyHeldThings();
                for (int index = 0; index < directlyHeldThings.Count; ++index)
                {
                    if (directlyHeldThings[index] is Pawn pawn && CaravanUtility.IsOwner(pawn, faction))
                        return true;
                }
            }
            return false;
        }

        public static Thing GetLookTarget(List<ActiveTransporterInfo> pods)
        {
            for (int index1 = 0; index1 < pods.Count; ++index1)
            {
                ThingOwner directlyHeldThings = pods[index1].GetDirectlyHeldThings();
                for (int index2 = 0; index2 < directlyHeldThings.Count; ++index2)
                {
                    if (directlyHeldThings[index2] is Pawn pawn && pawn.IsColonist)
                        return (Thing)pawn;
                }
            }
            for (int index = 0; index < pods.Count; ++index)
            {
                Thing thing = pods[index].GetDirectlyHeldThings().FirstOrDefault<Thing>();
                if (thing != null)
                    return thing;
            }
            return (Thing)null;
        }

        public static void DropTravellingDropPods(
          List<ActiveTransporterInfo> transporters,
          IntVec3 near,
          Map map)
        {
            TransportersArrivalActionUtility.RemovePawnsFromWorldPawns((IEnumerable<ActiveTransporterInfo>)transporters);
            for (int index = 0; index < transporters.Count; ++index)
            {
                IntVec3 result;
                DropCellFinder.TryFindDropSpotNear(near, map, out result, false, true, true, new IntVec2?(), true);
                MagicTransportPodUtility.MakeMagicTransportPodAt(result, map, transporters[index], (Faction)null);
            }
        }

        public static Thing DropShuttle(
          ActiveTransporterInfo transporter,
          Map map,
          IntVec3 near,
          Rot4? rotation = null,
          Faction faction = null)
        {
            TransportersArrivalActionUtility.RemovePawnsFromWorldPawns(Gen.YieldSingle<ActiveTransporterInfo>(transporter));
            Thing thing = transporter.RemoveShuttle() ?? QuestGen_Shuttle.GenerateShuttle(faction, (IEnumerable<Pawn>)null, (IEnumerable<ThingDefCount>)null, false, false, false, 0, false, false, true, false, (WorldObject)null, (WorldObject)null, -1, (ThingDef)null, false, true, false, false, false);
            TransportShipDef shipDef = thing.TryGetComp<CompShuttle>()?.Props.shipDef ?? TransportShipDefOf.Ship_Shuttle;
            rotation.GetValueOrDefault();
            if (!rotation.HasValue)
                rotation = new Rot4?(shipDef.shipThing.defaultPlacingRot);
            thing.Rotation = rotation.Value;
            IntVec3 result;
            if ((bool)RoyalTitlePermitWorker_CallShuttle.ShuttleCanLandHere((LocalTargetInfo)near, map, shipDef.shipThing, new Rot4?(rotation.Value)))
                result = near;
            else if (!CellFinder.TryFindRandomCellNear(near, map, 10, (Predicate<IntVec3>)(c => (bool)RoyalTitlePermitWorker_CallShuttle.ShuttleCanLandHere((LocalTargetInfo)c, map, shipDef.shipThing, new Rot4?(rotation.Value))), out result, -1))
            {
                Log.Warning("Could not find a suitable cell for shuttle landing.");
                result = near;
            }
            CompTransporter comp = thing.TryGetComp<CompTransporter>();
            comp.innerContainer.TryAddRangeOrTransfer((IEnumerable<Thing>)transporter.innerContainer, true, false);
            TransportShip transportShip = comp.Shuttle.shipParent ?? TransportShipMaker.MakeTransportShip(shipDef, (IEnumerable<Thing>)null, thing);
            if (!result.IsValid)
                result = DropCellFinder.GetBestShuttleLandingSpot(map, Faction.OfPlayer);
            transportShip.ArriveAt(result, map.Parent);
            TransportShipDef shipDef1 = thing.TryGetComp<CompShuttle>().Props.shipDef;
            if (shipDef1 == null || !shipDef1.playerShuttle)
            {
                transportShip.AddJobs(ShipJobDefOf.Unload, ShipJobDefOf.FlyAway);
            }
            else
            {
                ShipJob_Unload shipJobUnload = (ShipJob_Unload)ShipJobMaker.MakeShipJob(ShipJobDefOf.Unload);
                shipJobUnload.dropMode = TransportShipDropMode.PawnsOnly;
                transportShip.AddJob((ShipJob)shipJobUnload);
            }
            return thing;
        }

        public static void DropInDropPodsNearSpawnCenterMineIn(IncidentParms parms, List<Pawn> pawns)
        {
            Map target = (Map)parms.target;
            bool flag = parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer);
            MagicTransportPodUtility.MineInNear(parms.spawnCenter, target, pawns.Cast<Thing>(), parms.podOpenDelay, false, true, flag || parms.raidArrivalModeForQuickMilitaryAid, true);
        }

        public static void MineInNear(
             IntVec3 dropCenter,
             Map map,
             IEnumerable<Thing> things,
             int openDelay = 110,
             bool canInstaDropDuringInit = false,
             bool leaveSlag = false,
             bool canRoofPunch = true,
             bool forbid = true)
        {
            MagicTransportPodUtility.tempList.Clear();
            foreach (Thing thing in things)
                MagicTransportPodUtility.tempList.Add(new List<Thing>()
                {
                  thing
                });
            MagicTransportPodUtility.DropThingGroupsNearMineIn(dropCenter, map, MagicTransportPodUtility.tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch, forbid);
            MagicTransportPodUtility.tempList.Clear();
        }

        public static void DropThingGroupsNearMineIn(
          IntVec3 dropCenter,
          Map map,
          List<List<Thing>> thingsGroups,
          int openDelay = 110,
          bool instaDrop = false,
          bool leaveSlag = false,
          bool canRoofPunch = true,
          bool forbid = true)
        {
            MagicTransportPodUtility.DropThingGroupsNear_NewTmpMineIn(dropCenter, map, thingsGroups, openDelay, instaDrop, leaveSlag, canRoofPunch, forbid, true);
        }

        public static void DropThingGroupsNear_NewTmpMineIn(
          IntVec3 dropCenter,
          Map map,
          List<List<Thing>> thingsGroups,
          int openDelay = 110,
          bool instaDrop = false,
          bool leaveSlag = false,
          bool canRoofPunch = true,
          bool forbid = true,
          bool allowFogged = true)
        {
            foreach (List<Thing> thingsGroup in thingsGroups)
            {
                IntVec3 result = MagicTransportPodUtility.FindRootTunnelLoc(map, true, true);
                if (result == IntVec3.Invalid)
                {
                    Log.Warning("DropThingGroupsNear_NewTmpMineIn failed to find a place to mine in " + (object)thingsGroup.FirstOrDefault<Thing>() + " near " + (object)dropCenter + ". Dropping on random square instead.");
                    result = CellFinderLoose.RandomCellWith((Predicate<IntVec3>)(c => c.Walkable(map)), map, 1000);
                }

                ActiveTransporterInfo info = new ActiveTransporterInfo();
                foreach (Thing thing in thingsGroup)
                    info.innerContainer.TryAdd(thing, true);
                info.openDelay = openDelay;
                info.leaveSlag = leaveSlag;
                //MagicTransportPodUtility.MakeMagicPodAt(result, map, info);

                MagicTransportPodUtility.MineInAt(result, map, info.innerContainer, RH_TET_MagicDefOf.RH_TET_ActiveMagicPod, RH_TET_MagicDefOf.RH_TET_MagicPodIncoming, false);
                info.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
                //}
            }
        }

        internal static void DropTravelingTransportPodsMineIn(
          List<ActiveTransporterInfo> dropPods,
          IntVec3 near,
          Map map,
          bool exactCell = false,
          bool draftFlag = false)
        {
            TransportersArrivalActionUtility.RemovePawnsFromWorldPawns(dropPods);
            for (int index = 0; index < dropPods.Count; ++index)
            {
                IntVec3 c = new IntVec3();
                c = near;
                MagicTransportPodUtility.MineInAt(c, map, dropPods[index].innerContainer, RH_TET_MagicDefOf.RH_TET_ActiveMagicPod, RH_TET_MagicDefOf.RH_TET_MagicPodIncoming, draftFlag);
                dropPods[index].innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            }
        }

        public static void MineInAt(
         IntVec3 c,
         Map map,
         ThingOwner info,
         ThingDef makePodThing,
         ThingDef makeSkyfallerThing,
         bool draftFlag = false)
        {
            IntVec3 loc = new IntVec3();
            loc = MagicTransportPodUtility.FindRootTunnelLoc(map, true, true);
            if (!loc.IsValid)
                loc = c;

            foreach (Thing thing2 in (IEnumerable<Thing>)info)
            {
                if (thing2 is Pawn p && p.IsWorldPawn())
                {
                    Find.WorldPawns.RemovePawn(p);
                    p.psychicEntropy?.SetInitialPsyfocusLevel();

                    // spawn pawn here.
                    GenPlace.TryPlaceThing(ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel, (ThingDef)null), c, map, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4());

                    Rot4 rot4 = Rot4.South;

                    Thing lastResultingThing;
                    lastResultingThing = GenSpawn.Spawn(thing2, c, map, rot4, WipeMode.Vanish, false);

                    if (lastResultingThing is Pawn pawn)
                    {
                        if (pawn.RaceProps.Humanlike)
                            TaleRecorder.RecordTale(TaleDefOf.LandedInPod, (object)pawn);
                        if (pawn.IsColonist && pawn.Spawned && !map.IsPlayerHome)
                            pawn.drafter.Drafted = true;
                        if (pawn.guest != null && pawn.guest.IsPrisoner)
                            pawn.guest.WaitInsteadOfEscapingForDefaultTicks();
                    }
                }
            }

            DefDatabase<SoundDef>.GetNamed("DropPod_Open").PlayOneShot((SoundInfo)new TargetInfo(c, map, false));
        }

        private static IntVec3 FindRootTunnelLoc(
          Map map,
          bool spawnAnywhereIfNoGoodCell = false,
          bool ignoreRoofIfNoGoodCell = false)
        {
            IntVec3 intVec3;
            if (InfestationCellFinder.TryFindCell(out intVec3, map))
                return intVec3;
            if (!spawnAnywhereIfNoGoodCell)
                return IntVec3.Invalid;
            Func<IntVec3, bool, bool> validator = (Func<IntVec3, bool, bool>)((x, canIgnoreRoof) =>
            {
                if (!x.Standable(map) || x.Fogged(map))
                    return false;
                if (!canIgnoreRoof)
                {
                    bool flag = false;
                    int num = GenRadial.NumCellsInRadius(3f);
                    for (int index = 0; index < num; ++index)
                    {
                        IntVec3 c = x + GenRadial.RadialPattern[index];
                        if (c.InBounds(map))
                        {
                            RoofDef roof = c.GetRoof(map);
                            if (roof != null && roof.isThickRoof)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                        return false;
                }
                return true;
            });
            return RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((Predicate<IntVec3>)(x => validator(x, false)), map, out intVec3) || ignoreRoofIfNoGoodCell && RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((Predicate<IntVec3>)(x => validator(x, true)), map, out intVec3) ? intVec3 : IntVec3.Invalid;
        }
    }
}
