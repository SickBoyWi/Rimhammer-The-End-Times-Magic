using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class ActiveMagicPod : Thing, IActiveTransporter, IThingHolder
    {
        public int age;
        private ActiveTransporterInfo contents;

        public ActiveTransporterInfo Contents
        {
            get
            {
                return this.contents;
            }
            set
            {
                if (this.contents != null)
                    this.contents.parent = (IThingHolder)null;
                if (value != null)
                    value.parent = (IThingHolder)this;
                this.contents = value;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.age, "ageA", 0, false);
            Scribe_Deep.Look<ActiveTransporterInfo>(ref this.contents, "contentsA", (object)this);
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return (ThingOwner)null;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, (IList<Thing>)this.GetDirectlyHeldThings());
            if (this.contents == null)
                return;
            outChildren.Add((IThingHolder)this.contents);
        }

        protected override void Tick()
        {
            if (this.contents == null || !this.Spawned)
                return;
            ++this.age;
            if (this.age <= this.contents.openDelay)
                return;
            this.PodOpen();
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.contents != null)
                this.contents.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            Map map = this.Map;
            base.Destroy(mode);
            if (mode != DestroyMode.KillFinalize)
                return;
            for (int index = 0; index < 1; ++index)
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel, (ThingDef)null), this.Position, map, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4?(), 1);
        }

        private void PodOpen()
        {
            Map map1 = this.Map;
            if (this.contents.despawnPodBeforeSpawningThing)
                this.DeSpawn(DestroyMode.Vanish);
            Rot4? nullable;
            for (int index = this.contents.innerContainer.Count - 1; index >= 0; --index)
            {
                Thing thing = this.contents.innerContainer[index];
                nullable = this.contents.setRotation;
                Rot4 thingRot = nullable ?? Rot4.North;
                if (this.contents.moveItemsAsideBeforeSpawning)
                    GenSpawn.CheckMoveItemsAside(this.Position, thingRot, thing.def, map1);
                Thing lastResultingThing;
                if (!this.contents.spawnWipeMode.HasValue)
                    GenPlace.TryPlaceThing(thing, this.Position, map1, ThingPlaceMode.Near, out lastResultingThing, (Action<Thing, int>)((placedThing, count) =>
                    {
                        if (Find.TickManager.TicksGame >= 1200 || !TutorSystem.TutorialMode || placedThing.def.category != ThingCategory.Item)
                            return;
                        Find.TutorialState.AddStartingItem(placedThing);
                    }), (Predicate<IntVec3>)null, new Rot4?(thingRot), 1);
                else
                    lastResultingThing = !this.contents.setRotation.HasValue ? GenSpawn.Spawn(thing, this.Position, map1, this.contents.spawnWipeMode.Value) : GenSpawn.Spawn(thing, this.Position, map1, this.contents.setRotation.Value, this.contents.spawnWipeMode.Value, false, false);
                if (lastResultingThing is Pawn pawn)
                {
                    if (pawn.RaceProps.Humanlike)
                        TaleRecorder.RecordTale(TaleDefOf.LandedInPod, (object)pawn);
                    if (pawn.IsColonist && pawn.Spawned && !map1.IsPlayerHome)
                        pawn.drafter.Drafted = true;
                    if (pawn.guest != null && pawn.guest.IsPrisoner)
                        pawn.guest.WaitInsteadOfEscapingForDefaultTicks();
                }
            }
            this.contents.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            if (this.contents.leaveSlag)
            {
                for (int index = 0; index < 1; ++index)
                {
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel, (ThingDef)null);
                    IntVec3 position = this.Position;
                    Map map2 = map1;
                    nullable = new Rot4?();
                    Rot4? rot = nullable;
                    GenPlace.TryPlaceThing(thing, position, map2, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, rot, 1);
                }
            }
            if (this.def.soundOpen != null)
                this.def.soundOpen.PlayOneShot((SoundInfo)new TargetInfo(this.Position, map1, false));
            this.Destroy(DestroyMode.Vanish);
        }
    }
}
