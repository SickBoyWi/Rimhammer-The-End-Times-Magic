using System.Collections.Generic;
using AbilityUser;
using RimWorld;
using Verse;

/**
 * Same as trueburst ability verb, but adds miscast check.
 * */
namespace TheEndTimes_Magic
{
    public class Verb_UseAbility_TrueBurstFaith : Verb_UseAbility_TrueBurst
    {
        public Verb_UseAbility_TrueBurstFaith()
            : base()
        {
        }

        protected override bool TryCastShot()
        {
            MagicUtility.MakeGenericCastingMoteFaith(this.Caster as Pawn);

            return base.TryCastShot();
        }
    }
}