using HugsLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public static class MagicGasUtility
    {

        public static GasCloud TryFindGasCloudAt(Map map, IntVec3 pos, ThingDef matchingDef = null)
        {
            if (!pos.InBounds(map))
                return (GasCloud)null;
            List<Verse.Thing> thingList = map.thingGrid.ThingsListAtFast(map.cellIndices.CellToIndex(pos));
            for (int index = 0; index < thingList.Count; ++index)
            {
                if (thingList[index] is GasCloud gasCloud && (matchingDef == null || gasCloud.def == matchingDef))
                    return gasCloud;
            }
            return (GasCloud)null;
        }

        public static void DeployGas(Map map, IntVec3 pos, ThingDef gasDef, int amount)
        {
            if (gasDef == null)
            {
                Log.Error("Tried to deploy null gas ThingDef.");
            }
            else
            {
                GasCloud gasCloud = MagicGasUtility.TryFindGasCloudAt(map, pos, gasDef);
                if (gasCloud == null)
                {
                    if (!(ThingMaker.MakeThing(gasDef, (ThingDef)null) is GasCloud gasCloudT))
                    {
                        Log.Error(string.Format("Deployed thing was not a GasCloud: {0}", (object)gasDef));
                        return;
                    }
                    GenPlace.TryPlaceThing((Verse.Thing)gasCloud, pos, map, ThingPlaceMode.Direct, (Action<Verse.Thing, int>)null, (Predicate<IntVec3>)null, new Rot4());
                }
                gasCloud.ReceiveConcentration((float)amount);
            }
        }
    }
}