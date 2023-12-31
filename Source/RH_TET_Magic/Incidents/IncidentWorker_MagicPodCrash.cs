using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class IncidentWorker_MagicPodCrash : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            List<Thing> thingList = ThingSetMakerDefOf.ResourcePod.root.Generate();
            IntVec3 intVec3 = DropCellFinder.RandomDropSpot(target);
            MagicTransportPodUtility.DropThingsNear(intVec3, target, (IEnumerable<Thing>)thingList, 110, false, true, true, true);
            this.SendStandardLetter("RH_TET_LetterLabelMagicPodCrash".Translate(), "RH_TET_MagicPodCrash".Translate(), LetterDefOf.PositiveEvent, parms, (LookTargets)new TargetInfo(intVec3, target, false), (NamedArgument[])Array.Empty<NamedArgument>());
            return true;
        }
    }
}
