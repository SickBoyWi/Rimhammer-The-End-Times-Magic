using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class PawnsArrivalModeWorker_CenterDrop : PawnsArrivalModeWorker
    {
        public const int PodOpenDelay = 520;

        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            MagicTransportPodUtility.DropInDropPodsNearSpawnCenter(parms, pawns);
        }

        public override void TravellingTransportersArrived(List<ActiveTransporterInfo> dropPods, Map map)
        {
            IntVec3 spot;
            if (!DropCellFinder.TryFindRaidDropCenterClose(out spot, map, true, true, true, -1))
                spot = DropCellFinder.FindRaidDropCenterDistant(map, false);
            MagicTransportPodUtility.DropTravelingTransportPods(dropPods, spot, map);
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
                if (!DropCellFinder.TryFindRaidDropCenterClose(out parms.spawnCenter, target, !flag1 & flag2, !flag1, true, -1))
                {
                    parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeDrop;
                    return parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms);
                }
            }
            return true;
        }
    }
}
