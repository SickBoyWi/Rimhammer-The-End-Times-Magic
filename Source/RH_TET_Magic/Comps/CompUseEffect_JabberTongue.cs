using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TheEndTimes_Magic
{
    public class CompUseEffect_JabberTongue : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);

            // 1/20 chance of random magic trait.
            int rand = RH_TET_MagicMod.random.Next(0, 20);

            if (rand == 0)
            {
                // Success! Healing increased.
                List<TraitDef> traits = new List<TraitDef>();
                traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfChaosUndividedTrait);
                traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfSlaaneshTrait);
                traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfNurgleTrait);
                traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfTzeentchTrait);
                traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfWarpTrait);
                traits.Add(RH_TET_MagicDefOf.RH_TET_LoreOfPlagueTrait);

                TraitDef randTrait = traits.RandomElement();

                Trait trait = new Trait(randTrait, 1);
                user.story.traits.GainTrait(trait);

                Messages.Message("RH_TET_GainedMagicTrait".Translate(user.Name, randTrait.label), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
            }
            else
            {
                // Fail! 
                Messages.Message("RH_TET_DidntGainMagicTrait".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NeutralEvent, true);
            }

            // Add nutrition.
            float nutritionAmt = .75F;
            if (user.needs.food.CurLevel > .25F)
                nutritionAmt = 1.0F - user.needs.food.CurLevel;
            user.needs.food.CurLevel += nutritionAmt;

            // 50/50 chance of food poisoning.
            int rand2 = RH_TET_MagicMod.random.Next(0, 2);

            if (rand2 == 0)
            {
                // Fail, give food poisoning.
                HediffDef disease = HediffDefOf.FoodPoisoning;

                user.health.AddHediff(disease);
                user.health.hediffSet.GetFirstHediffOfDef(disease).Severity = (float)(RH_TET_MagicMod.random.Next(5, 15) * .01f);

                user.jobs.StartJob(JobMaker.MakeJob(JobDefOf.Vomit), JobCondition.InterruptForced, (ThinkNode)null, true, true, (ThinkTreeDef)null, new JobTag?(), false, false);

                Messages.Message("RH_TET_FoodPoisoning".Translate(user.Name, "jabberslythe tongue"), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeEvent, true);
            }

            // Add chaos corruption.
            HediffDef chaosCorruption = DefDatabase<HediffDef>.GetNamedSilentFail("RH_TET_ChaosTaintBuildup");
            int corruptionAmt = 39;
            if (chaosCorruption == null)
            {
                chaosCorruption = HediffDefOf.ToxicBuildup;
                corruptionAmt = 64;
            }

            user.health.AddHediff(chaosCorruption);
            user.health.hediffSet.GetFirstHediffOfDef(chaosCorruption).Severity = (float)(RH_TET_MagicMod.random.Next(20, corruptionAmt) * .01f);

            Messages.Message("RH_TET_ChaosCorruptionToxic".Translate(user.Name, chaosCorruption.label), (LookTargets)((Thing)user), MessageTypeDefOf.NegativeEvent, true);
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            failReason = (string) null;
            return true;
        }
    }
}
