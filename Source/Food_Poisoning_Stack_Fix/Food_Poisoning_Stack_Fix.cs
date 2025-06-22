using System.Reflection;
using HarmonyLib;
using Verse;

namespace Food_Poisoning_Stack_Fix;

[StaticConstructorOnStartup]
public class Food_Poisoning_Stack_Fix
{
    static Food_Poisoning_Stack_Fix()
    {
        new Harmony("rimworld.dyrewulfe.fpstackfix").PatchAll(Assembly.GetExecutingAssembly());
    }
}