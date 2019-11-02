using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace Food_Poisoning_Stack_Fix
{
    public class Food_Poisoning_Stack_Fix
    {
        [StaticConstructorOnStartup]
        static class HarmonyPatches
        {
            static HarmonyPatches()
            {
                HarmonyInstance harmony = HarmonyInstance.Create("rimworld.dyrewulfe.fpstackfix");

                harmony.Patch(AccessTools.Method(typeof(RimWorld.CompFoodPoisonable), "PostSplitOff"),
                    new HarmonyMethod(typeof(HarmonyPatches), nameof(PostSplitOff_Prefix)));

                harmony.Patch(AccessTools.Method(typeof(RimWorld.CompFoodPoisonable), "PreAbsorbStack"),
                    new HarmonyMethod(typeof(HarmonyPatches), nameof(PreAbsorbStack_Prefix)));
            }

            private static bool PostSplitOff_Prefix(CompFoodPoisonable __instance, ref float ___poisonPct, FoodPoisonCause ___cause, Thing piece)
            {
                CompFoodPoisonable compFoodPoisonable = piece.TryGetComp<CompFoodPoisonable>();

                if (__instance.parent.thingIDNumber == piece.thingIDNumber)
                {
                    return false;
                }

                var poisoned = (int)___poisonPct;
                var otherPoisoned = 0;

                if (poisoned < 1) return false;

                for (int i = 0; i < piece.stackCount; i++)
                {
                    float chance = (float)(poisoned - otherPoisoned) / (float)(__instance.parent.stackCount + piece.stackCount - i);
                    if (Rand.Chance(chance))
                    {
                        otherPoisoned++;
                        poisoned--;
                    }
                    if (poisoned < 1) break;
                }

                if (otherPoisoned > 0)
                {
                    Traverse.Create(compFoodPoisonable).Field("poisonPct").SetValue((float)(otherPoisoned));
                    compFoodPoisonable.cause = ___cause;
                    ___poisonPct = (float)poisoned;
                }

                return false;
            }

            private static bool PreAbsorbStack_Prefix(CompFoodPoisonable __instance, ref float ___poisonPct, Thing otherStack)
            {
                CompFoodPoisonable compFoodPoisonable = otherStack.TryGetComp<CompFoodPoisonable>();

                if (__instance.cause == FoodPoisonCause.Unknown && compFoodPoisonable.cause != FoodPoisonCause.Unknown)
                {
                    __instance.cause = compFoodPoisonable.cause;
                }
                else if (compFoodPoisonable.cause != FoodPoisonCause.Unknown || __instance.cause != FoodPoisonCause.Unknown)
                {
                    __instance.cause = ((___poisonPct <= compFoodPoisonable.PoisonPercent) ? compFoodPoisonable.cause : __instance.cause);
                }

                ___poisonPct += compFoodPoisonable.PoisonPercent;

                return false;
            }

        }
    }

}
