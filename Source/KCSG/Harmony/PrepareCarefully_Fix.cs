﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace KCSG
{
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(RimWorld.Page_ConfigureStartingPawns))]
    [HarmonyPatch("PreOpen", MethodType.Normal)]
    public class PrepareCarefully_Fix
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            if (Current.Game.Scenario.AllParts.ToList().Any(s => s.def.defName == "VFEC_AddStartingStructure"))
            {
                PrepareCarefully_Util.pcScenariosSave.Clear();
                ScenPart_AddStartingStructure spart = (ScenPart_AddStartingStructure)Current.Game.Scenario.AllParts.ToList().Find(s => s.def.defName == "VFEC_AddStartingStructure");
                if (spart.chooseFrom?.Count > 0) PrepareCarefully_Util.pcScenariosSave.Add(spart.chooseFrom.RandomElement(), spart.nearMapCenter);
            }
        }
    }

    [StaticConstructorOnStartup]
    internal static class PrepareCarefully_Util
    {
        public static Dictionary<StructureLayoutDef, bool> pcScenariosSave = new Dictionary<StructureLayoutDef, bool>();
    }
}