using RimWorld;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class ITab_Pawn_Faith : ITab
    {
        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = (Pawn)null;
                if (this.SelPawn != null)
                {
                    pawn = this.SelPawn;
                }
                else
                {
                    Corpse selThing = this.SelThing as Corpse;
                    if (selThing != null)
                        pawn = selThing.InnerPawn;
                }
                if (pawn != null)
                    return pawn;
                Log.Error("Character tab found, but there is no selected pawn to display.", false);
                return (Pawn)null;
            }
        }

        public override bool IsVisible
        {
            get
            {
                if (this.SelPawn.story == null || !MagicUtility.IsFaithUser(this.SelPawn))
                    return false;
                return true;
            }
        }

        public ITab_Pawn_Faith()
        {
            this.size = FaithCardUtility.FaithCardSize + (new Vector2(17f, 17f) * 2f);
            this.labelKey = "RH_TET_TabFaith";
        }

        protected override void FillTab()
        {
            FaithCardUtility.DrawFaithCard(new Rect(17f, 17f, (float)FaithCardUtility.FaithCardSize.x, (float)FaithCardUtility.FaithCardSize.y), this.PawnToShowInfoAbout);
        }
    }
}
