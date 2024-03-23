using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    [DefOf]
    public static class RH_TET_MagicDefOf
    {
        //// =============== Ability Action ===============
        ///  Witch Hunter
        public static AbilityActionAbilityDef RH_TET_Ability_WH_Condemn;
        public static AbilityActionAbilityDef RH_TET_Ability_WH_GrimResolve; 
        public static AbilityActionAbilityDef RH_TET_Ability_WH_PurifyingFlame;
        public static AbilityActionAbilityDef RH_TET_Ability_WH_Execute;

        //// =============== Faith ===============
        ///  Sigmar
        public static FaithAbilityDef RH_TET_Sigmar_Thunderbolt;
        public static FaithAbilityDef RH_TET_Sigmar_Comet;
        public static FaithAbilityDef RH_TET_Sigmar_Shield;
        /// Shallaya
        public static FaithAbilityDef RH_TET_Shallya_Light;
        public static FaithAbilityDef RH_TET_Shallya_Regrowth;
        public static FaithAbilityDef RH_TET_Shallya_Fortune;
        /// Ulric
        public static FaithAbilityDef RH_TET_Ulric_Winter;
        public static FaithAbilityDef RH_TET_Ulric_Wolf;
        public static FaithAbilityDef RH_TET_Ulric_Warhammer;

        //// =============== MAGIC ===============
        ///  Tzeentch
        public static MagicAbilityDef RH_TET_TzeentchBolt_Mage;
        public static MagicAbilityDef RH_TET_TzeentchBolt_Wizard;
        public static MagicAbilityDef RH_TET_TzeentchBolt_Warlock;
        public static MagicAbilityDef RH_TET_TzeentchBolt_Master;
        public static MagicAbilityDef RH_TET_TzeentchPinkFire_Mage;
        public static MagicAbilityDef RH_TET_TzeentchPinkFire_Wizard;
        public static MagicAbilityDef RH_TET_TzeentchPinkFire_Warlock;
        public static MagicAbilityDef RH_TET_TzeentchPinkFire_Master;
        public static MagicAbilityDef RH_TET_GreenTzeentchBolt_Mage;
        public static MagicAbilityDef RH_TET_GreenTzeentchBolt_Wizard;
        public static MagicAbilityDef RH_TET_GreenTzeentchBolt_Warlock;
        public static MagicAbilityDef RH_TET_GreenTzeentchBolt_Master; 
        public static MagicAbilityDef RH_TET_TzeenchFireOrange_Mage;
        public static MagicAbilityDef RH_TET_TzeenchFireOrange_Wizard;
        public static MagicAbilityDef RH_TET_TzeenchFireOrange_Warlock;
        public static MagicAbilityDef RH_TET_TzeenchFireOrange_Master;
        public static MagicAbilityDef RH_TET_TzeentchIndigoFire_Mage;
        public static MagicAbilityDef RH_TET_TzeentchIndigoFire_Wizard;
        public static MagicAbilityDef RH_TET_TzeentchIndigoFire_Warlock;
        public static MagicAbilityDef RH_TET_TzeentchIndigoFire_Master;
        ///  Beasts
        public static MagicAbilityDef RH_TET_BeastsFlockOfDoom_Mage;
        public static MagicAbilityDef RH_TET_BeastsFlockOfDoom_Wizard;
        public static MagicAbilityDef RH_TET_BeastsFlockOfDoom_Warlock;
        public static MagicAbilityDef RH_TET_BeastsFlockOfDoom_Master;
        public static MagicAbilityDef RH_TET_BeastsAmberSpear_Mage;
        public static MagicAbilityDef RH_TET_BeastsAmberSpear_Wizard;
        public static MagicAbilityDef RH_TET_BeastsAmberSpear_Warlock;
        public static MagicAbilityDef RH_TET_BeastsAmberSpear_Master;
        public static MagicAbilityDef RH_TET_BeastsAmberTrance_Mage;
        public static MagicAbilityDef RH_TET_BeastsAmberTrance_Wizard;
        public static MagicAbilityDef RH_TET_BeastsAmberTrance_Warlock;
        public static MagicAbilityDef RH_TET_BeastsAmberTrance_Master;
        public static MagicAbilityDef RH_TET_BeastsWyssonsWildform_Mage;
        public static MagicAbilityDef RH_TET_BeastsWyssonsWildform_Wizard;
        public static MagicAbilityDef RH_TET_BeastsWyssonsWildform_Warlock;
        public static MagicAbilityDef RH_TET_BeastsWyssonsWildform_Master;
        public static MagicAbilityDef RH_TET_BeastsBeastMaster_Mage;
        public static MagicAbilityDef RH_TET_BeastsBeastMaster_Wizard;
        public static MagicAbilityDef RH_TET_BeastsBeastMaster_Warlock;
        public static MagicAbilityDef RH_TET_BeastsBeastMaster_Master;
        ///  Shadow
        public static MagicAbilityDef RH_TET_ShadowBewilder_Mage;
        public static MagicAbilityDef RH_TET_ShadowBewilder_Wizard;
        public static MagicAbilityDef RH_TET_ShadowBewilder_Warlock;
        public static MagicAbilityDef RH_TET_ShadowBewilder_Master;
        public static MagicAbilityDef RH_TET_ShadowSteedOfShadows_Mage;
        public static MagicAbilityDef RH_TET_ShadowSteedOfShadows_Wizard;
        public static MagicAbilityDef RH_TET_ShadowSteedOfShadows_Warlock;
        public static MagicAbilityDef RH_TET_ShadowSteedOfShadows_Master; 
        public static MagicAbilityDef RH_TET_ShadowPenumbralPendulum_Mage;
        public static MagicAbilityDef RH_TET_ShadowPenumbralPendulum_Wizard;
        public static MagicAbilityDef RH_TET_ShadowPenumbralPendulum_Warlock;
        public static MagicAbilityDef RH_TET_ShadowPenumbralPendulum_Master;
        public static MagicAbilityDef RH_TET_ShadowMindSlip_Mage;
        public static MagicAbilityDef RH_TET_ShadowMindSlip_Wizard;
        public static MagicAbilityDef RH_TET_ShadowMindSlip_Warlock;
        public static MagicAbilityDef RH_TET_ShadowMindSlip_Master;
        public static MagicAbilityDef RH_TET_ShadowEnfeeblingFoe_Mage;
        public static MagicAbilityDef RH_TET_ShadowEnfeeblingFoe_Wizard;
        public static MagicAbilityDef RH_TET_ShadowEnfeeblingFoe_Warlock;
        public static MagicAbilityDef RH_TET_ShadowEnfeeblingFoe_Master;
        ///  Wild
        public static MagicAbilityDef RH_TET_WildBrayScream_Mage;
        public static MagicAbilityDef RH_TET_WildBrayScream_Wizard;
        public static MagicAbilityDef RH_TET_WildBrayScream_Warlock;
        public static MagicAbilityDef RH_TET_WildBrayScream_Master;
        public static MagicAbilityDef RH_TET_WildSavageDominion_Mage;
        public static MagicAbilityDef RH_TET_WildSavageDominion_Wizard;
        public static MagicAbilityDef RH_TET_WildSavageDominion_Warlock;
        public static MagicAbilityDef RH_TET_WildSavageDominion_Master;
        public static MagicAbilityDef RH_TET_WildMantleGhorok_Mage;
        public static MagicAbilityDef RH_TET_WildMantleGhorok_Wizard;
        public static MagicAbilityDef RH_TET_WildMantleGhorok_Warlock;
        public static MagicAbilityDef RH_TET_WildMantleGhorok_Master;
        public static MagicAbilityDef RH_TET_WildDevolve_Mage;
        public static MagicAbilityDef RH_TET_WildDevolve_Wizard;
        public static MagicAbilityDef RH_TET_WildDevolve_Warlock;
        public static MagicAbilityDef RH_TET_WildDevolve_Master;
        public static MagicAbilityDef RH_TET_WildBestialSurge_Mage;
        public static MagicAbilityDef RH_TET_WildBestialSurge_Wizard;
        public static MagicAbilityDef RH_TET_WildBestialSurge_Warlock;
        public static MagicAbilityDef RH_TET_WildBestialSurge_Master;
        ///  Death
        public static MagicAbilityDef RH_TET_DeathManacles_Mage;
        public static MagicAbilityDef RH_TET_DeathManacles_Wizard;
        public static MagicAbilityDef RH_TET_DeathManacles_Warlock;
        public static MagicAbilityDef RH_TET_DeathManacles_Master;
        public static MagicAbilityDef RH_TET_DeathPurpleSun_Mage;
        public static MagicAbilityDef RH_TET_DeathPurpleSun_Wizard;
        public static MagicAbilityDef RH_TET_DeathPurpleSun_Warlock;
        public static MagicAbilityDef RH_TET_DeathPurpleSun_Master;
        public static MagicAbilityDef RH_TET_DeathStealLife_Mage;
        public static MagicAbilityDef RH_TET_DeathStealLife_Wizard;
        public static MagicAbilityDef RH_TET_DeathStealLife_Warlock;
        public static MagicAbilityDef RH_TET_DeathStealLife_Master;
        public static MagicAbilityDef RH_TET_DeathWindOfDeath_Mage;
        public static MagicAbilityDef RH_TET_DeathWindOfDeath_Wizard;
        public static MagicAbilityDef RH_TET_DeathWindOfDeath_Warlock;
        public static MagicAbilityDef RH_TET_DeathWindOfDeath_Master;
        public static MagicAbilityDef RH_TET_DeathSoulBlight_Mage;
        public static MagicAbilityDef RH_TET_DeathSoulBlight_Wizard;
        public static MagicAbilityDef RH_TET_DeathSoulBlight_Warlock;
        public static MagicAbilityDef RH_TET_DeathSoulBlight_Master;
        // Fire
        public static MagicAbilityDef RH_TET_FireAegis_Mage;
        public static MagicAbilityDef RH_TET_FireAegis_Wizard;
        public static MagicAbilityDef RH_TET_FireAegis_Warlock;
        public static MagicAbilityDef RH_TET_FireAegis_Master;
        public static MagicAbilityDef RH_TET_FireBreatheFire_Mage;
        public static MagicAbilityDef RH_TET_FireBreatheFire_Wizard;
        public static MagicAbilityDef RH_TET_FireBreatheFire_Warlock;
        public static MagicAbilityDef RH_TET_FireBreatheFire_Master;
        public static MagicAbilityDef RH_TET_FireCascadingCloak_Mage;
        public static MagicAbilityDef RH_TET_FireCascadingCloak_Wizard;
        public static MagicAbilityDef RH_TET_FireCascadingCloak_Warlock;
        public static MagicAbilityDef RH_TET_FireCascadingCloak_Master;
        public static MagicAbilityDef RH_TET_FirePiercingBolts_Mage;
        public static MagicAbilityDef RH_TET_FirePiercingBolts_Wizard;
        public static MagicAbilityDef RH_TET_FirePiercingBolts_Warlock;
        public static MagicAbilityDef RH_TET_FirePiercingBolts_Master;
        public static MagicAbilityDef RH_TET_FireFlameStorm_Mage;
        public static MagicAbilityDef RH_TET_FireFlameStorm_Wizard;
        public static MagicAbilityDef RH_TET_FireFlameStorm_Warlock;
        public static MagicAbilityDef RH_TET_FireFlameStorm_Master;
        // Heavens
        public static MagicAbilityDef RH_TET_HeavensUrannonsThunderbolt_Mage;
        public static MagicAbilityDef RH_TET_HeavensUrannonsThunderbolt_Wizard;
        public static MagicAbilityDef RH_TET_HeavensUrannonsThunderbolt_Warlock;
        public static MagicAbilityDef RH_TET_HeavensUrannonsThunderbolt_Master;
        public static MagicAbilityDef RH_TET_HeavensCometOfCassandora_Mage;
        public static MagicAbilityDef RH_TET_HeavensCometOfCassandora_Wizard;
        public static MagicAbilityDef RH_TET_HeavensCometOfCassandora_Warlock;
        public static MagicAbilityDef RH_TET_HeavensCometOfCassandora_Master;
        public static MagicAbilityDef RH_TET_HeavensHarmonicConvergence_Mage;
        public static MagicAbilityDef RH_TET_HeavensHarmonicConvergence_Wizard;
        public static MagicAbilityDef RH_TET_HeavensHarmonicConvergence_Warlock;
        public static MagicAbilityDef RH_TET_HeavensHarmonicConvergence_Master;
        public static MagicAbilityDef RH_TET_HeavensClearSky_Mage;
        public static MagicAbilityDef RH_TET_HeavensClearSky_Wizard;
        public static MagicAbilityDef RH_TET_HeavensClearSky_Warlock;
        public static MagicAbilityDef RH_TET_HeavensClearSky_Master;
        public static MagicAbilityDef RH_TET_HeavensAzureBlades_Mage;
        public static MagicAbilityDef RH_TET_HeavensAzureBlades_Wizard;
        public static MagicAbilityDef RH_TET_HeavensAzureBlades_Warlock;
        public static MagicAbilityDef RH_TET_HeavensAzureBlades_Master;
        // Metal
        public static MagicAbilityDef RH_TET_MetalFinalTransmutation_Mage;
        public static MagicAbilityDef RH_TET_MetalFinalTransmutation_Wizard;
        public static MagicAbilityDef RH_TET_MetalFinalTransmutation_Warlock;
        public static MagicAbilityDef RH_TET_MetalFinalTransmutation_Master;
        public static MagicAbilityDef RH_TET_MetalForgeOfChamon_Mage;
        public static MagicAbilityDef RH_TET_MetalForgeOfChamon_Wizard;
        public static MagicAbilityDef RH_TET_MetalForgeOfChamon_Warlock;
        public static MagicAbilityDef RH_TET_MetalForgeOfChamon_Master;
        public static MagicAbilityDef RH_TET_MetalGleamingArrow_Mage;
        public static MagicAbilityDef RH_TET_MetalGleamingArrow_Wizard;
        public static MagicAbilityDef RH_TET_MetalGleamingArrow_Warlock;
        public static MagicAbilityDef RH_TET_MetalGleamingArrow_Master;
        public static MagicAbilityDef RH_TET_MetalMindTransmute_Mage;
        public static MagicAbilityDef RH_TET_MetalMindTransmute_Wizard;
        public static MagicAbilityDef RH_TET_MetalMindTransmute_Warlock;
        public static MagicAbilityDef RH_TET_MetalMindTransmute_Master;
        public static MagicAbilityDef RH_TET_MetalGlitteringRobe_Mage;
        public static MagicAbilityDef RH_TET_MetalGlitteringRobe_Wizard;
        public static MagicAbilityDef RH_TET_MetalGlitteringRobe_Warlock;
        public static MagicAbilityDef RH_TET_MetalGlitteringRobe_Master;
        // Life
        public static MagicAbilityDef RH_TET_LifeWinterFrost_Mage;
        public static MagicAbilityDef RH_TET_LifeWinterFrost_Wizard;
        public static MagicAbilityDef RH_TET_LifeWinterFrost_Warlock;
        public static MagicAbilityDef RH_TET_LifeWinterFrost_Master;
        public static MagicAbilityDef RH_TET_LifeDrainLife_Mage;
        public static MagicAbilityDef RH_TET_LifeDrainLife_Wizard;
        public static MagicAbilityDef RH_TET_LifeDrainLife_Warlock;
        public static MagicAbilityDef RH_TET_LifeDrainLife_Master;
        public static MagicAbilityDef RH_TET_LifeRegenerate_Mage;
        public static MagicAbilityDef RH_TET_LifeRegenerate_Wizard;
        public static MagicAbilityDef RH_TET_LifeRegenerate_Warlock;
        public static MagicAbilityDef RH_TET_LifeRegenerate_Master;
        public static MagicAbilityDef RH_TET_LifeCureBlight_Mage;
        public static MagicAbilityDef RH_TET_LifeCureBlight_Wizard;
        public static MagicAbilityDef RH_TET_LifeCureBlight_Warlock;
        public static MagicAbilityDef RH_TET_LifeCureBlight_Master;
        public static MagicAbilityDef RH_TET_LifeLifebloom_Mage;
        public static MagicAbilityDef RH_TET_LifeLifebloom_Wizard;
        public static MagicAbilityDef RH_TET_LifeLifebloom_Warlock;
        public static MagicAbilityDef RH_TET_LifeLifebloom_Master;
        // Light
        public static MagicAbilityDef RH_TET_LightAbullasSnare_Mage;
        public static MagicAbilityDef RH_TET_LightAbullasSnare_Wizard;
        public static MagicAbilityDef RH_TET_LightAbullasSnare_Warlock;
        public static MagicAbilityDef RH_TET_LightAbullasSnare_Master;
        public static MagicAbilityDef RH_TET_LightClawOfApek_Mage;
        public static MagicAbilityDef RH_TET_LightClawOfApek_Wizard;
        public static MagicAbilityDef RH_TET_LightClawOfApek_Warlock;
        public static MagicAbilityDef RH_TET_LightClawOfApek_Master;
        public static MagicAbilityDef RH_TET_LightHealingLight_Mage;
        public static MagicAbilityDef RH_TET_LightHealingLight_Wizard;
        public static MagicAbilityDef RH_TET_LightHealingLight_Warlock;
        public static MagicAbilityDef RH_TET_LightHealingLight_Master;
        public static MagicAbilityDef RH_TET_LightBurningGaze_Mage;
        public static MagicAbilityDef RH_TET_LightBurningGaze_Wizard;
        public static MagicAbilityDef RH_TET_LightBurningGaze_Warlock;
        public static MagicAbilityDef RH_TET_LightBurningGaze_Master;
        public static MagicAbilityDef RH_TET_LightIlluminateTheEdifice_Mage;
        public static MagicAbilityDef RH_TET_LightIlluminateTheEdifice_Wizard;
        public static MagicAbilityDef RH_TET_LightIlluminateTheEdifice_Warlock;
        public static MagicAbilityDef RH_TET_LightIlluminateTheEdifice_Master;
        // Nurgle
        public static MagicAbilityDef RH_TET_NurgleAfflictions_Mage;
        public static MagicAbilityDef RH_TET_NurgleAfflictions_Wizard;
        public static MagicAbilityDef RH_TET_NurgleAfflictions_Warlock;
        public static MagicAbilityDef RH_TET_NurgleAfflictions_Master;
        public static MagicAbilityDef RH_TET_NurgleBoon_Mage;
        public static MagicAbilityDef RH_TET_NurgleBoon_Wizard;
        public static MagicAbilityDef RH_TET_NurgleBoon_Warlock;
        public static MagicAbilityDef RH_TET_NurgleBoon_Master;
        public static MagicAbilityDef RH_TET_NurgleStream_Mage;
        public static MagicAbilityDef RH_TET_NurgleStream_Wizard;
        public static MagicAbilityDef RH_TET_NurgleStream_Warlock;
        public static MagicAbilityDef RH_TET_NurgleStream_Master;
        public static MagicAbilityDef RH_TET_NurglePestilence_Mage;
        public static MagicAbilityDef RH_TET_NurglePestilence_Wizard;
        public static MagicAbilityDef RH_TET_NurglePestilence_Warlock;
        public static MagicAbilityDef RH_TET_NurglePestilence_Master;
        public static MagicAbilityDef RH_TET_NurgleRotbomb_Mage;
        public static MagicAbilityDef RH_TET_NurgleRotbomb_Wizard;
        public static MagicAbilityDef RH_TET_NurgleRotbomb_Warlock;
        public static MagicAbilityDef RH_TET_NurgleRotbomb_Master;
        // Chaos Undivided
        public static MagicAbilityDef RH_TET_ChaosVisionOfTorment_Mage;
        public static MagicAbilityDef RH_TET_ChaosVisionOfTorment_Wizard;
        public static MagicAbilityDef RH_TET_ChaosVisionOfTorment_Warlock;
        public static MagicAbilityDef RH_TET_ChaosVisionOfTorment_Master;
        public static MagicAbilityDef RH_TET_ChaosBoon_Mage;
        public static MagicAbilityDef RH_TET_ChaosBoon_Wizard;
        public static MagicAbilityDef RH_TET_ChaosBoon_Warlock;
        public static MagicAbilityDef RH_TET_ChaosBoon_Master;
        public static MagicAbilityDef RH_TET_ChaosWordOfPain_Mage;
        public static MagicAbilityDef RH_TET_ChaosWordOfPain_Wizard;
        public static MagicAbilityDef RH_TET_ChaosWordOfPain_Warlock;
        public static MagicAbilityDef RH_TET_ChaosWordOfPain_Master;
        public static MagicAbilityDef RH_TET_ChaosBurningBlood_Mage;
        public static MagicAbilityDef RH_TET_ChaosBurningBlood_Wizard;
        public static MagicAbilityDef RH_TET_ChaosBurningBlood_Warlock;
        public static MagicAbilityDef RH_TET_ChaosBurningBlood_Master;
        public static MagicAbilityDef RH_TET_ChaosVeil_Mage;
        public static MagicAbilityDef RH_TET_ChaosVeil_Wizard;
        public static MagicAbilityDef RH_TET_ChaosVeil_Warlock;
        public static MagicAbilityDef RH_TET_ChaosVeil_Master;
        // Slaanesh
        public static MagicAbilityDef RH_TET_SlaaneshGoldenTorrent_Mage;
        public static MagicAbilityDef RH_TET_SlaaneshGoldenTorrent_Wizard;
        public static MagicAbilityDef RH_TET_SlaaneshGoldenTorrent_Warlock;
        public static MagicAbilityDef RH_TET_SlaaneshGoldenTorrent_Master;
        public static MagicAbilityDef RH_TET_SlaaneshBlissful_Mage;
        public static MagicAbilityDef RH_TET_SlaaneshBlissful_Wizard;
        public static MagicAbilityDef RH_TET_SlaaneshBlissful_Warlock;
        public static MagicAbilityDef RH_TET_SlaaneshBlissful_Master;
        public static MagicAbilityDef RH_TET_SlaaneshDelectableTorture_Mage;
        public static MagicAbilityDef RH_TET_SlaaneshDelectableTorture_Wizard;
        public static MagicAbilityDef RH_TET_SlaaneshDelectableTorture_Warlock;
        public static MagicAbilityDef RH_TET_SlaaneshDelectableTorture_Master;
        public static MagicAbilityDef RH_TET_SlaaneshSlothfulStupor_Mage;
        public static MagicAbilityDef RH_TET_SlaaneshSlothfulStupor_Wizard;
        public static MagicAbilityDef RH_TET_SlaaneshSlothfulStupor_Warlock;
        public static MagicAbilityDef RH_TET_SlaaneshSlothfulStupor_Master;
        public static MagicAbilityDef RH_TET_SlaaneshPainPleasure_Mage;
        public static MagicAbilityDef RH_TET_SlaaneshPainPleasure_Wizard;
        public static MagicAbilityDef RH_TET_SlaaneshPainPleasure_Warlock;
        public static MagicAbilityDef RH_TET_SlaaneshPainPleasure_Master;
        // High
        public static MagicAbilityDef RH_TET_HighPhoenixFlames_Mage;
        public static MagicAbilityDef RH_TET_HighPhoenixFlames_Wizard;
        public static MagicAbilityDef RH_TET_HighPhoenixFlames_Warlock;
        public static MagicAbilityDef RH_TET_HighPhoenixFlames_Master;
        public static MagicAbilityDef RH_TET_HighShieldSaphery_Mage;
        public static MagicAbilityDef RH_TET_HighShieldSaphery_Wizard;
        public static MagicAbilityDef RH_TET_HighShieldSaphery_Warlock;
        public static MagicAbilityDef RH_TET_HighShieldSaphery_Master;
        public static MagicAbilityDef RH_TET_HighApotheosis_Mage;
        public static MagicAbilityDef RH_TET_HighApotheosis_Wizard;
        public static MagicAbilityDef RH_TET_HighApotheosis_Warlock;
        public static MagicAbilityDef RH_TET_HighApotheosis_Master;
        public static MagicAbilityDef RH_TET_HighFuryOfKhaine_Mage;
        public static MagicAbilityDef RH_TET_HighFuryOfKhaine_Wizard;
        public static MagicAbilityDef RH_TET_HighFuryOfKhaine_Warlock;
        public static MagicAbilityDef RH_TET_HighFuryOfKhaine_Master;
        public static MagicAbilityDef RH_TET_HighGlamourOfTeclis_Mage;
        public static MagicAbilityDef RH_TET_HighGlamourOfTeclis_Wizard;
        public static MagicAbilityDef RH_TET_HighGlamourOfTeclis_Warlock;
        public static MagicAbilityDef RH_TET_HighGlamourOfTeclis_Master;
        // Plague
        public static MagicAbilityDef RH_TET_PlagueRedPox_Mage;
        public static MagicAbilityDef RH_TET_PlagueRedPox_Wizard;
        public static MagicAbilityDef RH_TET_PlagueRedPox_Warlock;
        public static MagicAbilityDef RH_TET_PlagueRedPox_Master;
        public static MagicAbilityDef RH_TET_PlagueInfectingGaze_Mage;
        public static MagicAbilityDef RH_TET_PlagueInfectingGaze_Wizard;
        public static MagicAbilityDef RH_TET_PlagueInfectingGaze_Warlock;
        public static MagicAbilityDef RH_TET_PlagueInfectingGaze_Master;
        public static MagicAbilityDef RH_TET_PlaguePestilentBreath_Mage;
        public static MagicAbilityDef RH_TET_PlaguePestilentBreath_Wizard;
        public static MagicAbilityDef RH_TET_PlaguePestilentBreath_Warlock;
        public static MagicAbilityDef RH_TET_PlaguePestilentBreath_Master;
        public static MagicAbilityDef RH_TET_PlagueAirOfPestilence_Mage;
        public static MagicAbilityDef RH_TET_PlagueAirOfPestilence_Wizard;
        public static MagicAbilityDef RH_TET_PlagueAirOfPestilence_Warlock;
        public static MagicAbilityDef RH_TET_PlagueAirOfPestilence_Master;
        public static MagicAbilityDef RH_TET_PlagueCloudOfFlies_Mage;
        public static MagicAbilityDef RH_TET_PlagueCloudOfFlies_Wizard;
        public static MagicAbilityDef RH_TET_PlagueCloudOfFlies_Warlock;
        public static MagicAbilityDef RH_TET_PlagueCloudOfFlies_Master;
        // Warp
        public static MagicAbilityDef RH_TET_WarpLightning_Mage;
        public static MagicAbilityDef RH_TET_WarpLightning_Wizard;
        public static MagicAbilityDef RH_TET_WarpLightning_Warlock;
        public static MagicAbilityDef RH_TET_WarpLightning_Master;
        public static MagicAbilityDef RH_TET_WarpGhostlyFlame_Mage;
        public static MagicAbilityDef RH_TET_WarpGhostlyFlame_Wizard;
        public static MagicAbilityDef RH_TET_WarpGhostlyFlame_Warlock;
        public static MagicAbilityDef RH_TET_WarpGhostlyFlame_Master;
        public static MagicAbilityDef RH_TET_WarpRatThrall_Mage;
        public static MagicAbilityDef RH_TET_WarpRatThrall_Wizard;
        public static MagicAbilityDef RH_TET_WarpRatThrall_Warlock;
        public static MagicAbilityDef RH_TET_WarpRatThrall_Master;
        public static MagicAbilityDef RH_TET_WarpCurseHornedRat_Mage;
        public static MagicAbilityDef RH_TET_WarpCurseHornedRat_Wizard;
        public static MagicAbilityDef RH_TET_WarpCurseHornedRat_Warlock;
        public static MagicAbilityDef RH_TET_WarpCurseHornedRat_Master;
        public static MagicAbilityDef RH_TET_WarpVerminousRuin_Mage;
        public static MagicAbilityDef RH_TET_WarpVerminousRuin_Wizard;
        public static MagicAbilityDef RH_TET_WarpVerminousRuin_Warlock;
        public static MagicAbilityDef RH_TET_WarpVerminousRuin_Master;
        // Maw
        public static MagicAbilityDef RH_TET_MawBonecrusher_Mage;
        public static MagicAbilityDef RH_TET_MawBonecrusher_Wizard;
        public static MagicAbilityDef RH_TET_MawBonecrusher_Warlock;
        public static MagicAbilityDef RH_TET_MawBonecrusher_Master;
        public static MagicAbilityDef RH_TET_MawBraingobbler_Mage;
        public static MagicAbilityDef RH_TET_MawBraingobbler_Wizard;
        public static MagicAbilityDef RH_TET_MawBraingobbler_Warlock;
        public static MagicAbilityDef RH_TET_MawBraingobbler_Master;
        public static MagicAbilityDef RH_TET_MawBullgorger_Mage;
        public static MagicAbilityDef RH_TET_MawBullgorger_Wizard;
        public static MagicAbilityDef RH_TET_MawBullgorger_Warlock;
        public static MagicAbilityDef RH_TET_MawBullgorger_Master;
        public static MagicAbilityDef RH_TET_MawTrollguts_Mage;
        public static MagicAbilityDef RH_TET_MawTrollguts_Wizard;
        public static MagicAbilityDef RH_TET_MawTrollguts_Warlock;
        public static MagicAbilityDef RH_TET_MawTrollguts_Master;
        public static MagicAbilityDef RH_TET_MawToothcracker_Mage;
        public static MagicAbilityDef RH_TET_MawToothcracker_Wizard;
        public static MagicAbilityDef RH_TET_MawToothcracker_Warlock;
        public static MagicAbilityDef RH_TET_MawToothcracker_Master;
        // Stealth
        public static MagicAbilityDef RH_TET_StealthSkitterleap_Mage;
        public static MagicAbilityDef RH_TET_StealthSkitterleap_Wizard;
        public static MagicAbilityDef RH_TET_StealthSkitterleap_Warlock;
        public static MagicAbilityDef RH_TET_StealthSkitterleap_Master;
        public static MagicAbilityDef RH_TET_StealthSwiftscamper_Mage;
        public static MagicAbilityDef RH_TET_StealthSwiftscamper_Wizard;
        public static MagicAbilityDef RH_TET_StealthSwiftscamper_Warlock;
        public static MagicAbilityDef RH_TET_StealthSwiftscamper_Master;
        public static MagicAbilityDef RH_TET_Stealth_ShadowsCompanion_Mage;
        public static MagicAbilityDef RH_TET_Stealth_ShadowsCompanion_Wizard;
        public static MagicAbilityDef RH_TET_Stealth_ShadowsCompanion_Warlock;
        public static MagicAbilityDef RH_TET_Stealth_ShadowsCompanion_Master;
        public static MagicAbilityDef RH_TET_StealthArmorOfDarkness_Mage;
        public static MagicAbilityDef RH_TET_StealthArmorOfDarkness_Wizard;
        public static MagicAbilityDef RH_TET_StealthArmorOfDarkness_Warlock;
        public static MagicAbilityDef RH_TET_StealthArmorOfDarkness_Master;
        public static MagicAbilityDef RH_TET_StealthTracelessDemise_Mage;
        public static MagicAbilityDef RH_TET_StealthTracelessDemise_Wizard;
        public static MagicAbilityDef RH_TET_StealthTracelessDemise_Warlock;
        public static MagicAbilityDef RH_TET_StealthTracelessDemise_Master;

        // Add On Magic Abilities
        public static MagicAbilityDef RH_TET_AddOn_WeakenPoison_Mage;
        public static MagicAbilityDef RH_TET_AddOn_WeakenPoison_Wizard;
        public static MagicAbilityDef RH_TET_AddOn_WeakenPoison_Warlock;
        public static MagicAbilityDef RH_TET_AddOn_WeakenPoison_Master;
        public static MagicAbilityDef RH_TET_AddOn_RaiseGround_Mage;
        public static MagicAbilityDef RH_TET_AddOn_RaiseGround_Wizard;
        public static MagicAbilityDef RH_TET_AddOn_RaiseGround_Warlock;
        public static MagicAbilityDef RH_TET_AddOn_RaiseGround_Master;
        public static MagicAbilityDef RH_TET_AddOn_Transport_Mage;
        public static MagicAbilityDef RH_TET_AddOn_Transport_Wizard;
        public static MagicAbilityDef RH_TET_AddOn_Transport_Warlock;
        public static MagicAbilityDef RH_TET_AddOn_Transport_Master;
        public static MagicAbilityDef RH_TET_AddOn_LowerGround_Mage;
        public static MagicAbilityDef RH_TET_AddOn_LowerGround_Wizard;
        public static MagicAbilityDef RH_TET_AddOn_LowerGround_Warlock;
        public static MagicAbilityDef RH_TET_AddOn_LowerGround_Master;
        public static MagicAbilityDef RH_TET_AddOn_ZoneOfFertility_Mage;
        public static MagicAbilityDef RH_TET_AddOn_ZoneOfFertility_Wizard;
        public static MagicAbilityDef RH_TET_AddOn_ZoneOfFertility_Warlock;
        public static MagicAbilityDef RH_TET_AddOn_ZoneOfFertility_Master;
        public static MagicAbilityDef RH_TET_AddOn_LightCantrip_Mage;
        public static MagicAbilityDef RH_TET_AddOn_LightCantrip_Wizard;
        public static MagicAbilityDef RH_TET_AddOn_LightCantrip_Warlock;
        public static MagicAbilityDef RH_TET_AddOn_LightCantrip_Master;

        // Artifacts
        public static ThingDef RH_TET_KhorneHorn;
        public static ThingDef RH_TET_NurgleCharm;
        public static ThingDef RH_TET_TzeentchCoin;
        public static ThingDef RH_TET_SlaaneshChalice;
        public static ThingDef RH_TET_StoneOfWisdomChaosUndiv;

        // Apparel
        public static ThingDef RH_TET_Magic_Apparel_TzeentchRibbon;
        public static ThingDef RH_TET_Magic_Apparel_KhorneBelt;
        public static ThingDef RH_TET_Magic_Apparel_SlaaneshSash;
        public static ThingDef RH_TET_Magic_Apparel_NurgleBand;

        // Pawns
        public static PawnKindDef RH_TET_JabberslytheSummoned;
        public static PawnKindDef RH_TET_Magic_WolfUlric_Summonable;
        public static PawnKindDef RH_TET_JabberslytheAnimal;
        public static PawnKindDef RH_TET_Beastmen_Raven;
        public static PawnKindDef RH_TET_Rat_Medium;
        public static PawnKindDef RH_TET_Rat;

        // Races
        public static ThingDef RH_TET_JabberslytheRaceAnimal;

        // Dark Gods 
        public static DarkGodDef RH_TET_Khorne;
        public static DarkGodDef RH_TET_Nurgle;
        public static DarkGodDef RH_TET_Slaanesh;
        public static DarkGodDef RH_TET_Tzeentch;

        // Thoughts
        public static ThoughtDef RH_TET_SlaaneshPleasure;
        public static ThoughtDef RH_TET_Plus15For15Days;
        public static ThoughtDef RH_TET_ShallyaFortuneThought;
        public static ThoughtDef RH_TET_GrimResolve_Mood;

        // Things 
        public static ThingDef RH_TET_FlameTornado;
        public static ThingDef RH_TET_Magic_FinalTransmutationSculpture;
        public static ThingDef RH_TET_IlluminateLight;
        public static ThingDef RH_TET_LightCantrip;
        public static ThingDef RH_TET_UlricVortex;
        public static ThingDef RH_TET_Magic_PawnLeaper;
        public static ThingDef RH_TET_PoisonGas;
        public static ThingDef RH_TET_InsectCloud;
        public static ThingDef RH_TET_GhostlyLight;
        public static ThingDef RH_TET_CombatGas;

        // Thing Categories
        public static ThingCategoryDef RH_TET_MagicItems;
        public static ThingCategoryDef RH_TET_MagicItems_Trainers;

        // Weapons 
        public static ThingDef RH_TET_MeleeWeapon_KhorneAxe;
        public static ThingDef RH_TET_KhorneLight;
        public static ThingDef RH_TET_MeleeWeapon_NurgleStar;
        public static ThingDef RH_TET_NurgleLight;
        public static ThingDef RH_TET_MeleeWeapon_SlaaneshWhip;
        public static ThingDef RH_TET_SlaaneshLight;
        public static ThingDef RH_TET_MeleeWeapon_TzeentchStaff;
        public static ThingDef RH_TET_TzeentchLight;
        public static ThingDef RH_TET_MeleeWeapon_SigmarHammer;
        public static ThingDef RH_TET_SigmarLight;
        public static ThingDef RH_TET_MeleeWeapon_UlricHammer;
        public static ThingDef RH_TET_UlricLight;
        //public static ThingDef MineInSpawner;
        
        //// =============== FAITH Traits ===============
        public static TraitDef RH_TET_SigmarTrait;
        public static TraitDef RH_TET_ShallyaTrait;
        public static TraitDef RH_TET_UlricTrait;

        //// =============== MAGIC Traits ===============
        public static TraitDef RH_TET_LoreOfBeastsTrait;
        public static TraitDef RH_TET_LoreOfWildTrait;
        public static TraitDef RH_TET_LoreOfTzeentchTrait;
        public static TraitDef RH_TET_LoreOfShadowTrait;
        public static TraitDef RH_TET_LoreOfDeathTrait;
        public static TraitDef RH_TET_LoreOfFireTrait;
        public static TraitDef RH_TET_LoreOfHeavensTrait;
        public static TraitDef RH_TET_LoreOfMetalTrait;
        public static TraitDef RH_TET_LoreOfLightTrait;
        public static TraitDef RH_TET_LoreOfLifeTrait;
        public static TraitDef RH_TET_LoreOfNurgleTrait;
        public static TraitDef RH_TET_LoreOfChaosUndividedTrait;
        public static TraitDef RH_TET_LoreOfSlaaneshTrait;
        public static TraitDef RH_TET_LoreOfHighTrait;
        public static TraitDef RH_TET_LoreOfPlagueTrait;
        public static TraitDef RH_TET_LoreOfGreatMawTrait;
        public static TraitDef RH_TET_LoreOfWarpTrait;
        public static TraitDef RH_TET_LoreOfStealthTrait;

        //// =============== ABILITY ACTION Traits ===============
        public static TraitDef RH_TET_WitchHunterTrait;
        
        //// =============== DAMAGE ===============
        public static DamageDef RH_TET_Lightning;
        public static DamageDef RH_TET_MagicFire;
        public static DamageDef RH_TET_MagicalInjury;
        public static DamageDef RH_TET_MagicHealDamage;
        public static DamageDef RH_TET_MagicalBlunt;

        //// =============== Jobs ===============
        public static JobDef RH_TET_MagicMeditationJob;
        public static JobDef RH_TET_Magic_DoBillIdol;
        public static JobDef RH_TET_Magic_DoBillMagicBench;
        public static JobDef RH_TET_Magic_RepairStuff;

        //// =============== HEDIFFS ===============
        public static HediffDef RH_TET_MagicWielder;
        public static HediffDef RH_TET_FaithWielder;
        public static HediffDef RH_TET_AbilityActionWielder;
        public static HediffDef RH_TET_FireCloak;
        public static HediffDef RH_TET_MagicShield; 
        public static HediffDef RH_TET_GlitteringRobe;
        public static HediffDef RH_TET_ArmorOfDarkness;
        public static HediffDef RH_TET_WildBestialSurge;
        public static HediffDef RH_TET_AmberTrance;
        public static HediffDef RH_TET_BeastsWyssonsWildformI;
        public static HediffDef RH_TET_BeastsWyssonsWildformII;
        public static HediffDef RH_TET_BeastsWyssonsWildformIII;
        public static HediffDef RH_TET_BeastsWyssonsWildformIV;
        public static HediffDef RH_TET_DeathManacles;
        public static HediffDef RH_TET_LightSnare; 
        public static HediffDef RH_TET_SigmarShield;
        public static HediffDef RH_TET_ShallyaFortune;
        public static HediffDef RH_TET_NurgleAfflictionsI;
        public static HediffDef RH_TET_NurgleAfflictionsII;
        public static HediffDef RH_TET_NurgleAfflictionsIII;
        public static HediffDef RH_TET_NurgleAfflictionsIV;
        public static HediffDef RH_TET_NurgleStreamOfCorruptionI;
        public static HediffDef RH_TET_Magic_Knockdown;
        public static HediffDef RH_TET_ChaosBoonI;
        public static HediffDef RH_TET_ChaosBoonII;
        public static HediffDef RH_TET_ChaosBoonIII;
        public static HediffDef RH_TET_ChaosBoonIV;
        public static HediffDef RH_TET_SlaaneshLethargy;
        public static HediffDef RH_TET_ShieldSaphery;
        public static HediffDef RH_TET_Magic_Toothcracker;
        public static HediffDef RH_TET_StealthSwiftscamper;
        public static HediffDef RH_TET_ShadowsCompanion_HE;
        public static HediffDef RH_TET_TrollHeartEffect;
        public static HediffDef RH_TET_Lethargic;
        
        //// =============== SOUNDS =============== 
        public static SoundDef RH_TET_Magic_Miscast;
        public static SoundDef RH_TET_Magic_SoundWhoosh;
        public static SoundDef RH_TET_Magic_SoundBrayScream;

        // Flecks
        public static FleckDef RH_TET_FleckPurpleEffect;
        public static FleckDef RH_TET_FleckRedEffect;
        public static FleckDef RH_TET_FleckBlueEffect;
        public static FleckDef RH_TET_FleckYellowEffect;
        public static FleckDef RH_TET_FleckGrayEffect;
        public static FleckDef RH_TET_FleckBrownEffect;
        public static FleckDef RH_TET_FleckGoldEffect;
        public static FleckDef RH_TET_FleckGreenEffect;
        public static FleckDef RH_TET_FleckGreenEffectFancy;
        public static FleckDef RH_TET_FleckWhiteEffect;
        public static FleckDef RH_TET_FleckWhiteLightEffect;
        public static FleckDef RH_TET_FleckPinkEffect;
        public static FleckDef RH_TET_Fleck_Afflictions;
        public static FleckDef RH_TET_Fleck_ChaosStar;
        public static FleckDef RH_TET_Fleck_Amber;
        public static FleckDef RH_TET_Fleck_Wildform;
        public static FleckDef RH_TET_Fleck_Enfeebled;
        public static FleckDef RH_TET_Fleck_Surge;
        public static FleckDef RH_TET_Fleck_Mantle;
        public static FleckDef RH_TET_Fleck_Manacles;
        public static FleckDef RH_TET_Fleck_Snare;
        public static FleckDef RH_TET_Fleck_Convergence;
        public static FleckDef RH_TET_Fleck_Apotheosis;
        public static FleckDef RH_TET_Fleck_RedPox;

        // Motes
        public static ThingDef RH_TET_Mote_Afflictions;
        public static ThingDef RH_TET_Mote_Amber;
        public static ThingDef RH_TET_Mote_Wildform;
        public static ThingDef RH_TET_Mote_Enfeebled;
        public static ThingDef RH_TET_Mote_Surge;
        public static ThingDef RH_TET_Mote_Mantle;
        public static ThingDef RH_TET_Mote_Manacles;
        public static ThingDef RH_TET_Mote_Snare;
        public static ThingDef RH_TET_Mote_Convergence;
        public static ThingDef RH_TET_Mote_Apotheosis;
        public static ThingDef RH_TET_Mote_RedPox;
        public static ThingDef RH_TET_Mote_ChaosStar;

        // Idols
        public static ThingDef RH_TET_Magic_FaithIdol_Sigmar;
        public static ThingDef RH_TET_Magic_FaithIdol_Shallya;
        public static ThingDef RH_TET_Magic_FaithIdol_Ulric;

        // Transport Pod Related
        public static ThingDef RH_TET_ActiveMagicPod;
        public static ThingDef RH_TET_MagicPodIncoming;
        public static ThingDef RH_TET_MagicPodLeaving;
        public static WorldObjectDef RH_TET_Magic_TravelingTransportPod;

        // Effecter
        public static EffecterDef RH_TET_ShadowsCompanion_Effecter;
        public static EffecterDef RH_TET_Magic_FlightEffect;

        // Magic Items
        public static ThingDef RH_TET_Potion_Healing;
        public static ThingDef RH_TET_Wand_Resurrection;
        public static ThingDef RH_TET_Magic_MagicEye;
        public static ThingDef RH_TET_Magic_MagicArm;
        public static ThingDef RH_TET_Magic_MagicLeg;
        public static ThingDef RH_TET_Magic_Jewelry_RubyRingOfRuin;
        public static ThingDef RH_TET_BootsOfSpeed;
        public static ThingDef RH_TET_Magic_BeltArdor;
        public static ThingDef RH_TET_Rod_FlamingDeath;
        public static ThingDef RH_TET_BootsWinged;
        public static ThingDef RH_TET_Magic_HelmetRegeneration;
        public static ThingDef RH_TET_Magic_ArmorVitality;
        public static ThingDef RH_TET_MeleeWeapon_ObsidianBlade;
        public static ThingDef RH_TET_MeleeWeapon_GoldSigilSword;
    }
}