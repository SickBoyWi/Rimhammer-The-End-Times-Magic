using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class MagicTransportPodUtility
    {
        private static List<List<Thing>> tempList = new List<List<Thing>>();

        public static void MakeMagicPodAt(IntVec3 c, Map map, ActiveDropPodInfo info)
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
                if (result == null)
                {
                    Log.Warning("DropThingGroupsNear_NewTmpMineIn failed to find a place to mine in " + (object)thingsGroup.FirstOrDefault<Thing>() + " near " + (object)dropCenter + ". Dropping on random square instead.", false);
                    result = CellFinderLoose.RandomCellWith((Predicate<IntVec3>)(c => c.Walkable(map)), map, 1000);
                }

                ActiveDropPodInfo info = new ActiveDropPodInfo();
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
          List<ActiveDropPodInfo> dropPods,
          IntVec3 near,
          Map map,
          bool exactCell = false,
          bool draftFlag = false)
        {
            MagicTransportPodUtility.RemovePawnsFromWorldPawns(dropPods);
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

            SoundDefOf.DropPod_Open.PlayOneShot((SoundInfo)new TargetInfo(c, map, false));
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

        public static void DropThingsNear(
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
            MagicTransportPodUtility.DropThingGroupsNear(dropCenter, map, MagicTransportPodUtility.tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch, forbid);
            MagicTransportPodUtility.tempList.Clear();
        }

        public static void DropThingGroupsNear_NewTmp(
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
                IntVec3 result;
                if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out result, allowFogged, canRoofPunch, true, new IntVec2?()) && (canRoofPunch || !DropCellFinder.TryFindDropSpotNear(dropCenter, map, out result, allowFogged, true, true, new IntVec2?())))
                {
                    Log.Warning("DropThingsNear failed to find a place to drop " + (object)thingsGroup.FirstOrDefault<Thing>() + " near " + (object)dropCenter + ". Dropping on random square instead.", false);
                    result = CellFinderLoose.RandomCellWith((Predicate<IntVec3>)(c => c.Walkable(map)), map, 1000);
                }
                if (forbid)
                {
                    for (int index = 0; index < thingsGroup.Count; ++index)
                        thingsGroup[index].SetForbidden(true, false);
                }
                if (instaDrop)
                {
                    foreach (Thing thing in thingsGroup)
                        GenPlace.TryPlaceThing(thing, result, map, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4());
                }
                else
                {
                    ActiveDropPodInfo info = new ActiveDropPodInfo();
                    foreach (Thing thing in thingsGroup)
                        info.innerContainer.TryAdd(thing, true);
                    info.openDelay = openDelay;
                    info.leaveSlag = leaveSlag;
                    MagicTransportPodUtility.MakeMagicPodAt(result, map, info);
                }
            }
        }

        public static void DropThingGroupsNear(
          IntVec3 dropCenter,
          Map map,
          List<List<Thing>> thingsGroups,
          int openDelay = 110,
          bool instaDrop = false,
          bool leaveSlag = false,
          bool canRoofPunch = true,
          bool forbid = true)
        {
            MagicTransportPodUtility.DropThingGroupsNear_NewTmp(dropCenter, map, thingsGroups, openDelay, instaDrop, leaveSlag, canRoofPunch, forbid, true);
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

        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions<T>(
          Func<FloatMenuAcceptanceReport> acceptanceReportGetter,
          Func<T> arrivalActionGetter,
          string label,
          CompLaunchable representative,
          int destinationTile,
          Action<Action> uiConfirmationCallback = null)
          where T : TransportPodsArrivalAction
        {
            FloatMenuAcceptanceReport floatMenuAcceptanceReport = acceptanceReportGetter();
            if (floatMenuAcceptanceReport.Accepted || !floatMenuAcceptanceReport.FailReason.NullOrEmpty() || !floatMenuAcceptanceReport.FailMessage.NullOrEmpty())
            {
                if (!floatMenuAcceptanceReport.FailReason.NullOrEmpty())
                    yield return new FloatMenuOption(label + " (" + floatMenuAcceptanceReport.FailReason + ")", (Action)null, MenuOptionPriority.Default, (Action<Rect>)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null, true, 0);
                else
                    yield return new FloatMenuOption(label, (Action)(() =>
                    {
                        FloatMenuAcceptanceReport acceptanceReport = acceptanceReportGetter();
                        if (acceptanceReport.Accepted)
                        {
                            if (uiConfirmationCallback == null)
                                representative.TryLaunch(destinationTile, (TransportPodsArrivalAction)arrivalActionGetter());
                            else
                                uiConfirmationCallback((Action)(() => representative.TryLaunch(destinationTile, (TransportPodsArrivalAction)arrivalActionGetter())));
                        }
                        else
                        {
                            if (acceptanceReport.FailMessage.NullOrEmpty())
                                return;
                            Messages.Message(acceptanceReport.FailMessage, (LookTargets)new GlobalTargetInfo(destinationTile), MessageTypeDefOf.RejectInput, false);
                        }
                    }), MenuOptionPriority.Default, (Action<Rect>)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null, true, 0);
            }
        }

        public static bool AnyNonDownedColonist(IEnumerable<IThingHolder> pods)
        {
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
                ThingOwner directlyHeldThings = pod.GetDirectlyHeldThings();
                for (int index = 0; index < directlyHeldThings.Count; ++index)
                {
                    if (directlyHeldThings[index] is Pawn pawn && CaravanUtility.IsOwner(pawn, faction))
                        return true;
                }
            }
            return false;
        }

        public static Thing GetLookTarget(List<ActiveDropPodInfo> pods)
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

        public static void DropTravelingTransportPods(
          List<ActiveDropPodInfo> dropPods,
          IntVec3 near,
          Map map,
          bool exactCell = false,
          bool draftFlag = false)
        {
            MagicTransportPodUtility.RemovePawnsFromWorldPawns(dropPods);
            for (int index = 0; index < dropPods.Count; ++index)
            {
                IntVec3 c = new IntVec3();
                if (!exactCell || (!near.InBounds(map) || !near.Walkable(map) || near.Roofed(map)))
                    DropCellFinder.TryFindDropSpotNear(near, map, out c, false, true, true, new IntVec2?(), true);
                else
                    c = near;
                MagicTransportPodUtility.MakeDropPodAt(c, map, dropPods[index], RH_TET_MagicDefOf.RH_TET_ActiveMagicPod, RH_TET_MagicDefOf.RH_TET_MagicPodIncoming, draftFlag);
            }
        }

        public static void RemovePawnsFromWorldPawns(List<ActiveDropPodInfo> pods)
        {
            for (int index1 = 0; index1 < pods.Count; ++index1)
            {
                ThingOwner innerContainer = pods[index1].innerContainer;
                for (int index2 = 0; index2 < innerContainer.Count; ++index2)
                {
                    if (innerContainer[index2] is Pawn p && p.IsWorldPawn())
                        Find.WorldPawns.RemovePawn(p);
                }
            }
        }

        public static void MakeDropPodAt(
          IntVec3 c,
          Map map,
          ActiveDropPodInfo info,
          ThingDef makePodThing,
          ThingDef makeSkyfallerThing,
          bool draftFlag = false)
        {
            ActiveMagicPod tmActiveDropPod = (ActiveMagicPod)ThingMaker.MakeThing(makePodThing, (ThingDef)null);
            tmActiveDropPod.Contents = info;
            tmActiveDropPod.Contents.openDelay = 5;
            tmActiveDropPod.draftFlag = draftFlag;
            ((MagicPodIncoming)SkyfallerMaker.SpawnSkyfaller(makeSkyfallerThing, (Thing)tmActiveDropPod, c, map)).draftFlag = draftFlag;
            foreach (Thing thing in (IEnumerable<Thing>)tmActiveDropPod.Contents.innerContainer)
            {
                if (thing is Pawn p && p.IsWorldPawn())
                {
                    Find.WorldPawns.RemovePawn(p);
                    p.psychicEntropy?.SetInitialPsyfocusLevel();
                }
            }
        }

        public static void DropInDropPodsNearSpawnCenter(IncidentParms parms, List<Pawn> pawns)
        {
            Map target = (Map)parms.target;
            bool flag = parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer);
            MagicTransportPodUtility.DropThingsNear(parms.spawnCenter, target, pawns.Cast<Thing>(), parms.podOpenDelay, false, true, flag || parms.raidArrivalModeForQuickMilitaryAid, true);
        }
    }
}
