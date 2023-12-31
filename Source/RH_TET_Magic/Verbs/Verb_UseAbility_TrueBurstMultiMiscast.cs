using System.Collections.Generic;
using AbilityUser;
using RimWorld;
using Verse;

/**
 * Same as trueburst ability verb, but adds miscast check. USE FOR Burst > 1.
 * 
 * NOTE: This is here because of a defect in the Ability User code base. 
 * If a multiburst spell kills it's intended target before the final burst shot, an
 * error pops up saying "No line of sight", and the spell does not go into cooldown
 * mode, nor does it reduce the magic pool.
 * */
// TODO THIS ISN'T USED IN THE CONFIG.
namespace TheEndTimes_Magic
{
    public class Verb_UseAbility_TrueBurstMultiMiscast : Verb_UseAbility_TrueBurst
    {
        //private bool shotsFired = false;
        private int shotsFiredCount = 0;

        public Verb_UseAbility_TrueBurstMultiMiscast()
            : base()
        {
        }

        protected override bool TryCastShot()
        {
            if (MagicUtility.TryMiscast(this.caster))
                return false;

            MagicUtility.MakeGenericCastingMoteMagic(this.Caster as Pawn);

            var result = false;
            TargetsAoE.Clear();
            UpdateTargets();
            MagicAbility ma = this.Ability as MagicAbility;

            int burstShots = ma.Def.MainVerb.burstShotCount;
            int ticksBetweenBursts = ma.Def.MainVerb.ticksBetweenBurstShots;
            if (UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && TargetsAoE.Count > 1)
            {
                TargetsAoE.RemoveRange(0, TargetsAoE.Count - 1);
            }

            // If we start shooting, add cooldown and magic pool usage. 
            this.Ability.CooldownTicksLeft = (int)UseAbilityProps.SecondsToRecharge * GenTicks.TicksPerRealSecond;
            (this.Caster as Pawn).needs.TryGetNeed<Need_MagicPool>()?.UseMagicPower(ma.ActualMagicCost);

            for (var i = 0; i < TargetsAoE.Count; i++)
            {
                for (var j = 0; j < burstShots; j++)
                {
                    var attempt = TryLaunchProjectile(ma.Def.MainVerb.defaultProjectile, TargetsAoE[i]);
                    ////Log.Message(TargetsAoE[i].ToString());
                    if (attempt.HasValue)
                        result = attempt.GetValueOrDefault();
                }
            }

            PostCastShot(result, out result);
            return result;

            //bool successVal = base.TryCastShot();

            //return successVal;
        }
    }
}