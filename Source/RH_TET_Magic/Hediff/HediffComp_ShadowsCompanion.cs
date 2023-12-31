using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class HediffComp_ShadowsCompanion : HediffComp
    {
        private bool initializing;

        public string labelCap
        {
            get
            {
                return (TaggedString)this.Def.LabelCap;
            }
        }

        public string label
        {
            get
            {
                return this.Def.label;
            }
        }

        private void Initialize()
        {
            if (Pawn == null || !Pawn.Spawned || Pawn.Map == null)
                return;
            FleckMaker.ThrowSmoke(GenThing.TrueCenter(Pawn), Pawn.Map, 2f);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.Pawn != null && this.initializing)
            {
                this.initializing = false;
                this.Initialize();
            }
            if (Find.TickManager.TicksGame % 30 == 0)
            {
                if (Pawn.CurJob != null && (Pawn.CurJob.targetA != null) && ((LocalTargetInfo)Pawn.CurJob.targetA).Thing is Pawn)
                    severityAdjustment -= 20f;
                Effecter effecter = RH_TET_MagicDefOf.RH_TET_ShadowsCompanion_Effecter.Spawn();
                effecter.Trigger(new TargetInfo(Pawn.Position, Pawn.Map, false), new TargetInfo(Pawn.Position, Pawn.Map, false));
                effecter.Cleanup();
            }
            if (Find.TickManager.TicksGame % 60 != 0)
                return;
            --severityAdjustment;
        }

        public HediffComp_ShadowsCompanion()
            : base()
        {
        }
    }
}
