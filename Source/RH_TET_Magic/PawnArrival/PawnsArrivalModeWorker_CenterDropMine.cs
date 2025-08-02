using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    /**
     * 
     * REALLY SLOW WHEN IT HAPPENS, AND DOESN'T SPAWN PAWNS.
     * 
     * SPENT TOO MANY HOURS ON THIS STUFF FOR NOW, BUT WILL REVISIT.
     * 
     * 20220626
     * 
     * 
     */
    public class PawnsArrivalModeWorker_CenterDropMine : PawnsArrivalModeWorker
    {
        public const int PodOpenDelay = 520;

        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            MagicTransportPodUtility.DropInDropPodsNearSpawnCenterMineIn(parms, pawns);
        }

        public override void TravellingTransportersArrived(List<ActiveTransporterInfo> dropPods, Map map)
        {
            IntVec3 spot;
            if (!InfestationCellFinder.TryFindCell(out spot, map))
                spot = CellFinderLoose.TryFindCentralCell(map, 100, 4000, (Predicate<IntVec3>)(c => c.Walkable(map)));
            MagicTransportPodUtility.DropTravelingTransportPodsMineIn(dropPods, spot, map);
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            if (!Settings.AllowDropPodRaids)
                return false;

            Map target = (Map)parms.target;
            if (!parms.raidArrivalModeForQuickMilitaryAid)
                parms.podOpenDelay = PodOpenDelay;
            parms.spawnRotation = Rot4.Random;
            if (!parms.spawnCenter.IsValid)
            {
                bool flag1 = parms.faction != null && parms.faction == Faction.OfMechanoids;
                bool flag2 = parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer);
                if (!InfestationCellFinder.TryFindCell(out parms.spawnCenter, target))
                {
                    parms.spawnCenter = CellFinderLoose.TryFindCentralCell(target, 100, 4000, (Predicate<IntVec3>)(c => c.Walkable(target)));
                }
            }
            return true;
        }
    }
}
