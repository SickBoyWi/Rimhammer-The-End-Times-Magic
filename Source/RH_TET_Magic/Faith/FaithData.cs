using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class FaithData : IExposable
    {
        private Pawn pawn;
        private int abilityPoints;
        private int ticksUntilMeditate;
        public bool faithPowersInitialized;
        public bool TabResolved;
        private List<FaithPower> powersSigmar;
        private List<FaithPower> powersShallya;
        private List<FaithPower> powersUlric;

        public bool FaithPowersInitialized
        {
            get
            {
                return this.faithPowersInitialized;
            }
            set
            {
                this.faithPowersInitialized = value;
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

        public int TicksUntilMeditate
        {
            get
            {
                return this.ticksUntilMeditate;
            }
            set
            {
                this.ticksUntilMeditate = value;
            }
        }

        public Pawn Pawn
        {
            get
            {
                return this.pawn;
            }
        }

        public List<FaithPower> PowersSigmar
        {
            get
            {
                if (this.powersSigmar == null)
                {
                    List<FaithPower> powerList = new List<FaithPower>();
                    List<AbilityUser.AbilityDef> newAbilityDefs1 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_Sigmar_Thunderbolt);
                    powerList.Add(new FaithPower(newAbilityDefs1));
                    List<AbilityUser.AbilityDef> newAbilityDefs2 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_Sigmar_Comet);
                    powerList.Add(new FaithPower(newAbilityDefs2));
                    List<AbilityUser.AbilityDef> newAbilityDefs3 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_Sigmar_Shield);
                    powerList.Add(new FaithPower(newAbilityDefs3));
                    this.powersSigmar = powerList;
                }
                return this.powersSigmar;
            }
        }

        public List<FaithPower> PowersShallya
        {
            get
            {
                if (this.powersShallya == null)
                {
                    List<FaithPower> powerList = new List<FaithPower>();
                    List<AbilityUser.AbilityDef> newAbilityDefs1 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_Shallya_Light);
                    powerList.Add(new FaithPower(newAbilityDefs1));
                    List<AbilityUser.AbilityDef> newAbilityDefs2 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_Shallya_Regrowth);
                    powerList.Add(new FaithPower(newAbilityDefs2));
                    List<AbilityUser.AbilityDef> newAbilityDefs3 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_Shallya_Fortune);
                    powerList.Add(new FaithPower(newAbilityDefs3));
                    this.powersShallya = powerList;
                }
                return this.powersShallya;
            }
        }

        public List<FaithPower> PowersUlric
        {
            get
            {
                if (this.powersUlric == null)
                {
                    List<FaithPower> powerList = new List<FaithPower>();
                    List<AbilityUser.AbilityDef> newAbilityDefs1 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_Ulric_Winter);
                    powerList.Add(new FaithPower(newAbilityDefs1)); 
                    List<AbilityUser.AbilityDef> newAbilityDefs2 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_Ulric_Wolf);
                    powerList.Add(new FaithPower(newAbilityDefs2));
                    List<AbilityUser.AbilityDef> newAbilityDefs3 = new List<AbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_Ulric_Warhammer);
                    powerList.Add(new FaithPower(newAbilityDefs3));
                    this.powersUlric = powerList;
                }
                return this.powersUlric;
            }
        }

        public IEnumerable<FaithPower> Powers
        {
            get
            {
                return this.PowersSigmar.Concat<FaithPower>(
                    this.PowersShallya.Concat<FaithPower>(
                        this.PowersUlric));
            }
        }

        public FaithData()
        {
        }

        public FaithData(CompFaithUser newUser)
        {
            this.pawn = newUser.AbilityUser;
        }

        public void ExposeData()
        {
            Scribe_References.Look<Pawn>(ref this.pawn, "faithDataPawn", false);
            Scribe_Values.Look<bool>(ref this.faithPowersInitialized, "faithDataPowersInitialized", true, false);
            Scribe_Values.Look<int>(ref this.abilityPoints, "faithDataAbilityPoints", 0, false);
            Scribe_Values.Look<int>(ref this.ticksUntilMeditate, "faithDataTicksUntilMeditate", 0, false);
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (this.PawnHasAPowerIn(this.powersSigmar))
                    Scribe_Collections.Look<FaithPower>(ref this.powersSigmar, "faithDataPowersSigmar", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersShallya))
                    Scribe_Collections.Look<FaithPower>(ref this.powersShallya, "faithDataPowersShallya", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersUlric))
                    Scribe_Collections.Look<FaithPower>(ref this.powersUlric, "faithDataPowersUlric", LookMode.Deep);
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Scribe_Collections.Look<FaithPower>(ref this.powersSigmar, "faithDataPowersSigmar", LookMode.Deep);
                Scribe_Collections.Look<FaithPower>(ref this.powersShallya, "faithDataPowersShallya", LookMode.Deep);
                Scribe_Collections.Look<FaithPower>(ref this.powersUlric, "faithDataPowersUlric", LookMode.Deep);
            }
        }

        private bool PawnHasAPowerIn(List<FaithPower> powers)
        {
            if (powers != null && powers.Count > 0)
            {
                foreach (FaithPower power in powers)
                {
                    if (power.level > 0)
                        return true;
                }
            }
            return false;
        }
    }
}
