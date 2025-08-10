using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class MagicTransportPodsArrivalAction_LandInSpecificCell : TransportersArrivalAction
    {
        private MapParent mapParent;
        private IntVec3 cell;
        private Rot4 rotation;
        private bool landInShuttle;

        public override bool GeneratesMap
        {
            get
            {
                return false;
            }
        }

        public MagicTransportPodsArrivalAction_LandInSpecificCell()
        {
        }

        public MagicTransportPodsArrivalAction_LandInSpecificCell(MapParent mapParent, IntVec3 cell)
        {
            this.mapParent = mapParent;
            this.cell = cell;
        }

        public MagicTransportPodsArrivalAction_LandInSpecificCell(
          MapParent mapParent,
          IntVec3 cell,
          Rot4 rotation,
          bool landInShuttle)
        {
            this.mapParent = mapParent;
            this.cell = cell;
            this.rotation = rotation;
            this.landInShuttle = landInShuttle;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<MapParent>(ref this.mapParent, "mapParentA", false);
            Scribe_Values.Look<IntVec3>(ref this.cell, "cellA", new IntVec3(), false);
            Scribe_Values.Look<Rot4>(ref this.rotation, "rotationA", new Rot4(), false);
            Scribe_Values.Look<bool>(ref this.landInShuttle, "landInShuttleA", false, false);
        }

        public override FloatMenuAcceptanceReport StillValid(
          IEnumerable<IThingHolder> pods,
          PlanetTile destinationTile)
        {
            FloatMenuAcceptanceReport acceptanceReport = base.StillValid(pods, destinationTile);
            if (!(bool)acceptanceReport)
                return acceptanceReport;
            return this.mapParent != null && this.mapParent.Tile != destinationTile ? (FloatMenuAcceptanceReport)false : (FloatMenuAcceptanceReport)MagicTransportPodsArrivalAction_LandInSpecificCell.CanLandInSpecificCell(pods, this.mapParent);
        }

        public override void Arrived(List<ActiveTransporterInfo> transporters, PlanetTile tile)
        {
            Thing lookTarget = MagicTransportPodUtility.GetLookTarget(transporters);
            if (this.landInShuttle)
            {
                if (transporters.Count > 1)
                    Log.Error("Shuttles can only have one transporter in group");
                MagicTransportPodUtility.DropShuttle(transporters.FirstOrDefault<ActiveTransporterInfo>(), this.mapParent.Map, this.cell, new Rot4?(this.rotation), (Faction)null);
                Messages.Message((string)"MessageShuttleArrived".Translate(), (LookTargets)lookTarget, MessageTypeDefOf.TaskCompletion, true);
            }
            else
            {
                MagicTransportPodUtility.DropTravellingDropPods(transporters, this.cell, this.mapParent.Map);
                Messages.Message((string)"MessageTransportPodsArrived".Translate(), (LookTargets)lookTarget, MessageTypeDefOf.TaskCompletion, true);
            }
        }

        public static bool CanLandInSpecificCell(IEnumerable<IThingHolder> pods, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned || !mapParent.HasMap)
                return false;
            return !mapParent.EnterCooldownBlocksEntering() || (bool)FloatMenuAcceptanceReport.WithFailMessage((string)"MessageEnterCooldownBlocksEntering".Translate((NamedArgument)mapParent.EnterCooldownTicksLeft().ToStringTicksToPeriod(true, false, true, true, false)));
        }
    }
}
