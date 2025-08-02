using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class ActiveMagicPod : Thing, IActiveTransporter, IThingHolder
    {
        public bool draftFlag = false;
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
            Scribe_Values.Look<int>(ref this.age, "age", 0, false);
            Scribe_Deep.Look<ActiveTransporterInfo>(ref this.contents, "contents", (object)this);
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
            // TODO: This line was active in 1.5 and before, consider it's necessity when you have some time.
            //this.contents.innerContainer.ThingOwnerTick(true);
            ++this.age;
            if (this.age <= this.contents.openDelay)
                return;
            this.PodOpen();
        }

        private void PodOpen()
        {
            Map map = this.Map;
            if (this.contents.despawnPodBeforeSpawningThing)
                this.DeSpawn(DestroyMode.Vanish);
            for (int index = this.contents.innerContainer.Count - 1; index >= 0; --index)
            {
                Thing thing = this.contents.innerContainer[index];
                Rot4 rot4 = this.contents.setRotation.HasValue ? this.contents.setRotation.Value : Rot4.North;
                if (this.contents.moveItemsAsideBeforeSpawning)
                    GenSpawn.CheckMoveItemsAside(this.Position, rot4, thing.def, map);
                Thing lastResultingThing = (Thing)null;
                if (this.contents.spawnWipeMode.HasValue)
                    lastResultingThing = !this.contents.setRotation.HasValue ? GenSpawn.Spawn(thing, this.Position, map, this.contents.spawnWipeMode.Value) : GenSpawn.Spawn(thing, this.Position, map, this.contents.setRotation.Value, this.contents.spawnWipeMode.Value, false);
                else
                    GenPlace.TryPlaceThing(thing, this.Position, map, ThingPlaceMode.Near, out lastResultingThing, (Action<Thing, int>)((placedThing, count) =>
                    {
                        if (Find.TickManager.TicksGame >= 1200 || !TutorSystem.TutorialMode || placedThing.def.category != ThingCategory.Item)
                            return;
                        Find.TutorialState.AddStartingItem(placedThing);
                    }), (Predicate<IntVec3>)null, rot4);
                if (lastResultingThing is Pawn pawn)
                {
                    if (pawn.RaceProps.Humanlike)
                        TaleRecorder.RecordTale(TaleDefOf.LandedInPod, (object)pawn);
                    if (pawn.drafter != null && (pawn.IsColonist && pawn.Spawned && !map.IsPlayerHome || this.draftFlag))
                        pawn.drafter.Drafted = true;
                    if (pawn.guest != null && pawn.guest.IsPrisoner)
                        pawn.guest.WaitInsteadOfEscapingForDefaultTicks();
                }
            }
            this.contents.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            if (this.contents.leaveSlag)
            {
                for (int index = 0; index < 1; ++index)
                    GenPlace.TryPlaceThing(ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel, (ThingDef)null), this.Position, map, ThingPlaceMode.Near, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4());
            }
            DefDatabase<SoundDef>.GetNamed("DropPod_Open").PlayOneShot((SoundInfo)new TargetInfo(this.Position, map, false));
            this.Destroy(DestroyMode.Vanish);
        }
    }
}
