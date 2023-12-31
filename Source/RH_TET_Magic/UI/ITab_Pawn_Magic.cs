using RimWorld;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class ITab_Pawn_Magic : ITab
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
                if (this.SelPawn.story == null || !MagicUtility.IsMagicUser(this.SelPawn))
                    return false;
                return true;
            }
        }

        public ITab_Pawn_Magic()
        {
            this.size = MagicCardUtility.MagicCardSize + (new Vector2(17f, 17f) * 2f);
            this.labelKey = "RH_TET_TabMagic";
        }

        protected override void FillTab()
        {
            MagicCardUtility.DrawMagicCard(new Rect(17f, 17f, (float)MagicCardUtility.MagicCardSize.x, (float)MagicCardUtility.MagicCardSize.y), this.PawnToShowInfoAbout);
        }
    }
}
