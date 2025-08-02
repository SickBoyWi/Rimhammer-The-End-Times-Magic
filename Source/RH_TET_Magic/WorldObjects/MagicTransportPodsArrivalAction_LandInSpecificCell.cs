using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class MagicTransportPodsArrivalAction_LandInSpecificCell : TransportersArrivalAction
    {
        public bool draftFlag = false;
        private MapParent mapParent;
        private IntVec3 cell;

        public override bool GeneratesMap
        {
            get
            {
                return true;
            }
        }

        public MagicTransportPodsArrivalAction_LandInSpecificCell()
        {
        }

        public MagicTransportPodsArrivalAction_LandInSpecificCell(
          MapParent mapParent,
          IntVec3 cell,
          bool dFlag = false)
        {
            this.mapParent = mapParent;
            this.cell = cell;
            this.draftFlag = dFlag;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<MapParent>(ref this.mapParent, "mapParent", false);
            Scribe_Values.Look<IntVec3>(ref this.cell, "cell", new IntVec3(), false);
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

        public override void Arrived(List<ActiveTransporterInfo> pods, PlanetTile tile)
        {
            MagicTransportPodUtility.DropTravelingTransportPods(pods, this.cell, this.mapParent.Map, true, this.draftFlag);
        }

        public static bool CanLandInSpecificCell(IEnumerable<IThingHolder> pods, MapParent mapParent)
        {
            if (mapParent == null || !mapParent.Spawned || !mapParent.HasMap)
                return false;
            return !mapParent.EnterCooldownBlocksEntering() || (bool)FloatMenuAcceptanceReport.WithFailMessage((string)"MessageEnterCooldownBlocksEntering".Translate((NamedArgument)mapParent.EnterCooldownDaysLeft().ToString("0.#")));
        }
    }
}
