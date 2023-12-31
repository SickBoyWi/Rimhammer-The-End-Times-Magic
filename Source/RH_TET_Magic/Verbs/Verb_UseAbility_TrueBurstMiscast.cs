using System.Collections.Generic;
using AbilityUser;
using RimWorld;
using Verse;

/**
 * Same as trueburst ability verb, but adds miscast check.
 * */
namespace TheEndTimes_Magic
{
    public class Verb_UseAbility_TrueBurstMiscast : Verb_UseAbility_TrueBurst
    {
        //private bool shotsFired = false;
        //private int shotsFiredCount = 0;

        public Verb_UseAbility_TrueBurstMiscast()
            : base()
        {
        }

        protected override bool TryCastShot()
        {
            Pawn p = this.Caster as Pawn;
            if (!RH_TET_MagicMod.IsActiveCasterWillAdd(p) &&  MagicUtility.TryMiscast(this.caster))
                return false;

            MagicUtility.MakeGenericCastingMoteMagic(p);

            bool successVal = base.TryCastShot();
            //int burstCount = 0;
            //if (this.Ability != null && this.Ability.Def != null && this.Ability.Def.MainVerb != null)
            //    burstCount = this.Ability.Def.MainVerb.burstShotCount;

            //if (burstCount > 1)
            //{
            //    shotsFiredCount++;
            //}

            //if (!shotsFired)
            //    this.Ability.PostAbilityAttempt();
            //shotsFired = true;

            return successVal;
        }

    }
}