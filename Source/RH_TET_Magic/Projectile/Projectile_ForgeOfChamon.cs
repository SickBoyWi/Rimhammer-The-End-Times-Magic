using AbilityUser;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace TheEndTimes_Magic
{
    [StaticConstructorOnStartup]
    public class Projectile_ForgeOfChamon : Projectile_Ability
    {
        public Projectile_ForgeOfChamon()
            : base()
        {
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing);
            FleckMaker.Static(this.Caster.Position, this.Caster.Map, FleckDefOf.PsycastAreaEffect, 1f);
            FleckMaker.ThrowMicroSparks(this.Caster.Position.ToVector3(), this.Caster.Map);
            IntVec3 target = this.usedTarget.CenterVector3.ToIntVec3();
            bool fail = false;

            try
            {
                List<Thing> thingsAtTargetPos = target.GetThingList(this.Caster.Map);
                if (thingsAtTargetPos != null && thingsAtTargetPos.Count > 0)
                {
                    Thing thing = null;
                    foreach (Thing thingTemp in thingsAtTargetPos)
                    {
                        if (thingTemp.def.IsApparel || thingTemp.def.IsWeapon)
                        {
                            thing = thingTemp;
                            break;
                        }
                    }

                    if (thing == null)
                    {
                        fail = true;
                    }
                    else
                    { 
                        if (thing.HitPoints == thing.MaxHitPoints)
                        {
                            CompQuality comp = thing.TryGetComp<CompQuality>();
                            if (comp != null)
                            {
                                if (comp.Quality < QualityCategory.Legendary)
                                { 
                                    comp.SetQuality(comp.Quality + (byte)1, ArtGenerationContext.Colony);
                                    Messages.Message("RH_TET_MessageForgeOfChamonSuccessQuality".Translate(this.Caster.Name, thing.def.label), thing, MessageTypeDefOf.PositiveEvent);
                                }
                                else
                                    fail = true;
                            }
                            else
                                fail = true;
                        }
                        else
                        {
                            // Repair item to full hit points.
                            thing.HitPoints = thing.MaxHitPoints;
                            Messages.Message("RH_TET_MessageForgeOfChamonSuccessHP".Translate(this.Caster.Name, thing.def.label), thing, MessageTypeDefOf.PositiveEvent);
                        }
                    }
                }
            }
            catch
            {
                fail = true;
            }

            if (fail)
            {
                Messages.Message("RH_TET_MessageForgeOfChamonFail".Translate(this.Caster.Name), this.Caster, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
