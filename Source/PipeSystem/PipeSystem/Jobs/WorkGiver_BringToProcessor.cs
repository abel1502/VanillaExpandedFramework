﻿using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PipeSystem
{
    public class WorkGiver_BringToProcessor : WorkGiver_Scanner
    {
        public override Danger MaxPathDanger(Pawn pawn) => Danger.Deadly;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => CachedAdvancedProcessorsManager.GetFor(pawn.Map).AwaitingIngredients;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            // TODO: Multiple pawns reservation
            if (t.IsBurning() || t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced) || t.Faction != pawn.Faction || pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
                return false;

            var comp = CachedCompAdvancedProcessor.GetFor(t);
            if (comp == null || comp.Process == null)
                return false;

            var firstMissing = comp.FirstIngredientMissing;
            if (firstMissing == null || FindIngredient(pawn, comp, firstMissing) == null)
                return false;

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var comp = CachedCompAdvancedProcessor.GetFor(t);
            return JobMaker.MakeJob(PSDefOf.PS_BringToProcessor, t, FindIngredient(pawn, comp, comp.FirstIngredientMissing));
        }

        private Thing FindIngredient(Pawn pawn, CompAdvancedResourceProcessor comp, ThingDef firstMissing)
        {
            if (firstMissing == null)
            {
                Log.Warning($"Tried to find ingredient for {comp.parent} but none is required.");
                return null;
            }

            bool validator(Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x);
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(firstMissing), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator);
        }
    }
}