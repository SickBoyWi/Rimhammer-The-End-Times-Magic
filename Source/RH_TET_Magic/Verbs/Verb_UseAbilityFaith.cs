using System.Collections.Generic;
using SickAbilityUser;
using RimWorld;
using Verse;

/**
 * Same as basic ability verb, for faith abilities.
 * */
namespace TheEndTimes_Magic
{
    public class Verb_UseAbilityFaith : Verb_UseAbility
    {
        public Verb_UseAbilityFaith()
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