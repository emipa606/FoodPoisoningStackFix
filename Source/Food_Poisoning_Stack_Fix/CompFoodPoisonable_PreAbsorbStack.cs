using HarmonyLib;
using RimWorld;
using Verse;

namespace Food_Poisoning_Stack_Fix;

[HarmonyPatch(typeof(CompFoodPoisonable), nameof(CompFoodPoisonable.PreAbsorbStack))]
public static class CompFoodPoisonable_PreAbsorbStack
{
    private static bool Prefix(CompFoodPoisonable __instance, ref float ___poisonPct,
        Thing otherStack)
    {
        var compFoodPoisonable = otherStack.TryGetComp<CompFoodPoisonable>();

        var causeField = AccessTools.Field(typeof(CompFoodPoisonable), "cause");
        var instanceValue = (FoodPoisonCause)causeField.GetValue(__instance);
        var otherValue = (FoodPoisonCause)causeField.GetValue(compFoodPoisonable);
        if (instanceValue == FoodPoisonCause.Unknown && otherValue != 0)
        {
            causeField.SetValue(__instance, otherValue);
        }
        else if (otherValue != 0 || instanceValue != 0)
        {
            causeField.SetValue(__instance,
                ___poisonPct <= compFoodPoisonable.PoisonPercent ? otherValue : instanceValue);
        }

        ___poisonPct += compFoodPoisonable.PoisonPercent;
        return false;
    }
}