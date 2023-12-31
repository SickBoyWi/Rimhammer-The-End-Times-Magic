using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class ThoughtWorker_MagicApparel : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            return (ThoughtState)ThoughtWorker_MagicApparel.HasWizardsApparel(p);
        }

        private static bool HasWizardsApparel(Pawn p)
        {
            if (p.gender == Gender.None)
                return false;

            List<Apparel> apparelWorn = p.apparel.WornApparel;

            foreach (Apparel apparel in apparelWorn)
            {
                if (apparel.def.defName.Contains("Wizard"))
                    return true;
            }

            return false;
        }
    }
}
