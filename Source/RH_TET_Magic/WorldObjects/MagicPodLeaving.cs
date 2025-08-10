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
                Log.Error(string.Format("Magic drop pod left the map, but its group ID is {0}", (object)this.groupID));
                this.Destroy(DestroyMode.Vanish);
            }
            else if (!this.destinationTile.Valid)
            {
                Log.Error(string.Format("Magic drop pod left the map, but its destination tile is {0}", (object)this.destinationTile));
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                Lord lord = TransporterUtility.FindLord(this.groupID, this.Map);
                if (lord != null)
                    this.Map.lordManager.RemoveLord(lord);

                TravelingMagicTransportPods travellingTransporters = (TravelingMagicTransportPods)WorldObjectMaker.MakeWorldObject(RH_TET_MagicDefOf.RH_TET_Magic_TravelingTransportPod);
                travellingTransporters.SetFaction(Faction.OfPlayer);
                travellingTransporters.destinationTile = this.destinationTile;

                


                travellingTransporters.arrivalAction = this.arrivalAction;




                PlanetTile other = this.Map.Tile;
                if (other.Layer != this.destinationTile.Layer)
                    other = this.destinationTile.Layer.GetClosestTile(other);
                travellingTransporters.Tile = other;

                Find.WorldObjects.Add((WorldObject)travellingTransporters);

                MagicPodLeaving.tmpActiveDropPods.Clear();
                MagicPodLeaving.tmpActiveDropPods.AddRange((IEnumerable<Thing>)this.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveTransporter));
                for (int index = 0; index < MagicPodLeaving.tmpActiveDropPods.Count; ++index)
                {
                    if (MagicPodLeaving.tmpActiveDropPods[index] is MagicPodLeaving activeTransporter && activeTransporter.groupID == this.groupID)
                    {
                        activeTransporter.alreadyLeft = true;
                        travellingTransporters.AddTransporter(activeTransporter.Contents, true);
                        activeTransporter.Contents = (ActiveTransporterInfo)null;
                        activeTransporter.Destroy(DestroyMode.Vanish);
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
