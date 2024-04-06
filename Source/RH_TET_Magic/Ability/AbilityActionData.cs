using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class AbilityActionData : IExposable
    {
        private Pawn pawn;
        private int abilityPoints;
        //private int ticksUntilMeditate;
        public bool abilityPowersInitialized;
        public bool TabResolved;
        private List<AbilityActionPower> powersWitchHunter;

        public bool AbilityPowersInitialized
        {
            get
            {
                return this.abilityPowersInitialized;
            }
            set
            {
                this.abilityPowersInitialized = value;
            }
        }

        public int AbilityPoints
        {
            get
            {
                return this.abilityPoints;
            }
            set
            {
                this.abilityPoints = value;
            }
        }

        public Pawn Pawn
        {
            get
            {
                return this.pawn;
            }
        }

        public List<AbilityActionPower> PowersWitchHunter
        {
            get
            {
                if (this.powersWitchHunter.NullOrEmpty())
                {
                    // TODO NEW Ability HERE
                    List<AbilityActionPower> powerList = new List<AbilityActionPower>();

                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_Ability_WH_Condemn);
                    powerList.Add(new AbilityActionPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_Ability_WH_GrimResolve);
                    powerList.Add(new AbilityActionPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_Ability_WH_PurifyingFlame);
                    powerList.Add(new AbilityActionPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_Ability_WH_Execute);
                    powerList.Add(new AbilityActionPower(newAbilityDefs4));

                    this.powersWitchHunter = powerList;
                }
                return this.powersWitchHunter;
            }
        }

        public IEnumerable<AbilityActionPower> Powers
        {
            get
            {
                return this.PowersWitchHunter.AsEnumerable<AbilityActionPower>();
                // TODO NEW Ability HERE
                //return this.PowersSigmar.Concat<FaithPower>(
                //    this.PowersShallya.Concat<FaithPower>(
                //        this.PowersUlric));
            }
        }

        public AbilityActionData()
        {
        }

        public AbilityActionData(CompAbilityActionUser newUser)
        {
            this.pawn = newUser.Pawn;
        }

        public void ExposeData()
        {
            Scribe_References.Look<Pawn>(ref this.pawn, "abilityActionDataPawn", false);
            Scribe_Values.Look<bool>(ref this.abilityPowersInitialized, "abilityActionDataPowersInitialized", true, false);
            Scribe_Values.Look<int>(ref this.abilityPoints, "abilityActionDataAbilityPoints", 0, false);
            //Scribe_Values.Look<int>(ref this.ticksUntilMeditate, "abilityActionDataTicksUntilMeditate", 0, false);
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (this.PawnHasAPowerIn(this.powersWitchHunter))
                    Scribe_Collections.Look<AbilityActionPower>(ref this.powersWitchHunter, "abilityActionDataPowersWitchHunter", LookMode.Deep);
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Scribe_Collections.Look<AbilityActionPower>(ref this.powersWitchHunter, "abilityActionDataPowersWitchHunter", LookMode.Deep);
            }
        }

        private bool PawnHasAPowerIn(List<AbilityActionPower> powers)
        {
            if (powers != null && powers.Count > 0)
            {
                foreach (AbilityActionPower power in powers)
                {
                    if (power.level > 0)
                        return true;
                }
            }
            return false;
        }
    }
}
