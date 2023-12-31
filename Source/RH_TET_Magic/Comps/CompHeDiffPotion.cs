using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompHeDiffPotion : CompUsable
    {
        public HediffDef hediff;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look<HediffDef>(ref this.hediff, "hediff");
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            CompProperties_HeDiffPotion propertiesHeDiff = (CompProperties_HeDiffPotion)props;
            this.hediff = propertiesHeDiff.hediff;
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
                return false;
            CompHeDiffPotion comp = other.TryGetComp<CompHeDiffPotion>();
            return comp != null && comp.hediff == this.hediff;
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompHeDiffPotion comp = piece.TryGetComp<CompHeDiffPotion>();
            if (comp == null)
                return;
            comp.hediff = this.hediff;
        }
    }
}
