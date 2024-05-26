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
    public class Projectile_SavageDominion : Projectile_Ability
    {
        private int age;
        private bool initialized;
        private bool summoning;
        private bool summoningComplete;
        private bool destroyed;
        private int duration = 2500;
        private CompMagicUser comp;
        private Pawn summonedPawn;
        private Pawn casterPawn;
        private List<IntVec3> summoningArea;

        public Projectile_SavageDominion()
            : base()
        {
        }

        public override void Tick()
        {
            base.Tick();
            if (this.summonedPawn != null && (this.summonedPawn.Destroyed || this.summonedPawn.Dead))
            {
                this.age = this.duration + 1;
                if (this.summonedPawn.Corpse != null)
                    this.summonedPawn.Corpse.Destroy(DestroyMode.Vanish);

                this.Destroy(DestroyMode.Vanish);
            }
            ++this.age;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.age < this.duration)
                return;
            if (this.destroyed)
                return;
            this.destroyed = true;
            if (base.Destroyed)
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
            
            if (this.usedTarget.CenterVector3 != null)
            {
                position = this.usedTarget.CenterVector3.ToIntVec3();
            }
            
            this.SingleSpawnLoop(position, map2);

            FleckMaker.ThrowSmoke(position.ToVector3(), map2, 2f);
            FleckMaker.ThrowMicroSparks(position.ToVector3(), map2);
            FleckMaker.ThrowHeatGlow(position, map2, 2f);
            FleckMaker.Static(position, map2, FleckDefOf.PsycastAreaEffect, 2f);

            SoundInfo info = SoundInfo.InMap(new TargetInfo(position, map2, false), MaintenanceType.None);
            info.pitchFactor = 1.3f;
            info.volumeFactor = 1.6f;
            SoundDef.Named("RH_TET_Jabberslythe_Call").PlayOneShot(info);

            this.summoningComplete = true;
            this.summoning = false;
        }

        public void SingleSpawnLoop(IntVec3 position, Map map)
        {
            Faction faction = this.launcher.Faction;
            
            // Determine spell level.
            int spellLevel = -1;

            foreach (MagicPower mp in this.casterPawn.GetComp<CompMagicUser>().MagicData.PowersWild)
            {
                if (mp.abilityDef.defName.Contains("SavageDominion"))
                {
                    spellLevel = mp.level;
                }
            }

            if (spellLevel <= 0)
                return;

            int minAge = 80;
            int maxAge = 120;

            switch (spellLevel)
            {
                case 1:
                    break;
                case 2:
                    minAge = 200;
                    maxAge = 300;
                    duration = 2800;
                    break;
                case 3:
                    minAge = 450;
                    maxAge = 600;
                    duration = 3000;
                    break;
                case 4:
                    minAge = 800;
                    maxAge = 950;
                    duration = 3200;
                    break;
            }

            float biologicalAge = (float)RH_TET_MagicMod.random.Next(minAge, maxAge);

            // TODO 1.4 - Set faction to null here
            PawnGenerationRequest pawnRequest = new PawnGenerationRequest(RH_TET_MagicDefOf.RH_TET_JabberslytheSummoned, null, PawnGenerationContext.PlayerStarter, -1,
                    true, false, false,
                    false, true, 0f,
                    false, true, false,
                    false, false, false,
                    false, false, false,
                    0.0f, 0.0f, null,
                    0.0f, null, null,
                    null, null, null,
                    biologicalAge, biologicalAge);
            
            Pawn_JabberslytheSummonedExpires newPawn = (Pawn_JabberslytheSummonedExpires)PawnGenerator.GeneratePawn(pawnRequest);
            newPawn.validSummoning = true;
            newPawn.Spawner = this.casterPawn;
            newPawn.Temporary = true;
            newPawn.TicksToDestroy = this.duration;

            try
            {
                GenSpawn.Spawn((Thing)newPawn, position, map, WipeMode.Vanish);
                this.summonedPawn = (Pawn)newPawn;
                newPawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, (string)null, true, false, false, (Pawn)null, false);
            }
            catch (Exception e)
            {
                this.age = this.duration;
                Log.Error("RH_TET_JabberFail".Translate() + e.Message);
                this.Destroy(DestroyMode.Vanish);
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
            Scribe_References.Look<Pawn>(ref this.summonedPawn, "summonedPawn", false);
            Scribe_Collections.Look<IntVec3>(ref this.summoningArea, "summoningArea", LookMode.Value);
            Scribe_References.Look<Pawn>(ref this.casterPawn, "casterPawn", false);
        }
    }
}
