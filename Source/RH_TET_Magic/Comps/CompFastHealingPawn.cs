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
    public class CompFastHealingPawn : ThingComp
    {
        public int healIntervalTicks = 60;
        public int ticksSinceHeal;

        public CompProperties_FastHealingPawn Props
        {
            get
            {
                return (CompProperties_FastHealingPawn)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.ticksSinceHeal, "ticksSinceHeal", 0, false);
        }

        public override void CompTick()
        {
            if (!((Pawn)this.parent).Dead)
            {
                base.CompTick();
                ++this.ticksSinceHeal;
                if (this.ticksSinceHeal > this.healIntervalTicks && ((Pawn)this.parent).health.hediffSet.HasNaturallyHealingInjury())
                {
                    this.ticksSinceHeal = 0;

                    List<Hediff_Injury> resultHediffs = new List<Hediff_Injury>();
                    ((Pawn)this.parent).health.hediffSet.GetHediffs<Hediff_Injury>(ref resultHediffs, (Predicate<Hediff_Injury>)(x => x.CanHealNaturally()));

                    resultHediffs.RandomElement<Hediff_Injury>().Heal((float)(8.0 * (double)((Pawn)this.parent).HealthScale * 0.00999999977648258));
                }
            }
        }

        public void TrySealWounds()
        {
            IEnumerable<Hediff> hediffs = ((Pawn)this.parent).health.hediffSet.hediffs.Where<Hediff>((Func<Hediff, bool>)(hd => hd.Bleeding));
            if (hediffs == null)
                return;
            foreach (Hediff hediff in hediffs)
            {
                if (hediff is HediffWithComps hd)
                {
                    HediffComp_TendDuration comp = hd.TryGetComp<HediffComp_TendDuration>();
                    comp.tendQuality = 2f;
                    comp.tendTicksLeft = Find.TickManager.TicksGame;
                    ((Pawn)this.parent).health.Notify_HediffChanged(hediff);
                }
            }
        }
        public static void doClot(Pawn pawn, BodyPartRecord part = null)
        {
            int num = 5;
            foreach (Hediff hediff in (IEnumerable<Hediff>)pawn.health.hediffSet.hediffs.Where<Hediff>((Func<Hediff, bool>)(x => x.Bleeding)).OrderByDescending<Hediff, float>((Func<Hediff, float>)(x => x.BleedRate)))
            {
                Rand.PushState();
                if (Rand.ChanceSeeded(0.25f, 463327381))
                    hediff.Tended(Math.Min(Rand.Value + Rand.Value + Rand.Value, 1f), 1f, 0);
                Rand.PopState();
                --num;
                if (num <= 0)
                    break;
            }
        }
    }
}
