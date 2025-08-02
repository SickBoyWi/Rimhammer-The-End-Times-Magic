
using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompMagicUser : CompAbilityUser
    {
        private MagicData magicData;
        private const int XP_PER_LEVEL = 600;
        public List<Thing> summonedPawns;

        public MagicData MagicData
        {
            get
            {
                return this.magicData ?? (this.magicData = new MagicData(this));
            }
        }

        public int MagicUserLevel
        {
            get
            {
                return this.MagicData.Level;
            }
            set
            {
                if (value > this.MagicData.Level)
                {
                    ++this.MagicData.AbilityPoints;
                    if (this.MagicData.XP < value * XP_PER_LEVEL)
                        this.MagicData.XP = value * XP_PER_LEVEL;
                }
                this.MagicData.Level = value;
            }
        }

        public int MagicUserXP
        {
            get
            {
                return this.MagicData.XP;
            }
            set
            {
                this.MagicData.XP = value;
            }
        }

        public float XPLastLevel
        {
            get
            {
                float num = 0.0f;
                if (this.MagicUserLevel > 0)
                    num = (float)(this.MagicUserLevel * XP_PER_LEVEL);
                return num;
            }
        }

        public float XPTillNextLevelPercent
        {
            get
            {
                return ((float)this.MagicUserXP - this.XPLastLevel) / ((float)this.MagicUserXPTillNextLevel - this.XPLastLevel);
            }
        }

        public int MagicUserXPTillNextLevel
        {
            get
            {
                return (this.MagicUserLevel + 1) * XP_PER_LEVEL;
            }
        }

        public int MagicPowerPoints
        {
            get
            {
                int num = 0;
                if ((this.MagicData.PowersBeast != null && this.MagicData.PowersBeast.Count > 0)
                    || (this.MagicData.PowersTzeentch != null && this.MagicData.PowersTzeentch.Count > 0)
                    || (this.MagicData.PowersShadow != null && this.MagicData.PowersShadow.Count > 0)
                    || (this.MagicData.PowersDeath != null && this.MagicData.PowersDeath.Count > 0)
                    || (this.MagicData.PowersWild != null && this.MagicData.PowersWild.Count > 0)
                    || (this.MagicData.PowersFire != null && this.MagicData.PowersFire.Count > 0)
                    || (this.MagicData.PowersMetal != null && this.MagicData.PowersMetal.Count > 0)
                    || (this.MagicData.PowersHeavens != null && this.MagicData.PowersHeavens.Count > 0)
                    || (this.MagicData.PowersLight != null && this.MagicData.PowersLight.Count > 0)
                    || (this.MagicData.PowersLife != null && this.MagicData.PowersLife.Count > 0)
                    || (this.MagicData.PowersNurgle != null && this.MagicData.PowersNurgle.Count > 0)
                    || (this.MagicData.PowersChaos != null && this.MagicData.PowersChaos.Count > 0)
                    || (this.MagicData.PowersSlaanesh != null && this.MagicData.PowersSlaanesh.Count > 0)
                    || (this.MagicData.PowersHigh != null && this.MagicData.PowersHigh.Count > 0)
                    || (this.MagicData.PowersPlague != null && this.MagicData.PowersPlague.Count > 0)
                    || (this.MagicData.PowersGreatMaw != null && this.MagicData.PowersGreatMaw.Count > 0)
                    || (this.MagicData.PowersWarp != null && this.MagicData.PowersWarp.Count > 0)
                    || (this.MagicData.PowersStealth != null && this.MagicData.PowersStealth.Count > 0)
                    || (this.MagicData.PowersAddons != null && this.MagicData.PowersAddons.Count > 0)
                    )
                {
                    foreach (MagicPower magicPower in this.MagicData.PowersBeast)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersTzeentch)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersShadow)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersDeath)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersWild)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersFire)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersHeavens)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersMetal)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersLight)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersLife)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersNurgle)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersChaos)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersSlaanesh)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersHigh)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersPlague)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersGreatMaw)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersWarp)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersStealth)
                        num += magicPower.level;
                    foreach (MagicPower magicPower in this.MagicData.PowersAddons)
                        num += magicPower.level;
                }
                return num;
            }
        }

        public void LevelUp(bool hideNotification = false)
        {
            ++this.MagicUserLevel;
            if (this.MagicUserLevel == 1)
            {
                if (!hideNotification)
                {
                    Messages.Message("RH_TET_MagicPowersUnlocked".Translate((object)((ThingComp)this).parent.Label), MessageTypeDefOf.SilentInput, true);
                    Find.LetterStack.ReceiveLetter("RH_TET_MagicalAwakeningLabel".Translate(), "RH_TET_MagicalAwakeningDesc".Translate((object)((ThingComp)this).parent.Label), LetterDefOf.PositiveEvent, (LookTargets)((Thing)((ThingComp)this).parent), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
                }
                SoundDef.Named("RH_TET_MagicPowersUnlocked").PlayOneShotOnCamera((Map)null);
            }
            else if (!hideNotification)
                Messages.Message("RH_TET_MagicalLevelUp".Translate((object)((ThingComp)this).parent.Label), MessageTypeDefOf.PositiveEvent, true);
        }

        public void IncreaseExperience(int increaseAmt)
        {
            this.MagicData.XP += increaseAmt;
        }

        public void ResetPowers()
        {
            foreach (MagicPower magicPower in this.MagicData.PowersBeast)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersTzeentch)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersWild)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersShadow)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersDeath)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersFire)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersHeavens)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersMetal)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersLight)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersLife)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersNurgle)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersChaos)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersSlaanesh)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersHigh)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersPlague)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersGreatMaw)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersWarp)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersStealth)
                magicPower.level = 0;
            foreach (MagicPower magicPower in this.MagicData.PowersAddons)
                magicPower.level = 0;

            List<MagicAbility> magicAbilityList = new List<MagicAbility>();
            using (List<PawnAbility>.Enumerator enumerator = this.AbilityData.Powers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PawnAbility current = enumerator.Current;
                    magicAbilityList.Add(current as MagicAbility);
                }
            }
            foreach (PawnAbility pawnAbility in magicAbilityList)
                this.RemovePawnAbility(pawnAbility.Def);
            this.MagicData.AbilityPoints = this.MagicUserLevel;
            this.UpdateAbilities();
        }

        public void LevelUpPower(MagicPower power)
        {
            using (List<SickAbilityUser.AbilityDef>.Enumerator enumerator = power.abilityDefs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    this.RemovePawnAbility(enumerator.Current);
            }
            ++power.level;
            this.AddPawnAbility(power.abilityDef, true, -1f);
        }

        public MagicLoreType MagicLoreType
        {
            get
            {
                if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfBeastsTrait))
                    return MagicLoreType.Beasts;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfTzeentchTrait))
                    return MagicLoreType.Tzeentch;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfShadowTrait))
                    return MagicLoreType.Shadow;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfDeathTrait))
                    return MagicLoreType.Death;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfWildTrait))
                    return MagicLoreType.Wild;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfFireTrait))
                    return MagicLoreType.Fire;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfHeavensTrait))
                    return MagicLoreType.Heavens;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfMetalTrait))
                    return MagicLoreType.Metal;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfLightTrait))
                    return MagicLoreType.Light;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfLifeTrait))
                    return MagicLoreType.Life;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfNurgleTrait))
                    return MagicLoreType.Nurgle;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfChaosUndividedTrait))
                    return MagicLoreType.ChaosUndivided;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfSlaaneshTrait))
                    return MagicLoreType.Slaanesh;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfHighTrait))
                    return MagicLoreType.High;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfPlagueTrait))
                    return MagicLoreType.Plague;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfGreatMawTrait))
                    return MagicLoreType.GreatMaw;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfWarpTrait))
                    return MagicLoreType.Warp;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_LoreOfStealthTrait))
                    return MagicLoreType.Stealth;
                else
                    return MagicLoreType.None;
            }
        }

        public Need_MagicPool MagicPool
        {
            get
            {
                return this.Pawn.needs.TryGetNeed<Need_MagicPool>();
            }
        }

        public bool IsMagicUser
        {
            get
            {
                return this.Pawn.IsMagicUser();
            }
        }

        public override bool TryTransformPawn()
        {
            return this.IsMagicUser;
        }

        public override List<HediffDef> IgnoredHediffs()
        {
            return new List<HediffDef>()
            {
                RH_TET_MagicDefOf.RH_TET_MagicWielder
            };
        }

        public override void CompTick()
        {
            if (this.AbilityUser == null || !this.AbilityUser.Spawned || (Find.TickManager.TicksGame <= 200 || !this.IsMagicUser))
                return;
            
            if (!this.MagicData.MagicPowersInitialized)
            {
                this.PostInitializeTick();
                this.MagicData.MagicPowersInitialized = true;
            }
            if (!this.MagicData.TabResolved)
            {
                this.ResolveMagicTab();
                this.MagicData.TabResolved = true;
            }

            if (summonedPawns != null)
            { 
                for (int index = 0; index < this.summonedPawns.Count; ++index)
                {
                    Pawn summonedMinion = this.summonedPawns[index] as Pawn;
                    if (summonedMinion == null || summonedMinion.Dead || summonedMinion.Destroyed)
                    {
                        this.summonedPawns.Remove(this.summonedPawns[index]);
                    }
                }
            }

            base.CompTick();

            if (Find.TickManager.TicksGame % 30 != 0 || this.MagicUserXP <= this.MagicUserXPTillNextLevel)
                return;

            this.LevelUp(false);
        }

        public void PostInitializeTick()
        {
            if (this.AbilityUser == null || !this.AbilityUser.Spawned || this.AbilityUser.story == null)
                return;
            
            this.Initialize();
            this.ResolveMagicTab();
            this.ResolveMagicPowers();
            this.ResolveMagicPool();
        }

        public void ResolveMagicTab()
        {
            IEnumerable<InspectTabBase> inspectTabs = this.AbilityUser.GetInspectTabs();
            if (inspectTabs == null || inspectTabs.Count<InspectTabBase>() <= 0)
                return;
            if (inspectTabs.FirstOrDefault<InspectTabBase>((Func<InspectTabBase, bool>)(x => x is ITab_Pawn_Magic)) != null)
                return;
            try
            {
                this.AbilityUser.def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Magic)));
            }
            catch (Exception ex)
            {
                Log.Error("Could not instantiate inspector tab of type " + (object)typeof(ITab_Pawn_Magic) + ": " + (object)ex);
            }
        }

        public void ResolveMagicPowers()
        {
            if (this.MagicData.magicPowersInitialized)
                return;
            this.MagicData.magicPowersInitialized = true;
            Trait trait1 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfBeastsTrait);
            Trait trait2 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfTzeentchTrait);
            Trait trait3 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfShadowTrait);
            Trait trait4 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfDeathTrait);
            Trait trait5 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfWildTrait);
            Trait trait6 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfFireTrait);
            Trait trait7 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfHeavensTrait);
            Trait trait8 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfMetalTrait);
            Trait trait9 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfLightTrait);
            Trait trait10 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfLifeTrait);
            Trait trait11 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfNurgleTrait);
            Trait trait12 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfChaosUndividedTrait);
            Trait trait13 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfSlaaneshTrait);
            Trait trait14 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfHighTrait);
            Trait trait15 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfPlagueTrait);
            Trait trait16 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfGreatMawTrait);
            Trait trait17 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfWarpTrait);
            Trait trait18 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_LoreOfStealthTrait);

            if (trait1 != null)
            {
                switch (trait1.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersBeast.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        return;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersBeast.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        return;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersBeast.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        return;
                    case 4:
                        for (int index = 0; index < 8; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersBeast.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        return;
                }
            }
            if (trait2 != null)
            {
                switch (trait2.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersTzeentch.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersTzeentch.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersTzeentch.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersTzeentch.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait3 != null)
            {
                switch (trait3.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersShadow.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersShadow.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersShadow.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersShadow.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait4 != null)
            {
                switch (trait4.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersDeath.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersDeath.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersDeath.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersDeath.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait5 != null)
            {
                switch (trait5.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersWild.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersWild.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersWild.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersWild.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait6 != null)
            {
                switch (trait6.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersFire.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersFire.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersFire.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersFire.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait7 != null)
            {
                switch (trait7.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersHeavens.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersHeavens.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersHeavens.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersHeavens.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait8 != null)
            {
                switch (trait8.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersMetal.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersMetal.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersMetal.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersMetal.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait9 != null)
            {
                switch (trait9.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersLight.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersLight.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersLight.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersLight.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait10 != null)
            {
                switch (trait10.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersLife.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersLife.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersLife.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersLife.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait11 != null)
            {
                switch (trait11.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersNurgle.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersNurgle.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersNurgle.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersNurgle.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait12 != null)
            {
                switch (trait12.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersChaos.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersChaos.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersChaos.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersChaos.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait13 != null)
            {
                switch (trait13.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersSlaanesh.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersSlaanesh.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersSlaanesh.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersSlaanesh.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait14 != null)
            {
                switch (trait14.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersHigh.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersHigh.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersHigh.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersHigh.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait15 != null)
            {
                switch (trait15.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersPlague.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersPlague.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersPlague.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersPlague.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait16 != null)
            {
                switch (trait16.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersGreatMaw.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersGreatMaw.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersGreatMaw.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersGreatMaw.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait17 != null)
            {
                switch (trait17.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersWarp.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersWarp.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersWarp.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersWarp.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
            if (trait18 != null)
            {
                switch (trait18.Degree)
                {
                    case 0:
                    case 1:
                        for (int index = 0; index < 1; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersStealth.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 2)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 2:
                        for (int index = 0; index < 3; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersStealth.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 3)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 3:
                        for (int index = 0; index < 6; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersStealth.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 4)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                    case 4:
                        for (int index = 0; index < 10; ++index)
                        {
                            ++this.MagicUserLevel;
                            this.LevelUpPower(this.MagicData.PowersStealth.InRandomOrder<MagicPower>((IList<MagicPower>)null).First<MagicPower>((Func<MagicPower, bool>)(x => x.level < 5)));
                            --this.MagicData.AbilityPoints;
                        }
                        break;
                }
            }
        }

        public void ResolveMagicPool()
        {
            if (this.MagicPool is null)
                return;
            Hediff firstHediffOfDef = this.Pawn.health.hediffSet.GetFirstHediffOfDef(RH_TET_MagicDefOf.RH_TET_MagicWielder, false);
            if (firstHediffOfDef != null)
            {
                firstHediffOfDef.Severity = 1f;
            }
            else
            {
                Hediff hediff = HediffMaker.MakeHediff(RH_TET_MagicDefOf.RH_TET_MagicWielder, this.Pawn, (BodyPartRecord)null);
                hediff.Severity = 1f;
                this.Pawn.health.AddHediff(hediff, (BodyPartRecord)null, new DamageInfo?(), (DamageWorker.DamageResult)null);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<MagicData>(ref this.magicData, "RH_TET_Magic_magicData", (object)this);
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            List<MagicAbility> magicAbilityList = new List<MagicAbility>();
            if (this.MagicData == null || this.MagicData.Powers.Count<MagicPower>() <= 0)
                return;
            foreach (MagicPower power1 in this.MagicData.Powers)
            {
                MagicPower power = power1;
                if (power.abilityDef != null && power.level > 0 && ((IEnumerable<PawnAbility>)this.AbilityData.Powers).FirstOrDefault<PawnAbility>((Func<PawnAbility, bool>)(x => x.Def == power.abilityDef)) == null)
                    this.AddPawnAbility(power.abilityDef, true, (float)power.ticksUntilNextCast);
            }
        }

        public CompMagicUser()
            : base()
        {
        }
    }
}
