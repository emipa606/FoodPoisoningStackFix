using HarmonyLib;
using RimWorld;
using Verse;

namespace Food_Poisoning_Stack_Fix;

[HarmonyPatch(typeof(ThingComp), nameof(ThingComp.CompInspectStringExtra))]
public static class ThingComp_CompInspectStringExtra
{
    private static void Postfix(ThingComp __instance, ref string __result)
    {
        if (!Prefs.DevMode || __instance is not CompFoodPoisonable comp)
        {
            return;
        }

        var poisonPct = (float)AccessTools.Field(typeof(CompFoodPoisonable), "poisonPct").GetValue(comp);
        var cause = (FoodPoisonCause)AccessTools.Field(typeof(CompFoodPoisonable), "cause").GetValue(comp);
        var poisonCount = (int)poisonPct;
        var stackCount = comp.parent.stackCount;

        if (poisonCount <= 0 && poisonPct == 0f && cause == FoodPoisonCause.Unknown)
        {
            // No poison
            __result = __result == null ? "Not poisoned" : $"{__result}\nNot poisoned";
            return;
        }

        string text;
        if (poisonCount >= 1)
        {
            text = $"Poisoned: {poisonCount} of {stackCount} ({cause})";
        }
        else if (poisonPct is > 0f and < 1f)
        {
            // Fractional poisonPct - came from a vanilla (unpatched) source
            text = $"Poison chance: {poisonPct * 100f:F0}% (vanilla format)";
        }
        else
        {
            // poisonPct is 0 but cause is set - stale leftover from split
            text = $"Safe (stale cause: {cause})";
        }

        __result = __result == null ? text : $"{__result}\n{text}";
    }
}