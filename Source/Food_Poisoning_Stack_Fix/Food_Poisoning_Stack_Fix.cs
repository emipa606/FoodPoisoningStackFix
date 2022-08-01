using HarmonyLib;
using RimWorld;
using Verse;

namespace Food_Poisoning_Stack_Fix;

public class Food_Poisoning_Stack_Fix
{
    [StaticConstructorOnStartup]
    private static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmonyInstance = new Harmony("rimworld.dyrewulfe.fpstackfix");
            harmonyInstance.Patch(AccessTools.Method(typeof(CompFoodPoisonable), "PostSplitOff"),
                new HarmonyMethod(typeof(HarmonyPatches), "PostSplitOff_Prefix"));
            harmonyInstance.Patch(AccessTools.Method(typeof(CompFoodPoisonable), "PreAbsorbStack"),
                new HarmonyMethod(typeof(HarmonyPatches), "PreAbsorbStack_Prefix"));
        }

        private static bool PostSplitOff_Prefix(CompFoodPoisonable __instance, ref float ___poisonPct,
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

            Traverse.Create(compFoodPoisonable).Field("poisonPct").SetValue((float)num2);
            compFoodPoisonable.cause = ___cause;
            ___poisonPct = num;

            return false;
        }

        private static bool PreAbsorbStack_Prefix(CompFoodPoisonable __instance, ref float ___poisonPct,
            Thing otherStack)
        {
            var compFoodPoisonable = otherStack.TryGetComp<CompFoodPoisonable>();
            if (__instance.cause == FoodPoisonCause.Unknown && compFoodPoisonable.cause != 0)
            {
                __instance.cause = compFoodPoisonable.cause;
            }
            else if (compFoodPoisonable.cause != 0 || __instance.cause != 0)
            {
                __instance.cause = ___poisonPct <= compFoodPoisonable.PoisonPercent
                    ? compFoodPoisonable.cause
                    : __instance.cause;
            }

            ___poisonPct += compFoodPoisonable.PoisonPercent;
            return false;
        }
    }
}