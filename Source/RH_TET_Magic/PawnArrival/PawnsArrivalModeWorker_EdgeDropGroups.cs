using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class PawnsArrivalModeWorker_EdgeDropGroups : PawnsArrivalModeWorker
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map target = (Map)parms.target;
            bool flag = parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer);
            List<Pair<List<Pawn>, IntVec3>> groups = PawnsArrivalModeWorkerUtility.SplitIntoRandomGroupsNearMapEdge(pawns, target, true);
            PawnsArrivalModeWorkerUtility.SetPawnGroupsInfo(parms, groups);
            for (int index = 0; index < groups.Count; ++index)
            {
                Pair<List<Pawn>, IntVec3> pair = groups[index];
                IntVec3 second = pair.Second;
                Map map = target;
                pair = groups[index];
                IEnumerable<Thing> things = pair.First.Cast<Thing>();
                int podOpenDelay = parms.podOpenDelay;
                int num = flag ? 1 : 0;
                MagicTransportPodUtility.DropThingsNear(second, map, things, podOpenDelay, false, true, num != 0, true);
            }
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            if (!Settings.AllowDropPodRaids)
                return false;

            parms.spawnRotation = Rot4.Random;
            return true;
        }
    }
}
