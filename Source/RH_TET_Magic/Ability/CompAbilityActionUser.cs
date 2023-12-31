
using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompAbilityActionUser : CompAbilityUser
    {
        private AbilityActionData abilityActionData;

        public AbilityActionData AbilityActionData
        {
            get
            {
                return this.abilityActionData ?? (this.abilityActionData = new AbilityActionData(this));
            }
        }

        public int AbilityActionPowerPoints
        {
            get
            {
                int num = 0;
                if ((this.AbilityActionData.PowersWitchHunter != null && this.AbilityActionData.PowersWitchHunter.Count > 0)
                    //|| (this.AbilityActionData.PowersShallya != null && this.AbilityActionData.PowersShallya.Count > 0)
                    //|| (this.AbilityActionData.PowersUlric != null && this.AbilityActionData.PowersUlric.Count > 0)
                    )
                {
                    foreach (AbilityActionPower abilityActionPower in this.AbilityActionData.PowersWitchHunter)
                        num += abilityActionPower.level;
                }
                return num;
            }
        }

        public void ResetPowers()
        {
            foreach (AbilityActionPower abilityActionPower in this.AbilityActionData.PowersWitchHunter)
                abilityActionPower.level = 0;

            List<AbilityActionAbility> abilityList = new List<AbilityActionAbility>();
            using (List<PawnAbility>.Enumerator enumerator = this.AbilityData.Powers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PawnAbility current = enumerator.Current;
                    abilityList.Add(current as AbilityActionAbility);
                }
            }
            foreach (PawnAbility pawnAbility in abilityList)
                this.RemovePawnAbility(pawnAbility.Def);
            this.UpdateAbilities();
        }

        public void GrantPower(AbilityActionPower power)
        {
            using (List<AbilityUser.AbilityDef>.Enumerator enumerator = power.abilityDefs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    this.RemovePawnAbility(enumerator.Current);
            }
            ++power.level;
            this.AddPawnAbility(power.abilityDef, true, -1f);
        }

        public AbilityActionType AbilityActionType
        {
            get
            {
                if (this.Pawn.story.traits.HasTrait(RH_TET_MagicDefOf.RH_TET_WitchHunterTrait))
                    return AbilityActionType.WitchHunter;
                else
                    return AbilityActionType.None;
            }
        }

        public Need_AbilityPool AbilityPool
        {
            get
            {
                return this.AbilityUser.needs.TryGetNeed<Need_AbilityPool>();
            }
        }

        public bool IsAbilityUser
        {
            get
            {
                return this.Pawn.IsAbilityUser();
            }
        }

        public override bool TryTransformPawn()
        {
            return this.IsAbilityUser;
        }

        public override List<HediffDef> IgnoredHediffs()
        {
            return new List<HediffDef>()
            {
                RH_TET_MagicDefOf.RH_TET_AbilityActionWielder
            };
        }

        public override void CompTick()
        {
            if (this.AbilityUser == null || !this.AbilityUser.Spawned || (Find.TickManager.TicksGame <= 200 || !this.IsAbilityUser))
                return;

            if (!this.AbilityActionData.AbilityPowersInitialized)
            {
                this.PostInitializeTick();
                this.AbilityActionData.AbilityPowersInitialized = true;
            }
            if (!this.AbilityActionData.TabResolved)
            {
                this.ResolveAbilityTab();
                this.AbilityActionData.TabResolved = true;
            }

            base.CompTick();
        }

        public void PostInitializeTick()
        {
            if (this.AbilityUser == null || !this.AbilityUser.Spawned || this.AbilityUser.story == null)
                return;

            this.Initialize();
            this.ResolveAbilityTab();
            this.ResolveAbilityPowers();
            this.ResolveAbilityPool();
        }

        public void ResolveAbilityTab()
        {
            IEnumerable<InspectTabBase> inspectTabs = this.AbilityUser.GetInspectTabs();
            if (inspectTabs == null || inspectTabs.Count<InspectTabBase>() <= 0)
                return;
            if (inspectTabs.FirstOrDefault<InspectTabBase>((Func<InspectTabBase, bool>)(x => x is ITab_Pawn_AbilityAction)) != null)
                return;
            try
            {
                this.AbilityUser.def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_AbilityAction)));
            }
            catch (Exception ex)
            {
                Log.Error("Could not instantiate inspector tab of type " + (object)typeof(ITab_Pawn_AbilityAction) + ": " + (object)ex, false);
            }
        }

        private AbilityActionPower GetUnusedPowerWitchHunter()
        {
            AbilityActionPower newPower = this.AbilityActionData.PowersWitchHunter.InRandomOrder<AbilityActionPower>((IList<AbilityActionPower>)null).First<AbilityActionPower>();

            return newPower;
        }

        public void ResolveAbilityPowers()
        {
            if (this.AbilityActionData.abilityPowersInitialized)
                return;
            this.AbilityActionData.abilityPowersInitialized = true;
            Trait trait1 = this.Pawn.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_WitchHunterTrait);
            //Trait trait2 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_ShallyaTrait);
            //Trait trait3 = this.AbilityUser.story.traits.GetTrait(RH_TET_MagicDefOf.RH_TET_UlricTrait);

            if (trait1 != null)
            {
                for (int i = 0; i < trait1.Degree; i++)
                {
                    AbilityActionPower newAbilityPower = GetUnusedPowerWitchHunter();
                    if (newAbilityPower != null)
                    {
                        this.GrantPower(newAbilityPower);
                        --this.AbilityActionData.AbilityPoints;
                    }
                }
            }
            //else if (trait2 != null)
            //{
            //    for (int i = 0; i < trait2.Degree; i++)
            //    {
            //        FaithPower newFaithPower = GetUnusedPowerShallya();
            //        if (newFaithPower != null)
            //        {
            //            this.GrantPower(newFaithPower);
            //            --this.FaithData.AbilityPoints;
            //        }
            //    }
            //}
            //else if (trait3 != null)
            //{
            //    for (int i = 0; i < trait3.Degree; i++)
            //    {
            //        FaithPower newFaithPower = GetUnusedPowerUlric();
            //        if (newFaithPower != null)
            //        {
            //            this.GrantPower(newFaithPower);
            //            --this.FaithData.AbilityPoints;
            //        }
            //    }
            //}
        }

        public void ResolveAbilityPool()
        {
            if (this.AbilityPool != null)
                return;
            Hediff firstHediffOfDef = this.AbilityUser.health.hediffSet.GetFirstHediffOfDef(RH_TET_MagicDefOf.RH_TET_AbilityActionWielder, false);
            if (firstHediffOfDef != null)
            {
                firstHediffOfDef.Severity = 1f;
            }
            else
            {
                Hediff hediff = HediffMaker.MakeHediff(RH_TET_MagicDefOf.RH_TET_AbilityActionWielder, this.AbilityUser, (BodyPartRecord)null);
                hediff.Severity = 1f;
                this.AbilityUser.health.AddHediff(hediff, (BodyPartRecord)null, new DamageInfo?(), (DamageWorker.DamageResult)null);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<AbilityActionData>(ref this.abilityActionData, "RH_TET_Magic_AbilityActionData", (object)this);
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            List<AbilityActionAbility> abilityList = new List<AbilityActionAbility>();
            if (this.AbilityActionData == null || this.AbilityActionData.Powers.Count<AbilityActionPower>() <= 0)
                return;
            foreach (AbilityActionPower power1 in this.AbilityActionData.Powers)
            {
                AbilityActionPower power = power1;
                if (power.abilityDef != null && power.level > 0 && ((IEnumerable<PawnAbility>)this.AbilityData.Powers).FirstOrDefault<PawnAbility>((Func<PawnAbility, bool>)(x => x.Def == power.abilityDef)) == null)
                    this.AddPawnAbility(power.abilityDef, true, (float)power.ticksUntilNextCast);
            }
        }

        public CompAbilityActionUser()
            : base()
        {
        }
    }
}
