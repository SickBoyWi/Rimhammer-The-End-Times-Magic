using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class TravelingMagicTransportPods : TravellingTransporters
    {
        public IntVec3 destinationCell = new IntVec3();
        public bool draftFlag = false;
        private List<ActiveTransporterInfo> pods = new List<ActiveTransporterInfo>();
        public float TravelSpeed = 0.0003f;
        private bool initialized = false;
        private bool arrived;
        private float traveledPct;
        private Vector3 startCell;
        private Vector3 endCell;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<ActiveTransporterInfo>(ref this.pods, "podsA", LookMode.Deep, (object[])Array.Empty<object>());
            Scribe_Values.Look<bool>(ref this.arrived, "arrivedA", false, false);
            Scribe_Values.Look<bool>(ref this.draftFlag, "draftFlagA", false, false);
            Scribe_Values.Look<float>(ref this.traveledPct, "traveledPctA", 0.0f, false);
            Scribe_Values.Look<Vector3>(ref this.startCell, "startCellA", new Vector3(), false);
            Scribe_Values.Look<Vector3>(ref this.endCell, "endCellA", new Vector3(), false);
            Scribe_Values.Look<IntVec3>(ref this.destinationCell, "destinationCellA", new IntVec3(), false);

            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            for (int index = 0; index < this.pods.Count; ++index)
                this.pods[index].parent = (IThingHolder)this;
        }

        private float TraveledPctStepPerTick
        {
            get
            {
                Vector3 tileCenter1 = Find.WorldGrid.GetTileCenter(Traverse.Create((object)this).Field("initialTile").GetValue<int>());
                startCell = tileCenter1;
                Vector3 tileCenter2 = Find.WorldGrid.GetTileCenter(Traverse.Create((object)this).Field("destinationTile").GetValue<int>());
                endCell = tileCenter2;
                if (tileCenter1 == tileCenter2)
                    return 1f;
                float num = GenMath.SphericalDistance((tileCenter1).normalized, tileCenter2.normalized);
                return (double)num == 0.0 ? 1f : this.TravelSpeed / num;
            }
        }

        protected override void Tick()
        {
            for (int index = 0; index < this.AllComps.Count; ++index)
                this.AllComps[index].CompTick();
            this.traveledPct += this.TraveledPctStepPerTick;
            if ((double)this.traveledPct < 1.0)
                return;
            this.traveledPct = 1f;
            this.Arrived();
        }

        public override Vector3 DrawPos
        {
            get
            {
                return Vector3.Slerp(this.startCell, this.endCell, this.traveledPct);
            }
        }

        private void Arrived()
        {
            if (this.arrived)
                return;
            this.arrived = true;
            bool setToAttack = false;

            if (this.arrivalAction != null && this.arrivalAction is TransportersArrivalAction_AttackSettlement)
            {
                setToAttack = true;
            }

            if (this.arrivalAction is TransportersArrivalAction_LandInSpecificCell)
            {
                TransportersArrivalAction_LandInSpecificCell actionIn = (TransportersArrivalAction_LandInSpecificCell)this.arrivalAction;
                this.destinationCell = Traverse.Create((object)actionIn).Field("cell").GetValue<IntVec3>();
            }

            if (this.arrivalAction == null || !(bool)this.arrivalAction.StillValid(this.pods.Cast<IThingHolder>(), this.destinationTile))
            {
                this.arrivalAction = null;
                List<Map> maps = Find.Maps;
                for (int index = 0; index < maps.Count; ++index)
                {
                    if (maps[index].Tile == this.destinationTile)
                    {
                        if (this.destinationCell != new IntVec3())
                        {
                            this.arrivalAction = (TransportersArrivalAction)new MagicTransportPodsArrivalAction_LandInSpecificCell(maps[index].Parent, this.destinationCell, this.draftFlag);
                            break;
                        }
                        this.arrivalAction = (TransportersArrivalAction)new TransportersArrivalAction_LandInSpecificCell(maps[index].Parent, DropCellFinder.RandomDropSpot(maps[index], true));
                        break;
                    }
                }
                if (this.arrivalAction == null)
                {
                    if (TransportersArrivalAction_FormCaravan.CanFormCaravanAt(this.pods.Cast<IThingHolder>(), this.destinationTile))
                    {
                        this.arrivalAction = (TransportersArrivalAction)new TransportersArrivalAction_FormCaravan();
                    }
                    else
                    {
                        List<Caravan> caravans = Find.WorldObjects.Caravans;
                        for (int index = 0; index < caravans.Count; ++index)
                        {
                            if (caravans[index].Tile == this.destinationTile && (bool)TransportersArrivalAction_GiveToCaravan.CanGiveTo(this.pods.Cast<IThingHolder>(), caravans[index]))
                            {
                                this.arrivalAction = (TransportersArrivalAction)new TransportersArrivalAction_GiveToCaravan(caravans[index]);
                                break;
                            }
                        }
                    }
                }
            }

            if (this.arrivalAction is TransportersArrivalAction_LandInSpecificCell)
            {
                List<Map> maps = Find.Maps;
                MapParent mp = null;
                for (int index = 0; index < maps.Count; ++index)
                {
                    if (maps[index].Tile == this.destinationTile)
                    {
                        if (this.destinationCell != new IntVec3())
                        {
                            mp = maps[index].Parent;
                            break;
                        }
                        break;
                    }
                }

                if (mp != null)
                    this.arrivalAction = (TransportersArrivalAction)new MagicTransportPodsArrivalAction_LandInSpecificCell(mp, this.destinationCell, this.draftFlag);
            }

            if (setToAttack)
            {
                this.arrivalAction = new MagicTransportPodsArrivalAction_AttackSettlement((TransportersArrivalAction_AttackSettlement)this.arrivalAction);
            }

            if (this.arrivalAction != null && (this.arrivalAction.ShouldUseLongEvent(this.pods, this.destinationTile) || this.arrivalAction is MagicTransportPodsArrivalAction_AttackSettlement))
            {
                LongEventHandler.QueueLongEvent((Action)(() => this.DoArrivalAction()), "GeneratingMapForNewEncounter", false, (Action<Exception>)null, true);
            }
            else
            {
                this.DoArrivalAction();
            }
        }

        private void DoArrivalAction()
        {
            for (int index = 0; index < this.pods.Count; ++index)
            {
                this.pods[index].savePawnsWithReferenceMode = false;
                this.pods[index].parent = (IThingHolder)null;
            }
            if (this.arrivalAction != null)
            {
                try
                {
                    this.arrivalAction.Arrived(this.pods, this.destinationTile);
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in magic transport pods arrival action: " + (object)ex);
                }
                this.arrivalAction = (TransportersArrivalAction)null;
            }
            else
            {
                for (int index1 = 0; index1 < this.pods.Count; ++index1)
                {
                    for (int index2 = 0; index2 < this.pods[index1].innerContainer.Count; ++index2)
                    {
                        if (this.pods[index1].innerContainer[index2] is Pawn pawn && (pawn.Faction == Faction.OfPlayer || pawn.HostFaction == Faction.OfPlayer))
                            PawnBanishUtility.Banish(pawn, this.destinationTile);
                    }
                }
                for (int index = 0; index < this.pods.Count; ++index)
                    this.pods[index].innerContainer.ClearAndDestroyContentsOrPassToWorld(DestroyMode.Vanish);
                Messages.Message((string)"MessageTransportPodsArrivedAndLost".Translate(), (LookTargets)new GlobalTargetInfo(this.destinationTile), MessageTypeDefOf.NegativeEvent, true);
            }
            this.pods.Clear();
            this.Destroy();
        }

        public new void AddPod(ActiveTransporterInfo contents, bool justLeftTheMap)
        {
            contents.parent = (IThingHolder)this;
            this.pods.Add(contents);
            ThingOwner innerContainer = contents.innerContainer;
            for (int index = 0; index < innerContainer.Count; ++index)
            {
                if (innerContainer[index] is Pawn pawn && !pawn.IsWorldPawn())
                {
                    if (!this.Spawned)
                        Log.Warning("Passing pawn " + (object)pawn + " to world, but the TravelingTransportPod is not spawned. This means that WorldPawns can discard this pawn which can cause bugs.");
                    if (justLeftTheMap)
                        pawn.ExitMap(false, Rot4.Invalid);
                    else
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                }
            }
            contents.savePawnsWithReferenceMode = true;
        }
    }
}
