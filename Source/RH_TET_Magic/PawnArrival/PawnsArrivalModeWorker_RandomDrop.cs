using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class PawnsArrivalModeWorker_RandomDrop : PawnsArrivalModeWorker
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map target = (Map)parms.target;
            bool canRoofPunch = parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer);
            for (int index = 0; index < pawns.Count; ++index)
                MagicTransportPodUtility.DropThingsNear(DropCellFinder.RandomDropSpot(target, true), target, Gen.YieldSingle<Thing>((Thing)pawns[index]), parms.podOpenDelay, false, true, canRoofPunch, true);
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            if (!Settings.AllowDropPodRaids)
                return false;

            if (!parms.raidArrivalModeForQuickMilitaryAid)
                parms.podOpenDelay = 520;
            parms.spawnRotation = Rot4.Random;
            return true;
        }
    }
}
