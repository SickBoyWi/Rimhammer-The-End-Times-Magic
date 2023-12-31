using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class GasCloud_DamageDealer : GasCloud_AffectThing
    {
        protected override void ApplyGasEffect(Verse.Thing thing, float strengthMultiplier)
        {
            BodyPartRecord hitPart = (BodyPartRecord)null;
            if (thing is Pawn pawn && this.Props.damageBodyPartTags.Count > 0)
            {
                BodyPartTagDef tag = this.RandomElementOrDefault<BodyPartTagDef>((IEnumerable<BodyPartTagDef>)this.Props.damageBodyPartTags);
                hitPart = this.RandomElementOrDefault<BodyPartRecord>(pawn.RaceProps?.body.GetPartsWithTag(tag));
            }
            float amount = this.Props.damageAmount * strengthMultiplier;
            if ((double)amount < 1.0 && this.Props.damageCanGlance)
                amount = (double)amount * (double)Rand.Value > 0.5 ? amount : 0.0f;
            thing.TakeDamage(new DamageInfo(this.Props.damageDef, amount, this.Props.damageArmorPenetration, -1f, (Verse.Thing)this, hitPart, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Verse.Thing)null));
        }

        private T RandomElementOrDefault<T>(IEnumerable<T> source)
        {
            if (!(source is IList<T> objList))
                objList = (IList<T>)source.ToList<T>();
            IList<T> objList1 = objList;
            return objList1.Count > 0 ? objList1[Rand.Range(0, objList1.Count)] : default(T);
        }
    }
}
