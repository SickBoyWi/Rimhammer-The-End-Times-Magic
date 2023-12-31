using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

// Taken from vanilla code base. Tweaked a bit to not toss errors, and not work on permanent injuries.
namespace TheEndTimes_Magic
{
    public class CompUseEffect_FixHealthConditionRH : CompUseEffect
    {
        private float HandCoverageAbsWithChildren
        {
            get
            {
                return ThingDefOf.Human.race.body.GetPartsWithDef(BodyPartDefOf.Hand).First<BodyPartRecord>().coverageAbsWithChildren;
            }
        }

        public override void DoEffect(Pawn usedBy)
        {
            // Core vanilla code from base class.
            if (usedBy.Map != Find.CurrentMap)
                return;

            // Core vanilla code.
            Hediff threateningHediff = this.FindLifeThreateningHediff(usedBy);
            if (threateningHediff != null)
            {
                this.Cure(threateningHediff);
            }
            else
            {
                if (HealthUtility.TicksUntilDeathDueToBloodLoss(usedBy) < 2500)
                {
                    Hediff mostBleedingHediff = this.FindMostBleedingHediff(usedBy);
                    if (mostBleedingHediff != null)
                    {
                        this.Cure(mostBleedingHediff);
                        return;
                    }
                }
                if (usedBy.health.hediffSet.GetBrain() != null)
                {
                    Hediff_Injury permanentInjury = this.FindPermanentInjury(usedBy, Gen.YieldSingle<BodyPartRecord>(usedBy.health.hediffSet.GetBrain()));
                    if (permanentInjury != null)
                    {
                        this.Cure((Hediff)permanentInjury);
                        return;
                    }
                }
                {
                    Hediff_Injury permanentInjury1 = this.FindPermanentInjury(usedBy, usedBy.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, (BodyPartTagDef)null, (BodyPartRecord)null).Where<BodyPartRecord>((Func<BodyPartRecord, bool>)(x => x.def == BodyPartDefOf.Eye)));
                    if (permanentInjury1 != null)
                    {
                        this.Cure((Hediff)permanentInjury1);
                    }
                    else
                    {
                        Hediff hediffWhichCanKill = this.FindImmunizableHediffWhichCanKill(usedBy);
                        if (hediffWhichCanKill != null)
                        {
                            this.Cure(hediffWhichCanKill);
                        }
                        else
                        {
                            Hediff injuryMiscBadHediff1 = this.FindNonInjuryMiscBadHediff(usedBy, true);
                            if (injuryMiscBadHediff1 != null)
                            {
                                this.Cure(injuryMiscBadHediff1);
                            }
                            else
                            {
                                Hediff injuryMiscBadHediff2 = this.FindNonInjuryMiscBadHediff(usedBy, false);
                                if (injuryMiscBadHediff2 != null)
                                {
                                    this.Cure(injuryMiscBadHediff2);
                                }
                                else
                                {
                                    if (usedBy.health.hediffSet.GetBrain() != null)
                                    {
                                        Hediff_Injury injury = this.FindInjury(usedBy, Gen.YieldSingle<BodyPartRecord>(usedBy.health.hediffSet.GetBrain()));
                                        if (injury != null)
                                        {
                                            this.Cure((Hediff)injury);
                                            return;
                                        }
                                    }
                                    {
                                        Hediff_Addiction addiction = this.FindAddiction(usedBy);
                                        if (addiction != null)
                                        {
                                            this.Cure((Hediff)addiction);
                                        }
                                        else
                                        {
                                            Hediff_Injury permanentInjury2 = this.FindPermanentInjury(usedBy, (IEnumerable<BodyPartRecord>)null);
                                            if (permanentInjury2 != null)
                                            {
                                                this.Cure((Hediff)permanentInjury2);
                                            }
                                            else
                                            {
                                                Hediff_Injury injury = this.FindInjury(usedBy, (IEnumerable<BodyPartRecord>)null);
                                                if (injury == null)
                                                    return;
                                                this.Cure((Hediff)injury);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private Hediff FindLifeThreateningHediff(Pawn pawn)
        {
            Hediff hediff = (Hediff)null;
            float num1 = -1f;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                if (hediffs[index].Visible && hediffs[index].def.everCurableByItem && !hediffs[index].FullyImmune())
                {
                    HediffStage curStage = hediffs[index].CurStage;
                    int num2 = curStage == null ? 0 : (curStage.lifeThreatening ? 1 : 0);
                    bool flag = (double)hediffs[index].def.lethalSeverity >= 0.0 && (double)hediffs[index].Severity / (double)hediffs[index].def.lethalSeverity >= 0.800000011920929;
                    if (num2 != 0 || flag)
                    {
                        float num3 = hediffs[index].Part != null ? hediffs[index].Part.coverageAbsWithChildren : 999f;
                        if (hediff == null || (double)num3 > (double)num1)
                        {
                            hediff = hediffs[index];
                            num1 = num3;
                        }
                    }
                }
            }
            return hediff;
        }

        private Hediff FindMostBleedingHediff(Pawn pawn)
        {
            float num = 0.0f;
            Hediff hediff = (Hediff)null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                if (hediffs[index].Visible && hediffs[index].def.everCurableByItem)
                {
                    float bleedRate = hediffs[index].BleedRate;
                    if ((double)bleedRate > 0.0 && ((double)bleedRate > (double)num || hediff == null))
                    {
                        num = bleedRate;
                        hediff = hediffs[index];
                    }
                }
            }
            return hediff;
        }

        private Hediff FindImmunizableHediffWhichCanKill(Pawn pawn)
        {
            Hediff hediff = (Hediff)null;
            float num = -1f;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                if (hediffs[index].Visible && hediffs[index].def.everCurableByItem && (hediffs[index].TryGetComp<HediffComp_Immunizable>() != null && !hediffs[index].FullyImmune()) && this.CanEverKill(hediffs[index]))
                {
                    float severity = hediffs[index].Severity;
                    if (hediff == null || (double)severity > (double)num)
                    {
                        hediff = hediffs[index];
                        num = severity;
                    }
                }
            }
            return hediff;
        }

        private Hediff FindNonInjuryMiscBadHediff(Pawn pawn, bool onlyIfCanKill)
        {
            Hediff hediff = (Hediff)null;
            float num1 = -1f;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                if (hediffs[index].Visible && hediffs[index].def.isBad && (hediffs[index].def.everCurableByItem && !(hediffs[index] is Hediff_Injury)) && (!(hediffs[index] is Hediff_MissingPart) && !(hediffs[index] is Hediff_Addiction) && !(hediffs[index] is Hediff_AddedPart)) && (!onlyIfCanKill || this.CanEverKill(hediffs[index])))
                {
                    float num2 = hediffs[index].Part != null ? hediffs[index].Part.coverageAbsWithChildren : 999f;
                    if (hediff == null || (double)num2 > (double)num1)
                    {
                        hediff = hediffs[index];
                        num1 = num2;
                    }
                }
            }
            return hediff;
        }

        private Hediff_Addiction FindAddiction(Pawn pawn)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                Hediff_Addiction hediffAddiction = hediffs[index] as Hediff_Addiction;
                if (hediffAddiction != null && hediffAddiction.Visible && hediffAddiction.def.everCurableByItem)
                    return hediffAddiction;
            }
            return (Hediff_Addiction)null;
        }

        private Hediff_Injury FindPermanentInjury(
          Pawn pawn,
          IEnumerable<BodyPartRecord> allowedBodyParts = null)
        {
            Hediff_Injury hediffInjury = (Hediff_Injury)null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                Hediff_Injury hd = hediffs[index] as Hediff_Injury;
                if (hd != null && hd.Visible && (hd.IsPermanent() && hd.def.everCurableByItem) && ((allowedBodyParts == null || allowedBodyParts.Contains<BodyPartRecord>(hd.Part)) && (hediffInjury == null || (double)hd.Severity > (double)hediffInjury.Severity)))
                    hediffInjury = hd;
            }
            return hediffInjury;
        }

        private Hediff_Injury FindInjury(
          Pawn pawn,
          IEnumerable<BodyPartRecord> allowedBodyParts = null)
        {
            Hediff_Injury hediffInjury1 = (Hediff_Injury)null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                Hediff_Injury hediffInjury2 = hediffs[index] as Hediff_Injury;
                if (hediffInjury2 != null && hediffInjury2.Visible && hediffInjury2.def.everCurableByItem && ((allowedBodyParts == null || allowedBodyParts.Contains<BodyPartRecord>(hediffInjury2.Part)) && (hediffInjury1 == null || (double)hediffInjury2.Severity > (double)hediffInjury1.Severity)))
                    hediffInjury1 = hediffInjury2;
            }
            return hediffInjury1;
        }

        private void Cure(Hediff hediff)
        {
            Pawn pawn = hediff.pawn;
            pawn.health.RemoveHediff(hediff);
            if (hediff.def.cureAllAtOnceIfCuredByItem)
            {
                int num = 0;
                while (true)
                {
                    ++num;
                    if (num <= 10000)
                    {
                        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediff.def, false);
                        if (firstHediffOfDef != null)
                            pawn.health.RemoveHediff(firstHediffOfDef);
                        else
                            goto label_6;
                    }
                    else
                        break;
                }
                Log.Error("Too many iterations.");
            }
        label_6:
            Messages.Message((string)"MessageHediffCuredByItem".Translate((NamedArgument)hediff.LabelBase.CapitalizeFirst()), (LookTargets)((Thing)pawn), MessageTypeDefOf.PositiveEvent, true);
        }

        private void Cure(BodyPartRecord part, Pawn pawn)
        {
            pawn.health.RestorePart(part, (Hediff)null, true);
            Messages.Message((string)"MessageBodyPartCuredByItem".Translate((NamedArgument)part.LabelCap), (LookTargets)((Thing)pawn), MessageTypeDefOf.PositiveEvent, true);
        }

        private bool CanEverKill(Hediff hediff)
        {
            if (hediff.def.stages != null)
            {
                for (int index = 0; index < hediff.def.stages.Count; ++index)
                {
                    if (hediff.def.stages[index].lifeThreatening)
                        return true;
                }
            }
            return (double)hediff.def.lethalSeverity >= 0.0;
        }
    }
}
