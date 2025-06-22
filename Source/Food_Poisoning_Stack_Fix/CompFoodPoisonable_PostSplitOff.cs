using HarmonyLib;
using RimWorld;
using Verse;

namespace Food_Poisoning_Stack_Fix;

[HarmonyPatch(typeof(CompFoodPoisonable), nameof(CompFoodPoisonable.PostSplitOff))]
public static class CompFoodPoisonable_PostSplitOff
{
    private static bool Prefix(CompFoodPoisonable __instance, ref float ___poisonPct,
        FoodPoisonCause ___cause, Thing piece)
    {
        var compFoodPoisonable = piece.TryGetComp<CompFoodPoisonable>();
        if (__instance.parent.thingIDNumber == piece.thingIDNumber)
        {
            return false;
        }

        var num = (int)___poisonPct;
        var num2 = 0;
        if (num < 1)
        {
            return false;
        }

        for (var i = 0; i < piece.stackCount; i++)
        {
            if (Rand.Chance((num - num2) / (float)(__instance.parent.stackCount + piece.stackCount - i)))
            {
                num2++;
                num--;
            }

            if (num < 1)
            {
                break;
            }
        }

        if (num2 <= 0)
        {
            return false;
        }

        AccessTools.Field(typeof(CompFoodPoisonable), "poisonPct").SetValue(compFoodPoisonable, num2);
        AccessTools.Field(typeof(CompFoodPoisonable), "cause").SetValue(compFoodPoisonable, ___cause);
        ___poisonPct = num;

        return false;
    }
}