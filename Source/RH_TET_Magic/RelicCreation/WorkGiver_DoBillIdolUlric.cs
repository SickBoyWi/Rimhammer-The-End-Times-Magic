using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class WorkGiver_DoBillIdolUlric : WorkGiver_DoBillIdolBase
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            bool shouldSkip = this.ShouldSkipCore(pawn, forced);

            if (!shouldSkip)
            {
                if (!MagicUtility.IsFaithUserUlric(pawn))
                {
                    shouldSkip = true;
                }
                else if (pawn.needs.TryGetNeed<Need_FaithPool>() != null & pawn.needs.TryGetNeed<Need_FaithPool>().CurLevel < .01)
                {
                    shouldSkip = true;
                }
            }

            return shouldSkip;
        }
    }
}
