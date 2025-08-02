using SickAbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class Projectile_UlricWolves : Projectile_Ability
    {
        private int age;
        private bool initialized;
        private bool summoning;
        private bool summoningComplete;
        private bool destroyed;
        private int duration = 3600;
        private CompMagicUser comp;
        private List<Pawn> summonedPawns;
        private Pawn casterPawn;
        private List<IntVec3> summoningArea;
        private int pawnSpawnCount = 3;

        public Projectile_UlricWolves()
            : base()
        {
        }

        protected override void Tick()
        {
            base.Tick();
            if(!summonedPawns.NullOrEmpty<Pawn>())
            { 
                foreach (Pawn summonedPawn in summonedPawns)
                { 
                    if (summonedPawn != null && (summonedPawn.Downed || summonedPawn.Destroyed || summonedPawn.Dead))
                    {
                        this.age = this.duration + 1;
                        if (summonedPawn.Corpse != null)
                            summonedPawn.Corpse.Destroy(DestroyMode.Vanish);

                        this.Destroy(DestroyMode.Vanish);
                    }
                    ++this.age;
                }
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.age < this.duration)
                return;
            base.Destroy(mode);
        }

        private void Initialize()
        {
            this.casterPawn = this.launcher as Pawn;
            this.comp = this.casterPawn.GetComp<CompMagicUser>();
            this.summoningArea = new List<IntVec3>();
            this.summoningArea.Clear();
            this.summoningArea = GenRadial.RadialCellsAround(this.usedTarget.CenterVector3.ToIntVec3(), 5f, false).ToList<IntVec3>();
            this.age = 0;
            this.summoning = true;
            this.initialized = true;
            this.summonedPawns = new List<Pawn>();
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map1 = this.Map;

            base.Impact(hitThing);

            if (!this.initialized)
            {
                this.Initialize();
                FleckMaker.ThrowSmoke(this.summoningArea[0].ToVector3Shifted(), this.Map, 2f);
            }

            if (this.summoningComplete)
            {
                return;
            }

            IntVec3 position = this.casterPawn.Position;

            Map map2 = this.Map;
            FleckMaker.Static(position, map2, FleckDefOf.PsycastAreaEffect, 2f);

            if (this.usedTarget.CenterVector3 != null)
            {
                position = this.usedTarget.CenterVector3.ToIntVec3();
            }
            
            this.SpawnLoop(position, map2);

            FleckMaker.ThrowSmoke(position.ToVector3(), map2, 2f);
            FleckMaker.ThrowMicroSparks(position.ToVector3(), map2);
            FleckMaker.ThrowHeatGlow(position, map2, 2f);

            SoundInfo info = SoundInfo.InMap(new TargetInfo(position, map2, false), MaintenanceType.None);
            info.pitchFactor = 1.3f;
            info.volumeFactor = 1.6f;
            SoundDef.Named("RH_TET_Jabberslythe_Call").PlayOneShot(info);

            this.summoningComplete = true;
            this.summoning = false;
        }

        public void SpawnLoop(IntVec3 position, Map map)
        {
            Faction faction = this.launcher.Faction;
            
            for (int i = 0; i < pawnSpawnCount; i++)
            { 
                float biologicalAge = (float)RH_TET_MagicMod.random.Next(10, 15);

                PawnGenerationRequest pawnRequest = new PawnGenerationRequest(RH_TET_MagicDefOf.RH_TET_Magic_WolfUlric_Summonable, faction, PawnGenerationContext.PlayerStarter, -1,
                    true, false, false,
                    false, true, 0f,
                    false, true, false,
                    false, false, false,
                    false, false, false,
                    0.0f, 0.0f, null,
                    0.0f, null, null,
                    null, null, null,
                    biologicalAge, biologicalAge);
            
                Pawn_WolfUlric newPawn = (Pawn_WolfUlric)PawnGenerator.GeneratePawn(pawnRequest);
                newPawn.validSummoning = true;
                newPawn.Spawner = this.casterPawn;
                newPawn.Temporary = true;
                newPawn.TicksToDestroy = this.duration;

                try
                {
                    GenSpawn.Spawn((Thing)newPawn, position, map, WipeMode.Vanish);
                    this.summonedPawns.Add(newPawn);
                    newPawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, (string)null, true, false, false, (Pawn)null, true);
                }
                catch (Exception e)
                {
                    this.age = this.duration;
                    Log.Error("RH_TET_WolfFail".Translate() + e.Message);
                    this.Destroy(DestroyMode.Vanish);
                }
            }
        }

        public Vector3 GetVector(IntVec3 center, IntVec3 objectPos)
        {
            Vector3 vector3 = (objectPos - center).ToVector3();
            float magnitude = vector3.magnitude;
            return vector3 / magnitude;
        }

        public override void ExposeData()
        {
            ((Projectile_AbilityBase)this).ExposeData();
            Scribe_Values.Look<int>(ref this.age, "age", 0, false);
            Scribe_Values.Look<bool>(ref this.initialized, "initialized", false, false);
            Scribe_Values.Look<bool>(ref this.summoning, "summoning", false, false);
            Scribe_Values.Look<bool>(ref this.summoningComplete, "summoningComplete", false, false);
            Scribe_Values.Look<bool>(ref this.destroyed, "destroyed", false, false);
            Scribe_Collections.Look<Pawn>(ref this.summonedPawns, "summonedPawns", LookMode.Reference, (object[])Array.Empty<object>());
            Scribe_Collections.Look<IntVec3>(ref this.summoningArea, "summoningArea", LookMode.Value);
            Scribe_References.Look<Pawn>(ref this.casterPawn, "casterPawn", false);
        }
    }
}
