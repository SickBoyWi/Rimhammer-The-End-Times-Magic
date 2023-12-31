using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class Alert_ShieldUserIsMagicUser : Alert
    {
        private List<Pawn> shieldUsersWithMagicAbilitiesResult = new List<Pawn>();

        private List<Pawn> ShieldUsersAreMagicUsers
        {
            get
            {
                this.shieldUsersWithMagicAbilitiesResult.Clear();
                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    if (MagicUtility.IsMagicUser(pawn) || MagicUtility.IsFaithUser(pawn))
                    {
                        List<Apparel> wornApparel = pawn.apparel.WornApparel;
                        for (int index = 0; index < wornApparel.Count; ++index)
                        {
                            Apparel currApparel = wornApparel[index];
                            if (currApparel.def.IsShieldThatBlocksRanged || this.IsShieldThatBlocksRanged(currApparel))
                            {
                                this.shieldUsersWithMagicAbilitiesResult.Add(pawn);
                                break;
                            }
                        }
                    }
                }
                return this.shieldUsersWithMagicAbilitiesResult;
            }
        }

        private bool IsShieldThatBlocksRanged (Apparel currApparel)
        {
            if (currApparel.GetComp<CompShieldKhorne>() != null)
                return true;
            return false;
        }

        public Alert_ShieldUserIsMagicUser()
        {
            this.defaultLabel = (string)"RH_TET_ShieldUserIsMagicUser".Translate();
            this.defaultExplanation = (string)"RH_TET_ShieldUserIsMagicUserDesc".Translate();
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(this.ShieldUsersAreMagicUsers);
        }
    }
}
