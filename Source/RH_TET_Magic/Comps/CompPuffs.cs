using RimWorld;
using TheEndTimes_Magic;

namespace Verse
{
    public class CompPuffs : ThingComp
    {
        public CompProperties_Puffs Props
        {
            get
            {
                return (CompProperties_Puffs)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override void CompTick()
        {
            base.CompTick();
        }

        public override void CompTickRare()
        {
            // Toss a visual puff.
            FleckMaker.ThrowAirPuffUp(this.parent.Position.ToVector3(), this.parent.Map);
        }
    }
}
