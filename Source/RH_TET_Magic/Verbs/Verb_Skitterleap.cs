using SickAbilityUser;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    internal class Verb_Skitterleap : Verb_UseAbility
    {
        private bool validTarg = false;
        private bool validInBounds;
        private bool validTargetAndCaster;

        protected override bool TryCastShot()
        {
            // Miscast?
            if (MagicUtility.TryMiscast(this.caster))
                return true;

            MagicPower magicPower = null;
            foreach (MagicPower mp in this.CasterPawn.GetComp<CompMagicUser>().MagicData.PowersStealth)
            {
                if (mp.abilityDef.defName.Contains("Skitterleap"))
                {
                    magicPower = mp;
                }
            }

            if (magicPower == null)
            {
                Log.Error("Someone tried to cast Skitterleap, but didn't have the spell.");
                return false;
            }

            bool success = false;
            bool validCell;
            
            if (currentTarget != null && CasterPawn != null)
            {
                Vector3 centerVector3 = currentTarget.CenterVector3;
                IntVec3 cell = currentTarget.Cell;
                validCell = cell.IsValid;
                this.validInBounds = GenGrid.InBounds(centerVector3, CasterPawn.Map);
                this.validTargetAndCaster = true;
            }
            else
                validCell = false;

            if (validCell)
            {
                if (this.validInBounds & this.validTargetAndCaster)
                {
                    Pawn casterPawn = CasterPawn;
                    Map map = CasterPawn.Map;
                    IntVec3 position = CasterPawn.Position;
                    bool drafted = CasterPawn.Drafted;
                    try
                    {
                        if (CasterPawn.IsColonist)
                        {
                            RH_TET_MagicMod.SetSafeToRemoveMapNow(false);
                            ThingSelectionUtility.SelectNextColonist();
                            FleckMaker.ThrowSmoke(position.ToVector3(), map, 1.5f);
                            CasterPawn.DeSpawn(DestroyMode.Vanish);
                            GenSpawn.Spawn((Thing)casterPawn, currentTarget.Cell, map, WipeMode.Vanish);
                            FleckMaker.Static(currentTarget.Cell, map, FleckDefOf.PsycastAreaEffect, 1f);
                            casterPawn.drafter.Drafted = drafted;
                            CameraJumper.TryJumpAndSelect((GlobalTargetInfo)casterPawn);
                            RH_TET_MagicMod.SetSafeToRemoveMapNow(true);
                        }
                        else
                        {
                            RH_TET_MagicMod.SetSafeToRemoveMapNow(false);
                            FleckMaker.ThrowSmoke(position.ToVector3(), map, 1.5f);
                            CasterPawn.DeSpawn(DestroyMode.Vanish);
                            GenSpawn.Spawn((Thing)casterPawn, currentTarget.Cell, map, WipeMode.Vanish);
                            FleckMaker.Static(currentTarget.Cell, map, FleckDefOf.PsycastAreaEffect, 1f);
                            RH_TET_MagicMod.SetSafeToRemoveMapNow(true);
                        }

                        this.Ability.PostAbilityAttempt();
                    }
                    catch
                    {
                        if (!CasterPawn.Spawned)
                        {
                            GenSpawn.Spawn((Thing)casterPawn, position, map, WipeMode.Vanish);
                            Log.Message("Exception occured when trying to Skitterleap - recovered pawn at the position the spell was cast from.", false);
                        }
                    }
                    success = true;
                }
                else
                    Messages.Message((TaggedString)(Translator.Translate("RH_TET_Magic_InvalidTargetLocation")), (MessageTypeDef)MessageTypeDefOf.RejectInput, true);
            }
            else
                Log.Warning("Failed to Skitterleap. Contact SickBoyWI on Discord.", false);

            this.PostCastShot(success, out success);

            return success;
        }

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.IsValid && GenGrid.InBounds(targ.CenterVector3, CasterPawn.Map) && !GridsUtility.Fogged(targ.Cell, CasterPawn.Map) && GenGrid.Walkable(targ.Cell, CasterPawn.Map))
            {
                IntVec3 intVec3 = root - targ.Cell;
                this.validTarg = intVec3.LengthHorizontal < verbProps.range;
            }
            else
                this.validTarg = false;
            return this.validTarg;
        }
    }
}
