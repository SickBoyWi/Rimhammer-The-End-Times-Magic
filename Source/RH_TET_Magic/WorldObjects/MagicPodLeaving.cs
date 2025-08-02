using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class MagicPodLeaving : FlyShipLeaving
    {
        private static List<Thing> tmpActiveDropPods = new List<Thing>();
        public IntVec3 arrivalCell;
        public bool draftFlag;
        private bool alreadyLeft;

        protected override void LeaveMap()
        {
            if (this.alreadyLeft || !this.createWorldObject)
            {
                if (this.Contents != null)
                {
                    foreach (Thing thing in (IEnumerable<Thing>)this.Contents.innerContainer)
                    {
                        if (thing is Pawn pawn)
                            pawn.ExitMap(false, Rot4.Invalid);
                    }
                    this.Contents.innerContainer.ClearAndDestroyContentsOrPassToWorld(DestroyMode.QuestLogic);
                }
                base.LeaveMap();
            }
            else if (this.groupID < 0)
            {
                Log.Error("Drop pod left the map, but its group ID is " + (object)this.groupID);
                this.Destroy(DestroyMode.Vanish);
            }
            else if (this.destinationTile < 0)
            {
                Log.Error("Drop pod left the map, but its destination tile is " + (object)this.destinationTile);
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                Lord lord = TransporterUtility.FindLord(this.groupID, this.Map);
                if (lord != null)
                    this.Map.lordManager.RemoveLord(lord);
                TravelingMagicTransportPods travelingTransportPods = (TravelingMagicTransportPods)WorldObjectMaker.MakeWorldObject(RH_TET_MagicDefOf.RH_TET_Magic_TravelingTransportPod);
                travelingTransportPods.Tile = this.Map.Tile;
                travelingTransportPods.SetFaction(Faction.OfPlayer);
                travelingTransportPods.destinationTile = this.destinationTile;
                travelingTransportPods.arrivalAction = this.arrivalAction;
                //travelingTransportPods.arrivalAction = new MagicTransportPodsArrivalAction_LandInSpecificCell();
                Find.WorldObjects.Add((WorldObject)travelingTransportPods);
                MagicPodLeaving.tmpActiveDropPods.Clear();
                MagicPodLeaving.tmpActiveDropPods.AddRange((IEnumerable<Thing>)this.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveTransporter));
                for (int index = 0; index < MagicPodLeaving.tmpActiveDropPods.Count; ++index)
                {
                    if (MagicPodLeaving.tmpActiveDropPods[index] is MagicPodLeaving tmpActiveDropPod && tmpActiveDropPod.groupID == this.groupID)
                    {
                        tmpActiveDropPod.alreadyLeft = true;
                        travelingTransportPods.AddPod(tmpActiveDropPod.Contents, true);
                        tmpActiveDropPod.Contents = (ActiveTransporterInfo)null;
                        tmpActiveDropPod.Destroy(DestroyMode.Vanish);
                    }
                }
            }
        }

        public MagicPodLeaving()
            : base()
        {
        }
    }
}
