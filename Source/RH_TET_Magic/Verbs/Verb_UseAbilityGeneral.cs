using System.Collections.Generic;
using AbilityUser;
using RimWorld;
using Verse;

/**
 * Same as basic ability verb, for general abilities.
 * */
namespace TheEndTimes_Magic
{
    public class Verb_UseAbilityAction : Verb_UseAbility
    {
        public Verb_UseAbilityAction()
            : base()
        {
        }

        protected override bool TryCastShot()
        {
            MagicUtility.MakeGenericCastingMoteAbility(this.Caster as Pawn);

            return base.TryCastShot();
        }
    }
}