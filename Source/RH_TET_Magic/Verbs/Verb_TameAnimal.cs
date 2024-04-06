using SickAbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Magic
{
    public class Verb_TameAnimal : Verb_UseAbility
    {
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            return targ.IsValid && targ.CenterVector3.InBounds(this.CasterPawn.Map) && !targ.Cell.Fogged(this.CasterPawn.Map) && (double)(root - targ.Cell).LengthHorizontal < (double)(this.verbProps.range); 
        }

        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;

            bool flag1 = false;
            int spellLevel = -1;
            Pawn theCaster = caster as Pawn;

            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersBeast)
            {
                if (mp.abilityDef.defName.Contains("BeastMaster"))
                {
                    spellLevel = mp.level;
                }
            }

            if (spellLevel <= 0)
                return false;
            
            this.TargetsAoE.Clear();
            this.FindTargets();

            if (this.UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && this.TargetsAoE.Count > 1)
                ((List<LocalTargetInfo>)this.TargetsAoE).RemoveRange(0, this.TargetsAoE.Count - 1);

            if (this.TargetsAoE.Count <= 0)
                Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);

            Map map = theCaster.Map;
            FleckMaker.Static(theCaster.Position, map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 2f);
            FleckMaker.Static(theCaster.Position, map, FleckDefOf.PsycastAreaEffect, 1.5f);

            int failureTotal = 10 * spellLevel;
            if (RH_TET_MagicMod.random.Next(0, failureTotal) == 0)
            {
                // Failure. Make animal mad.
                for (int i = 0; i < this.TargetsAoE.Count; ++i)
                {
                    LocalTargetInfo localTargetInfo = this.TargetsAoE[i];

                    localTargetInfo = ((List<LocalTargetInfo>)this.TargetsAoE)[i];
                    Pawn thing = localTargetInfo.Thing as Pawn;
                        
                    IntVec3 position;
                        
                    if (thing.kindDef.RaceProps.Animal)
                    {
                        string text = "RH_TET_LetterBeastMasterSpellFail".Translate(theCaster.Name, thing).CapitalizeFirst();
                        Find.LetterStack.ReceiveLetter("RH_TET_LetterLabelBeastMasterSpellFail".Translate((NamedArgument)thing.KindLabel, (NamedArgument)((Thing)thing)).CapitalizeFirst(), text, LetterDefOf.NegativeEvent, (LookTargets)((Thing)thing), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);

                        thing.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, (string)null, true, false, (Pawn)null, false);

                        position = thing.Position;
                        FleckMaker.Static(position, map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 1.5f);
                        FleckMaker.Static(position, map, FleckDefOf.PsycastAreaEffect, .5f);
                    }
                }
            }
            else
            {
                // Tame animal.
                for (int i = 0; i < this.TargetsAoE.Count; ++i)
                {
                    LocalTargetInfo localTargetInfo = this.TargetsAoE[i];

                    // Do not target pawns that are already members of the caster's faction.
                    if (localTargetInfo.Thing.Faction != this.CasterPawn.Faction)
                    {
                        localTargetInfo = ((List<LocalTargetInfo>)this.TargetsAoE)[i];
                        Pawn thing = localTargetInfo.Thing as Pawn;

                        // Flag2 = if it is a manhunter
                        bool manhunterFlag = thing.mindState.mentalStateHandler.CurStateDef == MentalStateDefOf.ManhunterPermanent
                            || thing.mindState.mentalStateHandler.CurStateDef == MentalStateDefOf.Manhunter;
                        IntVec3 position;

                        // If manhunter, and is an animal.
                        if (thing.kindDef.RaceProps.Animal && !thing.kindDef.RaceProps.Humanlike)
                        {
                            position = thing.Position;
                            if (manhunterFlag)
                            {
                                // Clear manhunter mindstate.
                                thing.mindState.mentalStateHandler.Reset();
                                thing.jobs.StopAll(false);
                            }
                            if (thing.guest != null)
                                    thing.guest.SetGuestStatus((Faction)null);
                            string str = thing.LabelIndefinite();
                            bool flag = thing.Name != null;
                            thing.SetFaction(Faction.OfPlayer, (Pawn)null);

                            FleckMaker.Static(position, map, RH_TET_MagicDefOf.RH_TET_FleckBrownEffect, 1.5f);
                            FleckMaker.Static(position, map, FleckDefOf.PsycastAreaEffect, .5f);

                            if (spellLevel >= 4)
                            {
                                // Set all trainable options to true on the animal.
                                int trainableCount = 0;

                                List<TrainableDef> trainableDefsInListOrder = TrainableUtility.TrainableDefsInListOrder;
                                for (int j = 0; j < trainableDefsInListOrder.Count; ++j)
                                {
                                    bool visible = false;
                                    AcceptanceReport ar = thing.training.CanAssignToTrain(trainableDefsInListOrder[j], out visible);
                                    if (ar.Accepted)
                                    {
                                        trainableCount++;
                                        thing.training.SetWantedRecursive(trainableDefsInListOrder[j], true);
                                    }
                                }
                                
                                switch (trainableCount)
                                {
                                    case 1:
                                        trainableCount = 0;
                                        break;
                                    case 2:
                                        trainableCount = 3;
                                        break;
                                    case 3:
                                        trainableCount = 5;
                                        break;
                                    case 4:
                                        trainableCount = 7;
                                        break;
                                    case 5:
                                        trainableCount = 14;
                                        break;
                                    default:
                                        break;
                                }
                                
                                if (trainableCount > 0)
                                { 
                                    int trainingNumber = RH_TET_MagicMod.random.Next(0, trainableCount + 1);
                                    
                                    for (int k = 0; k < trainingNumber; k++)
                                    {
                                        TrainableDef train = thing.training.NextTrainableToTrain();
                                        thing.training.Train(train, theCaster, false);
                                        //if (thing.caller != null)
                                        //    thing.caller.DoCall();
                                        //TaleRecorder.RecordTale(TaleDefOf.TrainedAnimal, (object)theCaster, (object)thing, (object)train);
                                    }
                                }
                            }

                            RelationsUtility.TryDevelopBondRelation(theCaster, thing, 1.0f);

                            string text = "RH_TET_LetterBeastMasterSpellSuccess".Translate(theCaster.Name, thing.KindLabel).CapitalizeFirst();
                            Find.LetterStack.ReceiveLetter("RH_TET_LetterLabelBeastMasterSpellSuccess".Translate((NamedArgument)thing.KindLabel, (NamedArgument)((Thing)thing)).CapitalizeFirst(), text, LetterDefOf.PositiveEvent, (LookTargets)((Thing)thing), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
                        }
                    }
                    else
                    {
                        Pawn thing = localTargetInfo.Thing as Pawn;

                        if(thing != null && !thing.RaceProps.Humanlike)
                        { 
                            bool manhunterFlag = thing.mindState.mentalStateHandler.CurStateDef == MentalStateDefOf.ManhunterPermanent
                                || thing.mindState.mentalStateHandler.CurStateDef == MentalStateDefOf.Manhunter;
                        
                            IntVec3 position;

                            if (manhunterFlag)
                            {
                                // Clear manhunter mindstate.
                                thing.mindState.mentalStateHandler.Reset();
                                thing.jobs.StopAll(false);
                                position = thing.Position;
                                FleckMaker.ThrowMicroSparks(position.ToVector3().normalized, thing.Map);
                            }
                        
                            string text = "RH_TET_LetterBeastMasterSpellSuccessTwo".Translate(theCaster.Name, thing.KindLabel).CapitalizeFirst();
                            Find.LetterStack.ReceiveLetter("RH_TET_LetterLabelBeastMasterSpellSuccess".Translate((NamedArgument)thing.KindLabel, (NamedArgument)((Thing)thing)).CapitalizeFirst(), text, LetterDefOf.PositiveEvent, (LookTargets)((Thing)thing), (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
                        }
                        else
                        {
                            Messages.Message("RH_TET_MessageSpellFailTargets".Translate(theCaster.Name), MessageTypeDefOf.NegativeEvent);
                        }
                    }
                }
            }
            
            this.PostCastShot(flag1, out flag1);
            return flag1;
        }

        private void FindTargets()
        {
            if (this.UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                if (this.UseAbilityProps.TargetAoEProperties == null)
                    Log.Error("Tried to Cast AoE-Ability without defining a target class");
                List<Thing> thingList = new List<Thing>();
                IntVec3 aoeStartPosition = ((Verb)this).caster.PositionHeld;
                if (!((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).startsFromCaster)
                    aoeStartPosition = this.currentTarget.Cell;
                List<Thing> list;

                if (!((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).friendlyFire)
                    list = ((Verb)this).caster.Map.listerThings.AllThings.Where<Thing>((Func<Thing, bool>)(x =>
                    {
                        if (x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range) && ((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType()))
                            return x.Faction != Faction.OfPlayer;
                        return false;
                    })).ToList<Thing>();

                else if (((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass == typeof(Plant) || ((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass == typeof(Building))
                {
                    using (List<Thing>.Enumerator enumerator = ((Verb)this).caster.Map.listerThings.AllThings.Where<Thing>((Func<Thing, bool>)(x =>
                    {
                        if (x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range))
                            return ((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType());
                        return false;
                    })).ToList<Thing>().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            ((List<LocalTargetInfo>)this.TargetsAoE).Add(new LocalTargetInfo(enumerator.Current));
                        return;
                    }
                }
                else
                {
                    thingList.Clear();
                    list = ((Verb)this).caster.Map.listerThings.AllThings.Where<Thing>((Func<Thing, bool>)(x =>
                    {
                        if (!x.Position.InHorDistOf(aoeStartPosition, (float)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).range) || !((System.Type)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).targetClass).IsAssignableFrom(x.GetType()))
                            return false;
                        if (!x.HostileTo(Faction.OfPlayer))
                            return (bool)((TargetAoEProperties)this.UseAbilityProps.TargetAoEProperties).friendlyFire;
                        return true;
                    })).ToList<Thing>();
                }
                int maxTargets = (int)((TargetAoEProperties)((VerbProperties_Ability)((SickAbilityUser.AbilityDef)this.UseAbilityProps.abilityDef).MainVerb).TargetAoEProperties).maxTargets;
                List<Thing> source = new List<Thing>(list.InRandomOrder<Thing>((IList<Thing>)null));
                for (int index = 0; index < maxTargets && index < source.Count<Thing>(); ++index)
                {
                    if (((VerbProperties)this.UseAbilityProps).targetParams.CanTarget(new TargetInfo(source[index])))
                        ((List<LocalTargetInfo>)this.TargetsAoE).Add(new LocalTargetInfo(source[index]));
                }
            }
            else
            {
                ((List<LocalTargetInfo>)this.TargetsAoE).Clear();
                ((List<LocalTargetInfo>)this.TargetsAoE).Add(this.currentTarget);
            }
        }

        public Verb_TameAnimal()
            : base()
        {
        }
    }
}
