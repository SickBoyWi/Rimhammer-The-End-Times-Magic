
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
    public class CompFaithUser : CompAbilityUser
    {
        private FaithData faithData;

        public FaithData FaithData
        {
            get
            {
                return this.faithData ?? (this.faithData = new FaithData(this));
            }
        }

        public int FaithPowerPoints
        {
            get
            {
                int num = 0;
                if ((this.FaithData.PowersSigmar != null && this.FaithData.PowersSigmar.Count > 0)
                    || (this.FaithData.PowersShallya != null && this.FaithData.PowersShallya.Count > 0)
                    || (this.FaithData.PowersUlric != null && this.FaithData.PowersUlric.Count > 0)
                    )
                {
                    foreach (FaithPower faithPower in this.FaithData.PowersSigmar)
                        num += faithPower.level;
                    foreach (FaithPower faithPower in this.FaithData.PowersShallya)
                        num += faithPower.level;
                    foreach (FaithPower faithPower in this.FaithData.PowersUlric)
                        num += faithPower.level;
                }
                return num;
            }
        }

        public void ResetPowers()
        {
            foreach (FaithPower faithPower in this.FaithData.PowersSigmar)
                faithPower.level = 0;
            foreach (FaithPower faithPower in this.FaithData.PowersShallya)
                faithPower.level = 0;
            foreach (FaithPower faithPower in this.FaithData.PowersUlric)
                faithPower.level = 0;

            List<FaithAbility> abilityList = new List<FaithAbility>();
            using (List<PawnAbility>.Enumerator enumerator = this.AbilityData.Powers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PawnAbility current = enumerator.Current;
                    abilityList.Add(current as FaithAbility);
                }
            }
            foreach (PawnAbility pawnAbility in abilityList)
                this.RemovePawnAbility(pawnAbility.Def);
            this.UpdateAbilities();
        }

        public void GrantPower(FaithPower power)
        {
            using (List<SickAbilityUser.AbilityDef>.Enumerator enumerator = power.abilityDefs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    this.RemovePawnAbility(enumerator.Current);
            }
            ++power.level;
            this.AddPawnAbility(power.abilityDef, true, -1f);
        }

        public FaithType FaithType
        {
            get
            {
                if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_SigmarTrait))
                    return FaithType.Sigmar;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_ShallyaTrait))
                    return FaithType.Shallya;
                else if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_UlricTrait))
                    return FaithType.Ulric;
                else
                    return FaithType.None;
            }
        }

        public Need_FaithPool FaithPool
        {
            get
            {
                return this.AbilityUser.needs.TryGetNeed<Need_FaithPool>();
            }
        }

        public bool IsFaithUser
        {
            get
            {
                return this.Pawn.IsFaithUser();
            }
        }

        public override bool TryTransformPawn()
        {
            return this.IsFaithUser;
        }

        public override List<HediffDef> IgnoredHediffs()
        {
            return new List<HediffDef>()
            {
                RH_TET_MagicDefOf.RH_TET_FaithWielder
            };
        }

        public override void CompTick()
        {
            if (this.AbilityUser == null || !this.AbilityUser.Spawned || (Find.TickManager.TicksGame <= 200 || !this.IsFaithUser))
                return;
            
            if (!this.FaithData.FaithPowersInitialized)
            {
                this.PostInitializeTick();
                this.FaithData.FaithPowersInitialized = true;
            }
            if (!this.FaithData.TabResolved)
            {
                this.ResolveFaithTab();
                this.FaithData.TabResolved = true;
            }

            base.CompTick();
        }

        public void PostInitializeTick()
        {
            if (this.AbilityUser == null || !this.AbilityUser.Spawned || this.AbilityUser.story == null)
                return;
            
            this.Initialize();
            this.ResolveFaithTab();
            this.ResolveFaithPowers();
            this.ResolveFaithPool();
        }

        public void ResolveFaithTab()
        {
            IEnumerable<InspectTabBase> inspectTabs = this.AbilityUser.GetInspectTabs();
            if (inspectTabs == null || inspectTabs.Count<InspectTabBase>() <= 0)
                return;
            if (inspectTabs.FirstOrDefault<InspectTabBase>((Func<InspectTabBase, bool>)(x => x is ITab_Pawn_Faith)) != null)
                return;
            try
            {
                this.AbilityUser.def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Faith)));
            }
            catch (Exception ex)
            {
                Log.Error("Could not instantiate inspector tab of type " + (object)typeof(ITab_Pawn_Faith) + ": " + (object)ex, false);
            }
        }

        private FaithPower GetUnusedPowerSigmar()
        {
            FaithPower newFaithPower = this.FaithData.PowersSigmar.InRandomOrder<FaithPower>((IList<FaithPower>)null).First<FaithPower>();

            return newFaithPower;
        }

        private FaithPower GetUnusedPowerShallya()
        {
            FaithPower newFaithPower = this.FaithData.PowersShallya.InRandomOrder<FaithPower>((IList<FaithPower>)null).First<FaithPower>();

            return newFaithPower;
        }

        private FaithPower GetUnusedPowerUlric()
        {
            FaithPower newFaithPower = this.FaithData.PowersUlric.InRandomOrder<FaithPower>((IList<FaithPower>)null).First<FaithPower>();

            return newFaithPower;
        }

        public void ResolveFaithPowers()
        {
            if (this.FaithData.faithPowersInitialized)
                return;
            this.FaithData.faithPowersInitialized = true;
            Trait trait1 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_SigmarTrait);
            Trait trait2 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_ShallyaTrait);
            Trait trait3 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_UlricTrait);

            if (trait1 != null)
            {
                for (int i = 0; i < trait1.Degree; i++)
                {
                    FaithPower newFaithPower = GetUnusedPowerSigmar();
                    if (newFaithPower != null)
                    {
                        this.GrantPower(newFaithPower);
                        --this.FaithData.AbilityPoints;
                    }
                }
            }
            else if (trait2 != null)
            {
                for (int i = 0; i < trait2.Degree; i++)
                {
                    FaithPower newFaithPower = GetUnusedPowerShallya();
                    if (newFaithPower != null)
                    {
                        this.GrantPower(newFaithPower);
                        --this.FaithData.AbilityPoints;
                    }
                }
            }
            else if (trait3 != null)
            {
                for (int i = 0; i < trait3.Degree; i++)
                {
                    FaithPower newFaithPower = GetUnusedPowerUlric();
                    if (newFaithPower != null)
                    {
                        this.GrantPower(newFaithPower);
                        --this.FaithData.AbilityPoints;
                    }
                }
            }
        }

        public void ResolveFaithPool()
        {
            if (this.FaithPool != null)
                return;
            Hediff firstHediffOfDef = this.AbilityUser.health.hediffSet.GetFirstHediffOfDef(RH_TET_MagicDefOf.RH_TET_FaithWielder, false);
            if (firstHediffOfDef != null)
            {
                firstHediffOfDef.Severity = 1f;
            }
            else
            {
                Hediff hediff = HediffMaker.MakeHediff(RH_TET_MagicDefOf.RH_TET_FaithWielder, this.AbilityUser, (BodyPartRecord)null);
                hediff.Severity = 1f;
                this.AbilityUser.health.AddHediff(hediff, (BodyPartRecord)null, new DamageInfo?(), (DamageWorker.DamageResult)null);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<FaithData>(ref this.faithData, "RH_TET_Magic_faithData", (object)this);
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            List<FaithAbility> abilityList = new List<FaithAbility>();
            if (this.FaithData == null || this.FaithData.Powers.Count<FaithPower>() <= 0)
                return;
            foreach (FaithPower power1 in this.FaithData.Powers)
            {
                FaithPower power = power1;
                if (power.abilityDef != null && power.level > 0 && ((IEnumerable<PawnAbility>)this.AbilityData.Powers).FirstOrDefault<PawnAbility>((Func<PawnAbility, bool>)(x => x.Def == power.abilityDef)) == null)
                    this.AddPawnAbility(power.abilityDef, true, (float)power.ticksUntilNextCast);
            }
        }

        public CompFaithUser()
            : base()
        {
        }
    }
}
