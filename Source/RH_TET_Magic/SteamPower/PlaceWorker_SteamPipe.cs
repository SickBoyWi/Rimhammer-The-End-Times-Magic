using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class PlaceWorker_SteamPipe : PlaceWorker
    {
        public override bool ForceAllowPlaceOver(BuildableDef other)
        {
            return true;
        }

        public override AcceptanceReport AllowsPlacing(
          BuildableDef def,
          IntVec3 loc,
          Rot4 rot,
          Map map,
          Thing thingToIgnore = null,
          Thing t = null)
        {
            ThingDef thingDef = def as ThingDef;
            CompProperties_SteamPipe pipe = thingDef.GetCompProperties<CompProperties_SteamPipe>();
            if (map.PipeNet().ZoneAt(loc, pipe.mode))
                return new AcceptanceReport("Steam pipe already exists here");
            List<Thing> thingList = loc.GetThingList(map);
            for (int index = 0; index < thingList.Count; ++index)
            {
                if (thingList[index].def != null && thingList[index].def.comps.OfType<CompProperties_SteamPipe>().Any<CompProperties_SteamPipe>((Func<CompProperties_SteamPipe, bool>)(x => x.mode == pipe.mode)))
                    return new AcceptanceReport("Steam pipe already exists here.");
                if (thingList[index].def != null && thingList[index].def.entityDefToBuild != null && (thingList[index].def.entityDefToBuild is ThingDef entityDefToBuild && thingDef != entityDefToBuild) && entityDefToBuild.comps.OfType<CompProperties_SteamPipe>().Any<CompProperties_SteamPipe>((Func<CompProperties_SteamPipe, bool>)(x => x.mode == pipe.mode)))
                    return new AcceptanceReport("Steam pipe already exists here");
            }
            return (AcceptanceReport)true;
        }
    }
}
