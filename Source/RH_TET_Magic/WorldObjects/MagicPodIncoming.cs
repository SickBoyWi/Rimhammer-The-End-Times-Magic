using RimWorld;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class MagicPodIncoming : Skyfaller, IActiveDropPod, IThingHolder
    {
        public bool draftFlag = false;

        public ActiveDropPodInfo Contents
        {
            get
            {
                return ((ActiveMagicPod)this.innerContainer[0]).Contents;
            }
            set
            {
                ((ActiveMagicPod)this.innerContainer[0]).Contents = value;
            }
        }

        protected override void SpawnThings()
        {
            if (!this.Contents.spawnWipeMode.HasValue)
            {
                base.SpawnThings();
            }
            else
            {
                for (int index = this.innerContainer.Count - 1; index >= 0; --index)
                    GenSpawn.Spawn(this.innerContainer[index], this.Position, this.Map, this.Contents.spawnWipeMode.Value);
            }
            // TODO - Added this 20220428 to fix defect. TEST!!
            RH_TET_MagicMod.SetSafeToRemoveMapNow(true);
        }

        protected override void Impact()
        {
            IntVec3 position;
            for (int index = 0; index < 6; ++index)
            {
                position = this.Position;
                FleckMaker.ThrowDustPuff(position.ToVector3Shifted() + Gen.RandomHorizontalVector(1f), this.Map, 1.2f);
            }
            position = this.Position;
            FleckMaker.ThrowLightningGlow(position.ToVector3Shifted(), this.Map, 2f);
            GenClamor.DoClamor((Thing)this, 15f, ClamorDefOf.Impact);
            RH_TET_MagicMod.SetSafeToRemoveMapNow(false);
            base.Impact();
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
        }
    }
}
