using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class MagicTransportPodsArrivalAction_AttackSettlement : TransportersArrivalAction_AttackSettlement
    {
        private Settlement settlement;
        private PawnsArrivalModeDef arrivalMode;

        public MagicTransportPodsArrivalAction_AttackSettlement(TransportersArrivalAction_AttackSettlement action)
        {
            this.settlement = Traverse.Create((object)action).Field("settlement").GetValue<Settlement>();
            this.arrivalMode = Traverse.Create((object)action).Field("arrivalMode").GetValue<PawnsArrivalModeDef>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Settlement>(ref this.settlement, "settlementA", false);
            Scribe_Defs.Look<PawnsArrivalModeDef>(ref this.arrivalMode, "arrivalModeA");
        }

        public override void Arrived(List<ActiveTransporterInfo> pods, PlanetTile tile)
        {
            Thing lookTarget = TransportersArrivalActionUtility.GetLookTarget(pods);
            int num = !this.settlement.HasMap ? 1 : 0;
            Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(this.settlement.Tile, (WorldObjectDef)null);
            TaggedString letterLabel = "LetterLabelCaravanEnteredEnemyBase".Translate();
            TaggedString letterText = "LetterTransportPodsLandedInEnemyBase".Translate((NamedArgument)this.settlement.Label).CapitalizeFirst();
            SettlementUtility.AffectRelationsOnAttacked((MapParent)this.settlement, ref letterText);
            if (num != 0)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter((IEnumerable<Pawn>)orGenerateMap.mapPawns.AllPawns, ref letterLabel, ref letterText, (string)"LetterRelatedPawnsInMapWherePlayerLanded".Translate((NamedArgument)Faction.OfPlayer.def.pawnsPlural), true, true);
            }
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent, (LookTargets)lookTarget, this.settlement.Faction, (Quest)null, (List<ThingDef>)null, (string)null);
            this.TravelingTransportPodsArrived(pods, orGenerateMap);
        }

        private void TravelingTransportPodsArrived(List<ActiveTransporterInfo> pods, Map orGenerateMap)
        {
            if (this.arrivalMode == PawnsArrivalModeDefOf.CenterDrop)
                TravelingTransportPodsArrivedCenter(pods, orGenerateMap);
            else if (this.arrivalMode == PawnsArrivalModeDefOf.EdgeDrop)
                TravelingTransportPodsArrivedEdge(pods, orGenerateMap);
            else
            {
                Log.Warning("Unknown pawn arrival mode for magic pod drop attack. Defaulting to Edge Arrival Mode.");
                TravelingTransportPodsArrivedEdge(pods, orGenerateMap);
            }
        }

        private void TravelingTransportPodsArrivedCenter(List<ActiveTransporterInfo> dropPods, Map map)
        {
            IntVec3 spot;
            if (!DropCellFinder.TryFindRaidDropCenterClose(out spot, map, true, true, true, -1))
                spot = DropCellFinder.FindRaidDropCenterDistant(map, false);
            MagicTransportPodUtility.DropTravelingTransportPods(dropPods, spot, map);
        }

        private void TravelingTransportPodsArrivedEdge(List<ActiveTransporterInfo> dropPods, Map map)
        {
            IntVec3 dropCenterDistant = DropCellFinder.FindRaidDropCenterDistant(map, false);
            MagicTransportPodUtility.DropTravelingTransportPods(dropPods, dropCenterDistant, map);
        }

        public override bool ShouldUseLongEvent(List<ActiveTransporterInfo> pods, PlanetTile tile)
        {
            return !this.settlement.HasMap;
        }
    }
}
