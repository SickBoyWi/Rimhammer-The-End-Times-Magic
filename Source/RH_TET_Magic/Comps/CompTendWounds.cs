using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Magic
{
    public class CompTendWounds : ThingComp
    {
        public int healIntervalTicks = 60;
        public int ticksSinceHeal;


        private static readonly FloatRange AwfulQualityTendQualtiyRange = new FloatRange(0.10f, 0.24f);
        private static readonly FloatRange PoorQualityTendQualtiyRange = new FloatRange(0.25f, 0.39f);
        private static readonly FloatRange NormalQualityTendQualtiyRange = new FloatRange(0.40f, 0.54f);
        private static readonly FloatRange GoodQualityTendQualtiyRange = new FloatRange(0.55f, 0.69f);
        private static readonly FloatRange ExcellentQualityTendQualtiyRange = new FloatRange(0.70f, 0.79f);
        private static readonly FloatRange MasterworkQualityTendQualtiyRange = new FloatRange(0.80f, 094f);
        private static readonly FloatRange LegendaryQualityTendQualtiyRange = new FloatRange(.95f, 1.0f);

        public CompProperties_TendWounds Props
        {
            get
            {
                return (CompProperties_TendWounds)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.ticksSinceHeal, "ticksSinceHeal", 0, false);
        }

        public override void CompTick()
        {
            base.CompTick();

            Pawn wearer = WearerOf();

            if (wearer != null && !wearer.Dead)
            {
                ++this.ticksSinceHeal;
                if (this.ticksSinceHeal > this.healIntervalTicks && wearer.health.hediffSet.HasNaturallyHealingInjury())
                {
                    TryTendWounds(wearer);
                }
            }
        }

        public void TryTendWounds(Pawn p)
        {
            ticksSinceHeal = 0;

            IEnumerable<Hediff> hediffs = p.health.hediffSet.hediffs.Where<Hediff>((Func<Hediff, bool>)(hd => hd.TendableNow()));
            if (hediffs == null)
                return;

            foreach (Hediff hediff in hediffs)
            {
                hediff.Tended(GetTendQualityRange(), 1f, 0);
            }
        }

        private float GetTendQualityRange()
        {
            QualityCategory qc;
            this.parent.TryGetQuality(out qc);

            if (qc.Equals(QualityCategory.Awful))
                return AwfulQualityTendQualtiyRange.RandomInRange;
            else if (qc.Equals(QualityCategory.Poor))
                return PoorQualityTendQualtiyRange.RandomInRange;
            else if (qc.Equals(QualityCategory.Normal))
                return NormalQualityTendQualtiyRange.RandomInRange;
            else if (qc.Equals(QualityCategory.Good))
                return GoodQualityTendQualtiyRange.RandomInRange;
            else if (qc.Equals(QualityCategory.Excellent))
                return ExcellentQualityTendQualtiyRange.RandomInRange;
            else if (qc.Equals(QualityCategory.Masterwork))
                return MasterworkQualityTendQualtiyRange.RandomInRange;
            else if (qc.Equals(QualityCategory.Legendary))
                return LegendaryQualityTendQualtiyRange.RandomInRange;

            return 0.5f;
        }

        public Pawn WearerOf()
        {
            return this.ParentHolder is Pawn_ApparelTracker parentHolder ? parentHolder.pawn : (Pawn)null;
        }
    }
}
