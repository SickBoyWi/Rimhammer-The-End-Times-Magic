using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class Pawn_WolfUlric : Pawn_SummonedExpires
    {
        public static readonly Texture2D Unsummon = ContentFinder<Texture2D>.Get("UI/Commands/RH_TET_Magic_Unsummon", true);

        public override void PostSummonSetup()
        {
            base.PostSummonSetup();
            if (this.Faction != null)
                this.SetFaction(null);
            if (this.def.defName.Contains("Summonable") || this.def.defName.Contains("Maddened"))
                this.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, (string)null, false, false, (Pawn)null, false);
        }

        private bool RetFalse()
        {
            return false;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
                yield return g;

            Pawn pawn = this;

            Command_Toggle commandToggle1 = new Command_Toggle();
            commandToggle1.hotKey = KeyBindingDefOf.Command_ColonistDraft;
            commandToggle1.isActive = RetFalse;
            commandToggle1.toggleAction = new Action((() =>
            {
                FleckMaker.ThrowLightningGlow(this.Position.ToVector3(), this.Map, 3.0f);
                FleckMaker.Static(this.Position, this.Map, FleckDefOf.PsycastAreaEffect, 1f);
                this.Destroy(DestroyMode.Vanish);
            }));
            commandToggle1.defaultDesc = (string)"RH_TET_Magic_Unsummon".Translate();
            commandToggle1.icon = Pawn_WolfUlric.Unsummon;
            commandToggle1.turnOnSound = SoundDefOf.DraftOn;
            commandToggle1.turnOffSound = SoundDefOf.DraftOff;
            commandToggle1.defaultLabel = (string)"RH_TET_Magic_Unsummon".Translate();
            commandToggle1.tutorTag = "RH_TET_Magic_Unsummon".Translate(); 
            if (!Faction.OfPlayer.def.defName.Equals("RH_TET_Empire_PlayerWizard") && !Faction.OfPlayer.def.defName.Equals("RH_TET_Empire_PlayerColony"))
                commandToggle1.Disable((string)"RH_TET_Magic_NotEmpirePlayerUnsummon".Translate());
            yield return (Gizmo)commandToggle1;
        }

        public Pawn_WolfUlric()
            : base()
        {
        }
    }
}
