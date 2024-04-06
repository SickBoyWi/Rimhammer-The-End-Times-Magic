using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class MagicData : IExposable
    {
        private int xp = 1;
        private int ticksUntilXPGain = -1;
        private Pawn pawn;
        private int level;
        private int abilityPoints;
        private int ticksUntilMeditate;
        public bool magicPowersInitialized;
        public bool TabResolved;
        private List<MagicPower> powersBeasts;
        private List<MagicPower> powersTzeentch;
        private List<MagicPower> powersWild;
        private List<MagicPower> powersShadow;
        private List<MagicPower> powersDeath;
        private List<MagicPower> powersFire;
        private List<MagicPower> powersLight;
        private List<MagicPower> powersMetal;
        private List<MagicPower> powersHeavens;
        private List<MagicPower> powersLife;
        private List<MagicPower> powersNurgle;
        private List<MagicPower> powersChaos;
        private List<MagicPower> powersSlaanesh;
        private List<MagicPower> powersHigh;
        private List<MagicPower> powersPlague;
        private List<MagicPower> powersGreatMaw;
        private List<MagicPower> powersWarp;
        private List<MagicPower> powersStealth;
        private List<MagicPower> powersAddons;

        public bool MagicPowersInitialized
        {
            get
            {
                return this.magicPowersInitialized;
            }
            set
            {
                this.magicPowersInitialized = value;
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
            }
        }

        public int XP
        {
            get
            {
                return this.xp;
            }
            set
            {
                this.xp = value;
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

        public int TicksUntilXPGain
        {
            get
            {
                return this.ticksUntilXPGain;
            }
            set
            {
                this.ticksUntilXPGain = value;
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

        public List<MagicPower> PowersWild
        {
            get
            {
                if (this.powersWild == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_WildBrayScream_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_WildBrayScream_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_WildBrayScream_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_WildBrayScream_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_WildSavageDominion_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_WildSavageDominion_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_WildSavageDominion_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_WildSavageDominion_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2)); 
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_WildMantleGhorok_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_WildMantleGhorok_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_WildMantleGhorok_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_WildMantleGhorok_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3)); 
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_WildDevolve_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_WildDevolve_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_WildDevolve_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_WildDevolve_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_WildBestialSurge_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_WildBestialSurge_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_WildBestialSurge_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_WildBestialSurge_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersWild = magicPowerList;
                }
                return this.powersWild;
            }
        }

        public List<MagicPower> PowersShadow
        {
            get
            {
                if (this.powersShadow  == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_ShadowBewilder_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_ShadowBewilder_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_ShadowBewilder_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_ShadowBewilder_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1)); 
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_ShadowSteedOfShadows_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_ShadowSteedOfShadows_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_ShadowSteedOfShadows_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_ShadowSteedOfShadows_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2)); 
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_ShadowPenumbralPendulum_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_ShadowPenumbralPendulum_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_ShadowPenumbralPendulum_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_ShadowPenumbralPendulum_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3)); 
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_ShadowMindSlip_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_ShadowMindSlip_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_ShadowMindSlip_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_ShadowMindSlip_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_ShadowEnfeeblingFoe_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_ShadowEnfeeblingFoe_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_ShadowEnfeeblingFoe_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_ShadowEnfeeblingFoe_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersShadow = magicPowerList;
                }
                return this.powersShadow;
            }
        }

        public List<MagicPower> PowersDeath
        {
            get
            {
                if (this.powersDeath == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>(); 
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_DeathManacles_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_DeathManacles_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_DeathManacles_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_DeathManacles_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1)); 
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_DeathPurpleSun_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_DeathPurpleSun_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_DeathPurpleSun_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_DeathPurpleSun_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_DeathStealLife_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_DeathStealLife_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_DeathStealLife_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_DeathStealLife_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3)); 
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_DeathWindOfDeath_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_DeathWindOfDeath_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_DeathWindOfDeath_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_DeathWindOfDeath_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_DeathSoulBlight_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_DeathSoulBlight_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_DeathSoulBlight_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_DeathSoulBlight_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersDeath = magicPowerList;
                }
                return this.powersDeath;
            }
        }

        public List<MagicPower> PowersBeast
        {
            get
            {
                if (this.powersBeasts == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_BeastsFlockOfDoom_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_BeastsFlockOfDoom_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_BeastsFlockOfDoom_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_BeastsFlockOfDoom_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_BeastsAmberSpear_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_BeastsAmberSpear_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_BeastsAmberSpear_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_BeastsAmberSpear_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_BeastsAmberTrance_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_BeastsAmberTrance_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_BeastsAmberTrance_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_BeastsAmberTrance_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_BeastsWyssonsWildform_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_BeastsWyssonsWildform_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_BeastsWyssonsWildform_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_BeastsWyssonsWildform_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_BeastsBeastMaster_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_BeastsBeastMaster_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_BeastsBeastMaster_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_BeastsBeastMaster_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersBeasts = magicPowerList;
                }
                return this.powersBeasts;
            }
        }

        public List<MagicPower> PowersTzeentch
        {
            get
            {
                if (this.powersTzeentch == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_TzeentchBolt_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_TzeentchBolt_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_TzeentchBolt_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_TzeentchBolt_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_TzeentchPinkFire_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_TzeentchPinkFire_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_TzeentchPinkFire_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_TzeentchPinkFire_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_GreenTzeentchBolt_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_GreenTzeentchBolt_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_GreenTzeentchBolt_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_GreenTzeentchBolt_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_TzeenchFireOrange_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_TzeenchFireOrange_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_TzeenchFireOrange_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_TzeenchFireOrange_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_TzeentchIndigoFire_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_TzeentchIndigoFire_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_TzeentchIndigoFire_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_TzeentchIndigoFire_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersTzeentch = magicPowerList;
                }
                return this.powersTzeentch;
            }
        }

        public List<MagicPower> PowersFire
        {
            get
            {
                if (this.powersFire == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_FireAegis_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_FireAegis_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_FireAegis_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_FireAegis_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_FireBreatheFire_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_FireBreatheFire_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_FireBreatheFire_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_FireBreatheFire_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_FireCascadingCloak_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_FireCascadingCloak_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_FireCascadingCloak_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_FireCascadingCloak_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_FirePiercingBolts_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_FirePiercingBolts_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_FirePiercingBolts_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_FirePiercingBolts_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_FireFlameStorm_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_FireFlameStorm_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_FireFlameStorm_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_FireFlameStorm_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersFire = magicPowerList;
                }
                return this.powersFire;
            }
        }

        public List<MagicPower> PowersHeavens
        {
            get
            {
                if (this.powersHeavens == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_HeavensUrannonsThunderbolt_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_HeavensUrannonsThunderbolt_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_HeavensUrannonsThunderbolt_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_HeavensUrannonsThunderbolt_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_HeavensCometOfCassandora_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_HeavensCometOfCassandora_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_HeavensCometOfCassandora_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_HeavensCometOfCassandora_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_HeavensHarmonicConvergence_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_HeavensHarmonicConvergence_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_HeavensHarmonicConvergence_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_HeavensHarmonicConvergence_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_HeavensClearSky_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_HeavensClearSky_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_HeavensClearSky_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_HeavensClearSky_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_HeavensAzureBlades_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_HeavensAzureBlades_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_HeavensAzureBlades_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_HeavensAzureBlades_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersHeavens = magicPowerList;
                }
                return this.powersHeavens;
            }
        }

        public List<MagicPower> PowersMetal
        {
            get
            {
                if (this.powersMetal == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_MetalFinalTransmutation_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_MetalFinalTransmutation_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_MetalFinalTransmutation_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_MetalFinalTransmutation_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_MetalForgeOfChamon_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_MetalForgeOfChamon_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_MetalForgeOfChamon_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_MetalForgeOfChamon_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_MetalGleamingArrow_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_MetalGleamingArrow_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_MetalGleamingArrow_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_MetalGleamingArrow_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_MetalMindTransmute_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_MetalMindTransmute_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_MetalMindTransmute_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_MetalMindTransmute_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_MetalGlitteringRobe_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_MetalGlitteringRobe_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_MetalGlitteringRobe_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_MetalGlitteringRobe_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersMetal = magicPowerList;
                }
                return this.powersMetal;
            }
        }

        public List<MagicPower> PowersLight
        {
            get
            {
                if (this.powersLight == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_LightAbullasSnare_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_LightAbullasSnare_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_LightAbullasSnare_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_LightAbullasSnare_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_LightClawOfApek_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_LightClawOfApek_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_LightClawOfApek_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_LightClawOfApek_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_LightHealingLight_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_LightHealingLight_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_LightHealingLight_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_LightHealingLight_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_LightBurningGaze_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_LightBurningGaze_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_LightBurningGaze_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_LightBurningGaze_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_LightIlluminateTheEdifice_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_LightIlluminateTheEdifice_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_LightIlluminateTheEdifice_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_LightIlluminateTheEdifice_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersLight = magicPowerList;
                }
                return this.powersLight;
            }
        }

        public List<MagicPower> PowersLife
        {
            get
            {
                if (this.powersLife == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_LifeWinterFrost_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_LifeWinterFrost_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_LifeWinterFrost_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_LifeWinterFrost_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_LifeDrainLife_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_LifeDrainLife_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_LifeDrainLife_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_LifeDrainLife_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_LifeRegenerate_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_LifeRegenerate_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_LifeRegenerate_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_LifeRegenerate_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_LifeCureBlight_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_LifeCureBlight_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_LifeCureBlight_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_LifeCureBlight_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_LifeLifebloom_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_LifeLifebloom_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_LifeLifebloom_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_LifeLifebloom_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersLife = magicPowerList;
                }
                return this.powersLife;
            }
        }

        public List<MagicPower> PowersNurgle
        {
            get
            {
                if (this.powersNurgle == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_NurgleAfflictions_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_NurgleAfflictions_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_NurgleAfflictions_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_NurgleAfflictions_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_NurgleBoon_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_NurgleBoon_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_NurgleBoon_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_NurgleBoon_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_NurgleStream_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_NurgleStream_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_NurgleStream_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_NurgleStream_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_NurglePestilence_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_NurglePestilence_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_NurglePestilence_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_NurglePestilence_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_NurgleRotbomb_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_NurgleRotbomb_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_NurgleRotbomb_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_NurgleRotbomb_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersNurgle = magicPowerList;
                }
                return this.powersNurgle;
            }
        }

        public List<MagicPower> PowersChaos
        {
            get
            {
                if (this.powersChaos == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_ChaosVisionOfTorment_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_ChaosVisionOfTorment_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_ChaosVisionOfTorment_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_ChaosVisionOfTorment_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_ChaosBoon_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_ChaosBoon_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_ChaosBoon_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_ChaosBoon_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_ChaosWordOfPain_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_ChaosWordOfPain_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_ChaosWordOfPain_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_ChaosWordOfPain_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_ChaosBurningBlood_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_ChaosBurningBlood_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_ChaosBurningBlood_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_ChaosBurningBlood_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_ChaosVeil_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_ChaosVeil_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_ChaosVeil_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_ChaosVeil_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersChaos = magicPowerList;
                }
                return this.powersChaos;
            }
        }

        public List<MagicPower> PowersSlaanesh
        {
            get
            {
                if (this.powersSlaanesh == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshGoldenTorrent_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshGoldenTorrent_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshGoldenTorrent_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshGoldenTorrent_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshBlissful_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshBlissful_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshBlissful_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshBlissful_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshDelectableTorture_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshDelectableTorture_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshDelectableTorture_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshDelectableTorture_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshSlothfulStupor_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshSlothfulStupor_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshSlothfulStupor_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshSlothfulStupor_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshPainPleasure_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshPainPleasure_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshPainPleasure_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_SlaaneshPainPleasure_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersSlaanesh = magicPowerList;
                }
                return this.powersSlaanesh;
            }
        }

        public List<MagicPower> PowersHigh
        {
            get
            {
                if (this.powersHigh == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_HighPhoenixFlames_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_HighPhoenixFlames_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_HighPhoenixFlames_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_HighPhoenixFlames_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_HighShieldSaphery_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_HighShieldSaphery_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_HighShieldSaphery_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_HighShieldSaphery_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_HighApotheosis_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_HighApotheosis_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_HighApotheosis_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_HighApotheosis_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_HighFuryOfKhaine_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_HighFuryOfKhaine_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_HighFuryOfKhaine_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_HighFuryOfKhaine_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_HighGlamourOfTeclis_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_HighGlamourOfTeclis_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_HighGlamourOfTeclis_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_HighGlamourOfTeclis_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersHigh = magicPowerList;
                }
                return this.powersHigh;
            }
        }

        public List<MagicPower> PowersPlague
        {
            get
            {
                if (this.powersPlague == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_PlagueRedPox_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_PlagueRedPox_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_PlagueRedPox_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_PlagueRedPox_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_PlagueInfectingGaze_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_PlagueInfectingGaze_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_PlagueInfectingGaze_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_PlagueInfectingGaze_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_PlaguePestilentBreath_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_PlaguePestilentBreath_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_PlaguePestilentBreath_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_PlaguePestilentBreath_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_PlagueAirOfPestilence_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_PlagueAirOfPestilence_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_PlagueAirOfPestilence_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_PlagueAirOfPestilence_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_PlagueCloudOfFlies_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_PlagueCloudOfFlies_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_PlagueCloudOfFlies_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_PlagueCloudOfFlies_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersPlague = magicPowerList;
                }
                return this.powersPlague;
            }
        }

        public List<MagicPower> PowersGreatMaw
        {
            get
            {
                if (this.powersGreatMaw == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_MawBonecrusher_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_MawBonecrusher_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_MawBonecrusher_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_MawBonecrusher_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_MawBraingobbler_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_MawBraingobbler_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_MawBraingobbler_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_MawBraingobbler_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_MawBullgorger_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_MawBullgorger_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_MawBullgorger_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_MawBullgorger_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_MawTrollguts_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_MawTrollguts_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_MawTrollguts_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_MawTrollguts_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_MawToothcracker_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_MawToothcracker_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_MawToothcracker_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_MawToothcracker_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersGreatMaw = magicPowerList;
                }
                return this.powersGreatMaw;
            }
        }

        public List<MagicPower> PowersWarp
        {
            get
            {
                if (this.powersWarp == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_WarpLightning_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_WarpLightning_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_WarpLightning_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_WarpLightning_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_WarpGhostlyFlame_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_WarpGhostlyFlame_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_WarpGhostlyFlame_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_WarpGhostlyFlame_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_WarpRatThrall_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_WarpRatThrall_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_WarpRatThrall_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_WarpRatThrall_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_WarpCurseHornedRat_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_WarpCurseHornedRat_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_WarpCurseHornedRat_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_WarpCurseHornedRat_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_WarpVerminousRuin_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_WarpVerminousRuin_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_WarpVerminousRuin_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_WarpVerminousRuin_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersWarp = magicPowerList;
                }
                return this.powersWarp;
            }
        }
        public List<MagicPower> PowersStealth
        {
            get
            {
                if (this.powersStealth == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    List<SickAbilityUser.AbilityDef> newAbilityDefs1 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_StealthSkitterleap_Mage);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_StealthSkitterleap_Wizard);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_StealthSkitterleap_Warlock);
                    newAbilityDefs1.Add(RH_TET_MagicDefOf.RH_TET_StealthSkitterleap_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs1));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs2 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_StealthSwiftscamper_Mage);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_StealthSwiftscamper_Wizard);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_StealthSwiftscamper_Warlock);
                    newAbilityDefs2.Add(RH_TET_MagicDefOf.RH_TET_StealthSwiftscamper_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs2));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs3 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_Stealth_ShadowsCompanion_Mage);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_Stealth_ShadowsCompanion_Wizard);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_Stealth_ShadowsCompanion_Warlock);
                    newAbilityDefs3.Add(RH_TET_MagicDefOf.RH_TET_Stealth_ShadowsCompanion_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs3));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs4 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_StealthArmorOfDarkness_Mage);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_StealthArmorOfDarkness_Wizard);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_StealthArmorOfDarkness_Warlock);
                    newAbilityDefs4.Add(RH_TET_MagicDefOf.RH_TET_StealthArmorOfDarkness_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs4));
                    List<SickAbilityUser.AbilityDef> newAbilityDefs5 = new List<SickAbilityUser.AbilityDef>();
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_StealthTracelessDemise_Mage);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_StealthTracelessDemise_Wizard);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_StealthTracelessDemise_Warlock);
                    newAbilityDefs5.Add(RH_TET_MagicDefOf.RH_TET_StealthTracelessDemise_Master);
                    magicPowerList.Add(new MagicPower(newAbilityDefs5));
                    this.powersStealth = magicPowerList;
                }
                return this.powersStealth;
            }
        }
        public List<MagicPower> PowersAddons
        {
            get
            {
                if (this.powersAddons == null)
                {
                    List<MagicPower> magicPowerList = new List<MagicPower>();
                    this.powersAddons = magicPowerList;
                }
                return this.powersAddons;
            }
        }

        public IEnumerable<MagicPower> Powers
        {
            get
            {
                return this.PowersBeast.Concat<MagicPower>(
                    this.PowersTzeentch.Concat<MagicPower>(
                        this.PowersWild.Concat<MagicPower>(
                            this.PowersDeath.Concat<MagicPower>(
                                this.PowersShadow.Concat<MagicPower>(
                                    this.PowersFire.Concat<MagicPower>(
                                        this.PowersHeavens.Concat<MagicPower>(
                                            this.PowersMetal.Concat<MagicPower>(
                                                this.PowersLight.Concat<MagicPower>(
                                                    this.PowersLife).Concat<MagicPower>(
                                                        this.PowersNurgle).Concat<MagicPower>(
                                                            this.PowersChaos).Concat<MagicPower>(
                                                                this.PowersSlaanesh).Concat<MagicPower>(
                                                                    this.PowersHigh).Concat<MagicPower>(
                                                                        this.PowersPlague).Concat<MagicPower>(
                                                                            this.PowersGreatMaw).Concat<MagicPower>(
                                                                                this.PowersWarp).Concat<MagicPower>(
                                                                                    this.PowersStealth).Concat<MagicPower>(
                                                                                        this.PowersAddons)))))))));
            }
        }

        public MagicData()
        {
        }

        public MagicData(CompMagicUser newUser)
        {
            this.pawn = newUser.AbilityUser;
        }

        public void ExposeData()
        {
            Scribe_References.Look<Pawn>(ref this.pawn, "magicDataPawn", false);
            Scribe_Values.Look<int>(ref this.level, "magicDataLevel", 0, false);
            Scribe_Values.Look<int>(ref this.xp, "magicDataXp", 0, false);
            Scribe_Values.Look<bool>(ref this.magicPowersInitialized, "magicDataPowersInitialized", true, false);
            Scribe_Values.Look<int>(ref this.abilityPoints, "magicDataAbilityPoints", 0, false);
            Scribe_Values.Look<int>(ref this.ticksUntilMeditate, "magicDataTicksUntilMeditate", 0, false);
            Scribe_Values.Look<int>(ref this.ticksUntilXPGain, "magicDataTicksUntilXPGain", -1, false);
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (this.PawnHasAPowerIn(this.powersBeasts, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersBeasts, "magicDataPowersBeasts", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersTzeentch, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersTzeentch, "magicDataPowersTzeentch", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersWild, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersWild, "magicDataPowersWild", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersShadow, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersShadow, "magicDataPowersShadow", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersDeath, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersDeath, "magicDataPowersDeath", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersFire, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersFire, "magicDataPowersFire", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersHeavens, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersHeavens, "magicDataPowersHeavens", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersMetal, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersMetal, "magicDataPowersMetal", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersLight, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersLight, "magicDataPowersLight", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersLife, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersLife, "magicDataPowersLife", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersNurgle, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersNurgle, "magicDataPowersNurgle", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersChaos, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersChaos, "magicDataPowersChaos", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersSlaanesh, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersSlaanesh, "magicDataPowersSlaanesh", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersHigh, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersHigh, "magicDataPowersHigh", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersPlague, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersPlague, "magicDataPowersPlague", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersGreatMaw, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersGreatMaw, "magicDataPowersGreatMaw", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersWarp, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersWarp, "magicDataPowersWarp", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersStealth, false))
                    Scribe_Collections.Look<MagicPower>(ref this.powersStealth, "magicDataPowersStealth", LookMode.Deep);
                if (this.PawnHasAPowerIn(this.powersAddons, true))
                    Scribe_Collections.Look<MagicPower>(ref this.powersAddons, "magicDataPowersAddOns", LookMode.Deep);
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            { 
                Scribe_Collections.Look<MagicPower>(ref this.powersBeasts, "magicDataPowersBeasts", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersTzeentch, "magicDataPowersTzeentch", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersWild, "magicDataPowersWild", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersShadow, "magicDataPowersShadow", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersDeath, "magicDataPowersDeath", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersFire, "magicDataPowersFire", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersHeavens, "magicDataPowersHeavens", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersMetal, "magicDataPowersMetal", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersLight, "magicDataPowersLight", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersLife, "magicDataPowersLife", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersNurgle, "magicDataPowersNurgle", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersChaos, "magicDataPowersChaos", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersSlaanesh, "magicDataPowersSlaanesh", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersHigh, "magicDataPowersHigh", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersPlague, "magicDataPowersPlague", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersGreatMaw, "magicDataPowersGreatMaw", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersWarp, "magicDataPowersWarp", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersStealth, "magicDataPowersStealth", LookMode.Deep);
                Scribe_Collections.Look<MagicPower>(ref this.powersAddons, "magicDataPowersAddOns", LookMode.Deep);
            }
        }

        private bool PawnHasAPowerIn(List<MagicPower> powers, bool isAddOn)
        {
            if (powers != null && powers.Count > 0)
            {
                foreach (MagicPower power in powers)
                {
                    if (isAddOn || power.level > 0)
                        return true;
                }
            }
            return false;
        }
    }
}
