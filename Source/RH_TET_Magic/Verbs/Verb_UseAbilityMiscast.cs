using System.Collections.Generic;
using AbilityUser;
using RimWorld;
using Verse;

/**
 * Same as basic ability verb, but adds miscast check.
 * */
namespace TheEndTimes_Magic
{
    public class Verb_UseAbilityMiscast : Verb_UseAbility
    {
        public Verb_UseAbilityMiscast()
            : base()
        {
        }

        protected override bool TryCastShot()
        {
            if (MagicUtility.TryMiscast(this.caster))
                return true;

            MagicUtility.MakeGenericCastingMoteMagic(this.Caster as Pawn);

            return base.TryCastShot();
        }
    }
}