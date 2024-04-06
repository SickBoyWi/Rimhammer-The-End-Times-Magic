using SickAbilityUser;
using Verse;

namespace TheEndTimes_Magic
{
    internal class Verb_ConsiderLineOfSight : Verb_UseAbility
    {
        private bool validTarg;

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            this.validTarg = targ.IsValid && targ.CenterVector3.InBounds(this.CasterPawn.Map) && !targ.Cell.Fogged(this.CasterPawn.Map) && targ.Cell.Walkable(this.CasterPawn.Map) && (double)(root - targ.Cell).LengthHorizontal < (double)(this.verbProps.range);
            return this.validTarg;
        }

        public Verb_ConsiderLineOfSight()
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
