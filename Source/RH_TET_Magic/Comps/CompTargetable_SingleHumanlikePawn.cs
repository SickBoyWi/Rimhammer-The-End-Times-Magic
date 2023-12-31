using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompTargetable_SingleHumanlikePawn : CompTargetable
    {
        protected override bool PlayerChoosesTarget
        {
            get
            {
                return true;
            }
        }

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters()
            {
                canTargetPawns = true,
                canTargetBuildings = false,
                validator = (Predicate<TargetInfo>)(x => this.TargetValidator(x.Thing))
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }

        public bool TargetValidator(Thing t)
        {
            Pawn pawn = t as Pawn;
            if (pawn == null || pawn.Faction != Faction.OfPlayer || !pawn.RaceProps.Humanlike)
                return false;
            return true;
        }
    }
}
