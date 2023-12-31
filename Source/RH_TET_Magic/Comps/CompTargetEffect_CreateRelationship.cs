using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Magic
{
    public class CompTargetEffect_CreateRelationship : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn targetP = target as Pawn;

            if (targetP == null)
                return; 

            Gender userGender = user.gender;
            bool userIsGay = user.story.traits.HasTrait(TraitDefOf.Gay);
            bool userIsAsexual = user.story.traits.HasTrait(TraitDefOf.Asexual);
            bool userHasPartner = LovePartnerRelationUtility.HasAnyLovePartner(targetP);
            Gender targetGender = targetP.gender;
            bool targetIsGay = targetP.story.traits.HasTrait(TraitDefOf.Gay);
            bool targetIsAsexual = targetP.story.traits.HasTrait(TraitDefOf.Asexual);
            bool targetHasPartner = LovePartnerRelationUtility.HasAnyLovePartner(targetP);

            // Confirm the target is appropriate considering gender, orientation, family to user, and partner status.
            if (userHasPartner)
            {
                Pawn partner = LovePartnerRelationUtility.ExistingLovePartner(user);


                if (partner != null && !partner.Dead)
                {
                    Messages.Message("RH_TET_LovePotion_NoUseUserPartner".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NeutralEvent, true);
                    return;
                }
            }
            else if (targetHasPartner)
            {
                Pawn partner = LovePartnerRelationUtility.ExistingLovePartner(targetP);

                if (partner != null && !partner.Dead)
                { 
                    Messages.Message("RH_TET_LovePotion_NoUseTargetPartner".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NeutralEvent, true);
                    return;
                }
            }

            if (userIsAsexual)
            {
                Messages.Message("RH_TET_LovePotion_NoUseUserAsexual".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NeutralEvent, true);
                return;
            }
            else if (targetIsAsexual)
            {
                Messages.Message("RH_TET_LovePotion_NoUseTargetAsexual".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NeutralEvent, true);
                return;
            }

            if (!RelationIsAppropriate(userGender, userIsGay, targetGender, targetIsGay))
            {
                Messages.Message("RH_TET_LovePotion_NoUseInappropriate".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NeutralEvent, true);
                return;
            }

            IEnumerable<PawnRelationDef> relations = user.GetRelations(targetP);
            if (relations != null) 
            { 
                foreach(PawnRelationDef rel in relations)
                {
                    if (rel.Equals(PawnRelationDefOf.Child) || rel.Equals(PawnRelationDefOf.Cousin) || rel.Equals(PawnRelationDefOf.Grandchild)
                        || rel.Equals(PawnRelationDefOf.Grandparent) || rel.Equals(PawnRelationDefOf.GranduncleOrGrandaunt) || rel.Equals(PawnRelationDefOf.GreatGrandchild)
                        || rel.Equals(PawnRelationDefOf.GreatGrandparent) || rel.Equals(PawnRelationDefOf.HalfSibling) || rel.Equals(PawnRelationDefOf.Kin)
                        || rel.Equals(PawnRelationDefOf.NephewOrNiece) || rel.Equals(PawnRelationDefOf.Parent) || rel.Equals(PawnRelationDefOf.Sibling)
                        || rel.Equals(PawnRelationDefOf.UncleOrAunt))
                    {
                        Messages.Message("RH_TET_LovePotion_NoUseRelation".Translate(user.Name), (LookTargets)((Thing)user), MessageTypeDefOf.NeutralEvent, true);
                        return;
                    }
                }
            }

            // Create love relationship between the two pawns.
            TaleRecorder.RecordTale(TaleDefOf.BecameLover, user, targetP);
            user.relations.TryRemoveDirectRelation(PawnRelationDefOf.ExLover, targetP);
            // This will remove dead lovers.
            user.relations.TryRemoveDirectRelation(PawnRelationDefOf.Lover, targetP);
            user.relations.TryRemoveDirectRelation(PawnRelationDefOf.Lover, user);

            user.relations.AddDirectRelation(PawnRelationDefOf.Lover, targetP);
            LovePartnerRelationUtility.TryToShareBed(user, targetP);

            // Destroy item.
            this.parent.SplitOff(1).Destroy(DestroyMode.Vanish);

            Messages.Message("RH_TET_LovePotion_NewRelationship".Translate(user.Name, targetP.Name), (LookTargets)((Thing)user), MessageTypeDefOf.PositiveEvent, true);
            return;
        }

        private bool RelationIsAppropriate(Gender oneGender, bool oneIsGay, Gender twoGender, bool twoIsGay)
        {
            // if is gay, genders must match, else, must not match.
            if (Gender.None.Equals(oneGender) || Gender.None.Equals(twoGender))
                return false;
            if (oneIsGay || twoIsGay)
                return oneGender.Equals(twoGender);
            else
                return !oneGender.Equals(twoGender);
        }
    }
}
