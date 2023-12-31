using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class MoteProperties_GasEffect : MoteProperties_GasCloud
    {
        public int effectInterval = 1;
        public bool affectsDownedPawns = true;
        public float toxicSensitivityStatPower = 1f;
        public float immunizingApparelPower = 1f;
        public List<ThingDef> immunizingApparelDefs = new List<ThingDef>(0);
        public float damageArmorPenetration = 1f;
        public List<BodyPartTagDef> damageBodyPartTags = new List<BodyPartTagDef>(0);
        public bool damageCanGlance = true;
        public bool affectsThings;
        public bool affectsPlants;
        public bool affectsFleshy;
        public bool affectsMechanical;
        public HediffDef hediffDef;
        public FloatRange hediffSeverityPerGastick;
        public DamageDef damageDef;
        public float damageAmount;
    }
}
