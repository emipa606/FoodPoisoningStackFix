using HarmonyLib;
using RimWorld;
using Verse;

namespace Food_Poisoning_Stack_Fix;

[HarmonyPatch(typeof(CompFoodPoisonable), nameof(CompFoodPoisonable.PreAbsorbStack))]
public static class CompFoodPoisonable_PreAbsorbStack
{
    private static bool Prefix(CompFoodPoisonable __instance, ref float ___poisonPct,
        Thing otherStack, int count)
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

        var otherPoisonCount = (int)compFoodPoisonable.PoisonPercent;
        var otherStackCount = otherStack.stackCount;

        int drawnPoisoned;
        if (count >= otherStackCount)
        {
            // Full absorption - take all poisoned meals
            drawnPoisoned = otherPoisonCount;
        }
        else
        {
            // Partial absorption - we're only taking 'count' meals from the
            // other stack, so we pick them one at a time. For each pick, the
            // chance it's poisoned is (poisoned remaining / total remaining).
            // Each pick shrinks the pool, like drawing cards from a deck.
            drawnPoisoned = 0;
            var remainingPoisoned = otherPoisonCount;
            var remainingTotal = otherStackCount;
            for (var i = 0; i < count; i++)
            {
                if (remainingPoisoned > 0 &&
                    Rand.Chance(remainingPoisoned / (float)remainingTotal))
                {
                    drawnPoisoned++;
                    remainingPoisoned--;
                }

                remainingTotal--;
            }

            // Adjust the source stack so it no longer claims the drawn meals are poisoned
            AccessTools.Field(typeof(CompFoodPoisonable), "poisonPct")
                .SetValue(compFoodPoisonable, compFoodPoisonable.PoisonPercent - drawnPoisoned);
        }

        ___poisonPct += drawnPoisoned;
        return false;
    }
}
