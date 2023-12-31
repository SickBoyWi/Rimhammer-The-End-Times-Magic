using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    public class CompRechargeable : ThingComp, IVerbOwner
    {
        private VerbTracker verbTracker;
        private int ticksUntilNextUse;

        public CompProperties_Rechargeable Props
        {
            get
            {
                return this.props as CompProperties_Rechargeable;
            }
        }

        public int TicksUntilNextUse
        {
            get
            {
                return this.ticksUntilNextUse;
            }
        }

        public Pawn Wearer
        {
            get
            {
                return CompRechargeable.WearerOf(this);
            }
        }

        public List<Verse.VerbProperties> VerbProperties
        {
            get
            {
                return this.parent.def.Verbs;
            }
        }

        public List<Tool> Tools
        {
            get
            {
                return this.parent.def.tools;
            }
        }

        public ImplementOwnerTypeDef ImplementOwnerTypeDef
        {
            get
            {
                return ImplementOwnerTypeDefOf.NativeVerb;
            }
        }

        public Thing ConstantCaster
        {
            get
            {
                return (Thing)this.Wearer;
            }
        }

        public string UniqueVerbOwnerID()
        {
            return "RH_TET_Rechargeable_" + this.parent.ThingID;
        }

        public bool VerbsStillUsableBy(Pawn p)
        {
            return this.Wearer == p;
        }

        public VerbTracker VerbTracker
        {
            get
            {
                if (this.verbTracker == null)
                    this.verbTracker = new VerbTracker((IVerbOwner)this);
                return this.verbTracker;
            }
        }

        public List<Verb> AllVerbs
        {
            get
            {
                return this.VerbTracker.AllVerbs;
            }
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            this.ticksUntilNextUse = 0;
        }

        public override string CompInspectStringExtra()
        {
            if (this.ticksUntilNextUse == 0)
                return "RH_TET_Magic_Charged".Translate();
            return "RH_TET_Magic_TimeUntilCharged".Translate(this.ticksUntilNextUse.ToStringTicksToPeriod(true, true, true, false));
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.ticksUntilNextUse, "ticksUntilNextUse", 0, false);
            Scribe_Deep.Look<VerbTracker>(ref this.verbTracker, "verbTracker", (object)this);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (this.ticksUntilNextUse > 0)
                this.ticksUntilNextUse--;
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            CompRechargeable compRecharge = this;
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;
            bool drafted = compRecharge.Wearer.Drafted;
            if ((!drafted || compRecharge.Props.displayGizmoWhileDrafted) && (drafted || compRecharge.Props.displayGizmoWhileUndrafted))
            {
                ThingWithComps gear = compRecharge.parent;
                foreach (Verb allVerb in compRecharge.VerbTracker.AllVerbs)
                {
                    if (allVerb.verbProps.hasStandardCommand)
                        yield return (Gizmo)compRecharge.CreateVerbTargetCommand((Thing)gear, allVerb);
                }
                if (Prefs.DevMode)
                {
                    Command_Action commandAction = new Command_Action();
                    commandAction.defaultLabel = "Debug: Recharge";
                    commandAction.action = new Action((() =>
                    {
                        this.ticksUntilNextUse = 0;
                    }));
                    yield return (Gizmo)commandAction;
                }
            }
        }

        private Command_Rechargeable CreateVerbTargetCommand(Thing gear, Verb verb)
        {
            Command_Rechargeable commandReloadable = new Command_Rechargeable(this);
            commandReloadable.defaultDesc = gear.def.description;
            commandReloadable.hotKey = this.Props.hotKey;
            commandReloadable.defaultLabel = verb.verbProps.label;
            commandReloadable.verb = verb;
            if (verb.verbProps.defaultProjectile != null && verb.verbProps.commandIcon == null)
            {
                commandReloadable.icon = verb.verbProps.defaultProjectile.uiIcon;
                commandReloadable.iconAngle = verb.verbProps.defaultProjectile.uiIconAngle;
                commandReloadable.iconOffset = verb.verbProps.defaultProjectile.uiIconOffset;
                commandReloadable.overrideColor = new Color?(verb.verbProps.defaultProjectile.graphicData.color);
            }
            else
            {
                commandReloadable.icon = (UnityEngine.Object)verb.UIIcon != (UnityEngine.Object)BaseContent.BadTex ? verb.UIIcon : gear.def.uiIcon;
                commandReloadable.iconAngle = gear.def.uiIconAngle;
                commandReloadable.iconOffset = gear.def.uiIconOffset;
                commandReloadable.defaultIconColor = gear.DrawColor;
            }
            if (!this.Wearer.IsColonistPlayerControlled)
                commandReloadable.Disable((string)null);
            else if (verb.verbProps.violent && this.Wearer.WorkTagIsDisabled(WorkTags.Violent))
                commandReloadable.Disable((string)("IsIncapableOfViolenceLower".Translate((NamedArgument)this.Wearer.LabelShort, (NamedArgument)(Thing)this.Wearer).CapitalizeFirst() + "."));
            else if (!this.CanBeUsed)
                commandReloadable.Disable(this.DisabledReason());
            return commandReloadable;
        }

        public bool CanBeUsed
        {
            get
            {
                return this.ticksUntilNextUse < 1;
            }
        }

        public string DisabledReason()
        {
            return "RH_TET_Magic_NeedsToCharge".Translate();
        }

        public static Pawn WearerOf(CompRechargeable comp)
        {
            return comp.ParentHolder is Pawn_ApparelTracker parentHolder ? parentHolder.pawn : (Pawn)null;
        }

        public void UsedOnce()
        {
            this.ticksUntilNextUse = this.Props.baseRechargeTicks;
        }
    }
}
