using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private PlanetTile initialTile = PlanetTile.Invalid;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<ActiveTransporterInfo>(ref this.pods, "podsA", LookMode.Deep, (object[])Array.Empty<object>());
            Scribe_Values.Look<PlanetTile>(ref this.destinationTile, "destinationTileA", new PlanetTile(), false);
            Scribe_Values.Look<bool>(ref this.arrived, "arrivedA", false, false);
            Scribe_Values.Look<bool>(ref this.draftFlag, "draftFlagA", false, false);
            Scribe_Values.Look<Vector3>(ref this.startCell, "startCellA", new Vector3(), false);
            Scribe_Values.Look<PlanetTile>(ref this.initialTile, "initialTileA", new PlanetTile(), false);
            Scribe_Values.Look<float>(ref this.traveledPct, "traveledPctA", 0.0f, false);
            Scribe_Values.Look<Vector3>(ref this.endCell, "endCellA", new Vector3(), false);
            Scribe_Values.Look<IntVec3>(ref this.destinationCell, "destinationCellA", new IntVec3(), false);
            Scribe_Deep.Look<TransportersArrivalAction>(ref this.arrivalAction, "arrivalActionA", (object[])Array.Empty<object>());
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            for (int index = 0; index < this.pods.Count; ++index)
                this.pods[index].parent = (IThingHolder)this;
        }

        private Vector3 Start
        {
            get
            {
                return Find.WorldGrid.GetTileCenter(this.initialTile);
            }
        }

        private Vector3 End
        {
            get
            {
                return Find.WorldGrid.GetTileCenter(this.destinationTile);
            }
        }

        public override Vector3 DrawPos
        {
            get
            {
                return Vector3.Slerp(this.Start, this.End, this.traveledPct);
            }
        }

        public override bool ExpandingIconFlipHorizontal
        {
            get
            {
                return (double)GenWorldUI.WorldToUIPosition(this.Start).x > (double)GenWorldUI.WorldToUIPosition(this.End).x;
            }
        }

        public override float ExpandingIconRotation
        {
            get
            {
                if (!this.def.rotateGraphicWhenTraveling)
                    return base.ExpandingIconRotation;
                Vector2 uiPosition1 = GenWorldUI.WorldToUIPosition(this.Start);
                Vector2 uiPosition2 = GenWorldUI.WorldToUIPosition(this.End);
                float num = Mathf.Atan2(uiPosition2.y - uiPosition1.y, uiPosition2.x - uiPosition1.x) * 57.29578f;
                if ((double)num > 180.0)
                    num -= 180f;
                return num + 90f;
            }
        }

        private float TraveledPctStepPerTick
        {
            get
            {
                Vector3 start = this.Start;
                Vector3 end = this.End;
                if (start == end)
                    return 1f;
                float num = GenMath.SphericalDistance(start.normalized, end.normalized);
                return (double)num == 0.0 ? 1f : 0.00025f / num;
            }
        }

        private bool PodsHaveAnyPotentialCaravanOwner
        {
            get
            {
                for (int index1 = 0; index1 < this.pods.Count; ++index1)
                {
                    ThingOwner innerContainer = this.pods[index1].innerContainer;
                    for (int index2 = 0; index2 < innerContainer.Count; ++index2)
                    {
                        if (innerContainer[index2] is Pawn pawn && CaravanUtility.IsOwner(pawn, this.Faction))
                            return true;
                    }
                }
                return false;
            }
        }

        public new bool PodsHaveAnyFreeColonist
        {
            get
            {
                for (int index1 = 0; index1 < this.pods.Count; ++index1)
                {
                    ThingOwner innerContainer = this.pods[index1].innerContainer;
                    for (int index2 = 0; index2 < innerContainer.Count; ++index2)
                    {
                        if (innerContainer[index2] is Pawn pawn && pawn.IsColonist && pawn.HostFaction == null)
                            return true;
                    }
                }
                return false;
            }
        }

        public new IEnumerable<Pawn> Pawns
        {
            get
            {
                for (int i = 0; i < this.pods.Count; ++i)
                {
                    ThingOwner things = this.pods[i].innerContainer;
                    for (int j = 0; j < things.Count; ++j)
                    {
                        if (things[j] is Pawn pawn)
                            yield return pawn;
                    }
                    things = (ThingOwner)null;
                }
            }
        }
        public new bool ContainsPawn(Pawn p)
        {
            for (int index = 0; index < this.pods.Count; ++index)
            {
                if (this.pods[index].innerContainer.Contains((Thing)p))
                    return true;
            }
            return false;
        }

        public override void PostAdd()
        {
            base.PostAdd();
            this.initialTile = this.Tile;
        }

        protected override void TickInterval(int delta)
        {
            //base.TickInterval(delta);

            for (int index = 0; index < this.AllComps.Count; ++index)
                this.AllComps[index].CompTickInterval(delta);

            this.traveledPct += this.TraveledPctStepPerTick * (float)delta;
            if ((double)this.traveledPct < 1.0)
                return;
            this.traveledPct = 1f;
            this.Arrived();
        }

        public new void AddTransporter(ActiveTransporterInfo contents, bool justLeftTheMap)
        {
            contents.parent = (IThingHolder)this;
            this.pods.Add(contents);
            ThingOwner innerContainer = contents.innerContainer;
            for (int index = 0; index < innerContainer.Count; ++index)
            {
                if (innerContainer[index] is Pawn pawn && !pawn.IsWorldPawn())
                {
                    if (!this.Spawned)
                        Log.Warning(string.Format("Passing pawn {0} to world, but the TravelingMagicTransportPod is not spawned. This means that WorldPawns can discard this pawn which can cause bugs.", (object)pawn));
                    if (justLeftTheMap)
                        pawn.ExitMap(false, Rot4.Invalid);
                    else
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                }
            }
            contents.savePawnsWithReferenceMode = true;
        }

        public new ThingOwner GetDirectlyHeldThings()
        {
            return (ThingOwner)null;
        }

        public new void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, (IList<Thing>)this.GetDirectlyHeldThings());
            for (int index = 0; index < this.pods.Count; ++index)
                outChildren.Add((IThingHolder)this.pods[index]);
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
                            // DIFF HERE on PARMS TO Construtor.
                            this.arrivalAction = (TransportersArrivalAction)new MagicTransportPodsArrivalAction_LandInSpecificCell(maps[index].Parent, DropCellFinder.RandomDropSpot(maps[index], true));
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
                Map tempMap = null;
                MapParent mp = null;
                for (int index = 0; index < maps.Count; ++index)
                {
                    if (maps[index].Tile == this.destinationTile)
                    {
                        if (this.destinationCell != new IntVec3())
                        {
                            mp = maps[index].Parent;
                            tempMap = maps[index]; 
                            break;
                        }
                        break;
                    }
                }

                if (mp != null)
                    this.arrivalAction = (TransportersArrivalAction)new MagicTransportPodsArrivalAction_LandInSpecificCell(mp, DropCellFinder.RandomDropSpot(tempMap, true));
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
                bool flag = true;
                if (ModsConfig.BiotechActive)
                {
                    flag = false;
                    for (int index1 = 0; index1 < this.pods.Count && !flag; ++index1)
                    {
                        for (int index2 = 0; index2 < this.pods[index1].innerContainer.Count; ++index2)
                        {
                            if (this.pods[index1].innerContainer[index2].def != ThingDefOf.Wastepack)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                for (int index1 = 0; index1 < this.pods.Count; ++index1)
                {
                    for (int index2 = 0; index2 < this.pods[index1].innerContainer.Count; ++index2)
                        this.pods[index1].innerContainer[index2].Notify_AbandonedAtTile(this.destinationTile);
                }
                for (int index = 0; index < this.pods.Count; ++index)
                    this.pods[index].innerContainer.ClearAndDestroyContentsOrPassToWorld(DestroyMode.Vanish);
                if (flag)
                {
                    string key = "MessageTransportPodsArrivedAndLost";
                    if (this.def == WorldObjectDefOf.TravelingShuttle)
                        key = "MessageShuttleArrivedContentsLost";
                    Messages.Message((string)key.Translate(), (LookTargets)new GlobalTargetInfo(this.destinationTile), MessageTypeDefOf.NegativeEvent, true);
                }
            }
            this.pods.Clear();
            this.Destroy();
        }
    }
}
