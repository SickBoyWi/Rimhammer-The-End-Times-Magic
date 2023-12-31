using RimWorld;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class DamageWorker_Flying : DamageWorker_PowerLeveled
    {
        public Vector3 PushResult(Thing thingToPush, int pushDist, out bool collision)
        {
            Vector3 vector3_1 = thingToPush.TrueCenter();
            Vector3 vector3_2 = vector3_1;
            bool flag = false;
            for (int index = 1; index <= pushDist; ++index)
            {
                int num1 = index;
                int num2 = index;
                if (vector3_1.x < this.Caster.TrueCenter().x)
                    num1 = -num1;
                if (vector3_1.z < this.Caster.TrueCenter().z)
                    num2 = -num2;
                Vector3 vect = new Vector3((float)vector3_1.x + (float)num1, 0.0f, (float)vector3_1.z + (float)num2);
            if (vect.ToIntVec3().Standable(this.Caster.Map))
                vector3_2 = vect;
            else if (thingToPush is Pawn)
            {
                flag = true;
                break;
            }
        }
        collision = flag;
          return vector3_2;
        }

        public void PushEffect(Thing target, int distance, bool damageOnCollision = false)
        {
            if (target == null || !(target is Pawn))
                return;
            bool collision;
            Vector3 vect = this.PushResult(target, distance, out collision);
            if (((Pawn)target).RaceProps.Humanlike && !target.Equals(Caster))
                ((Pawn)target).needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("RH_TET_MagicallyMoved"), (Pawn)null);
            FlyingObject flyingObject = (FlyingObject)GenSpawn.Spawn(ThingDef.Named("RH_TET_FlyingObjectP"), target.Position, target.Map, WipeMode.Vanish);
            if (collision & damageOnCollision)
                flyingObject.Launch((Thing)this.Caster, new LocalTargetInfo(vect.ToIntVec3()), target, new DamageInfo?(new DamageInfo(DamageDefOf.Blunt, (float)Rand.Range(8, 10), 0.0f, -1f, (Thing)null, (BodyPartRecord)null, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Thing)null)));
            else
                flyingObject.Launch((Thing)this.Caster, new LocalTargetInfo(vect.ToIntVec3()), target);
        }

        public override void MageEffect(Thing target)
        {
            this.PushEffect(target, 8, false);
        }

        public override void WizardEffect(Thing target)
        {
            this.PushEffect(target, 10, false);
        }

        public override void WarlockEffect(Thing target)
        {
            this.PushEffect(target, 12, true);
        }

        public override void MasterEffect(Thing target)
        {
            this.PushEffect(target, 14, true);
        }
    }
}
