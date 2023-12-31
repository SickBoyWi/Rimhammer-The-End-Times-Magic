using RimWorld;
using Verse;

namespace TheEndTimes_Magic
{
    public class DamageWorker_PowerLeveled : DamageWorker
    {
        private Pawn caster;

        public Pawn Caster
        {
            get
            {
                return this.caster;
            }
            set
            {
                this.caster = value;
            }
        }

        public virtual void MageEffect(Thing target)
        {
        }

        public virtual void WizardEffect(Thing target)
        {
        }

        public virtual void WarlockEffect(Thing target)
        {
        }

        public virtual void MasterEffect(Thing target)
        {
        }

        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
            damageResult.totalDamageDealt = 0.0f;
            int amount = (int)dinfo.Amount;
            this.caster = dinfo.Instigator as Pawn;
            switch (amount)
            {
                case 1:
                    this.MageEffect(victim);
                    break;
                case 2:
                    this.WizardEffect(victim);
                    break;
                case 3:
                    this.WarlockEffect(victim);
                    break;
                case 4:
                    this.MasterEffect(victim);
                    break;
                default:
                    Log.Error(this.def.label + " only works with damages 1, 2, 3, or 4");
                    break;
            }
            return damageResult;
        }
    }
}
