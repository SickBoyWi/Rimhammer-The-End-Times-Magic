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

        public override bool GeneratesMap
        {
            get
            {
                return true;
            }
        }

        public MagicTransportPodsArrivalAction_AttackSettlement()
        {
        }

        public MagicTransportPodsArrivalAction_AttackSettlement(TransportersArrivalAction_AttackSettlement action)
        {
            this.settlement = Traverse.Create((object)action).Field("settlement").GetValue<Settlement>();
            this.arrivalMode = Traverse.Create((object)action).Field("arrivalMode").GetValue<PawnsArrivalModeDef>();
        }

        public MagicTransportPodsArrivalAction_AttackSettlement(
          Settlement settlement,
          PawnsArrivalModeDef arrivalMode)
        {
            this.settlement = settlement;
            this.arrivalMode = arrivalMode;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Settlement>(ref this.settlement, "settlementA", false);
            Scribe_Defs.Look<PawnsArrivalModeDef>(ref this.arrivalMode, "arrivalModeA");
        }

        public override FloatMenuAcceptanceReport StillValid(
          IEnumerable<IThingHolder> pods,
          PlanetTile destinationTile)
        {
            FloatMenuAcceptanceReport acceptanceReport = base.StillValid(pods, destinationTile);
            if (!(bool)acceptanceReport)
                return acceptanceReport;
            return this.settlement != null && this.settlement.Tile != destinationTile ? (FloatMenuAcceptanceReport)false : MagicTransportPodsArrivalAction_AttackSettlement.CanAttack(pods, this.settlement);
        }

        public override bool ShouldUseLongEvent(List<ActiveTransporterInfo> pods, PlanetTile tile)
        {
            return !this.settlement.HasMap;
        }

        public override void Arrived(List<ActiveTransporterInfo> transporters, PlanetTile tile)
        {
            Thing lookTarget = MagicTransportPodUtility.GetLookTarget(transporters);
            int num = !this.settlement.HasMap ? 1 : 0;
            Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(this.settlement.Tile, (WorldObjectDef)null, (IEnumerable<GenStepWithParams>)null);
            TaggedString letterLabel = "LetterLabelCaravanEnteredEnemyBase".Translate();
            TaggedString letterText = "LetterTransportPodsLandedInEnemyBase".Translate((NamedArgument)this.settlement.Label).CapitalizeFirst();
            SettlementUtility.AffectRelationsOnAttacked((MapParent)this.settlement, ref letterText);
            if (num != 0)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter((IEnumerable<Pawn>)orGenerateMap.mapPawns.AllPawns, ref letterLabel, ref letterText, (string)"LetterRelatedPawnsInMapWherePlayerLanded".Translate((NamedArgument)Faction.OfPlayer.def.pawnsPlural), true, true);
            }
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent, (LookTargets)lookTarget, this.settlement.Faction, (Quest)null, (List<ThingDef>)null, (string)null, 0, true);

            this.TravelingTransportPodsArrived(transporters, orGenerateMap);
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
            MagicTransportPodUtility.DropTravellingDropPods(dropPods, spot, map);
        }

        private void TravelingTransportPodsArrivedEdge(List<ActiveTransporterInfo> dropPods, Map map)
        {
            IntVec3 dropCenterDistant = DropCellFinder.FindRaidDropCenterDistant(map, false);
            MagicTransportPodUtility.DropTravellingDropPods(dropPods, dropCenterDistant, map);
        }

        public static FloatMenuAcceptanceReport CanAttack(
          IEnumerable<IThingHolder> pods,
          Settlement settlement)
        {
            if (settlement == null || !settlement.Spawned || !settlement.Attackable)
                return (FloatMenuAcceptanceReport)false;
            if (!TransportersArrivalActionUtility.AnyNonDownedColonist(pods))
                return (FloatMenuAcceptanceReport)false;
            return settlement.EnterCooldownBlocksEntering() ? FloatMenuAcceptanceReport.WithFailReasonAndMessage((string)"EnterCooldownBlocksEntering".Translate(), (string)"MessageEnterCooldownBlocksEntering".Translate((NamedArgument)settlement.EnterCooldownTicksLeft().ToStringTicksToPeriod(true, false, true, true, false))) : (FloatMenuAcceptanceReport)true;
        }

        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(
          Action<PlanetTile, TransportersArrivalAction> launchAction,
          IEnumerable<IThingHolder> pods,
          Settlement settlement)
        {
            foreach (FloatMenuOption floatMenuOption in TransportersArrivalActionUtility.GetFloatMenuOptions<MagicTransportPodsArrivalAction_AttackSettlement>((Func<FloatMenuAcceptanceReport>)(() => MagicTransportPodsArrivalAction_AttackSettlement.CanAttack(pods, settlement)), (Func<MagicTransportPodsArrivalAction_AttackSettlement>)(() => new MagicTransportPodsArrivalAction_AttackSettlement(settlement, PawnsArrivalModeDefOf.EdgeDrop)), (string)"AttackAndDropAtEdge".Translate((NamedArgument)settlement.Label), launchAction, settlement.Tile, (Action<Action>)null))
                yield return floatMenuOption;
            foreach (FloatMenuOption floatMenuOption in TransportersArrivalActionUtility.GetFloatMenuOptions<MagicTransportPodsArrivalAction_AttackSettlement>((Func<FloatMenuAcceptanceReport>)(() => MagicTransportPodsArrivalAction_AttackSettlement.CanAttack(pods, settlement)), (Func<MagicTransportPodsArrivalAction_AttackSettlement>)(() => new MagicTransportPodsArrivalAction_AttackSettlement(settlement, PawnsArrivalModeDefOf.CenterDrop)), (string)"AttackAndDropInCenter".Translate((NamedArgument)settlement.Label), launchAction, settlement.Tile, (Action<Action>)null))
                yield return floatMenuOption;
        }
    }
}
