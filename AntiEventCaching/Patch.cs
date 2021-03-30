using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AntiEventCaching
{
    public class Patch
    {
        public static readonly HarmonyInstance HInstance = HarmonyInstance.Create("AntiEventCachingPatches");

        public Patch(Type PatchClass, Type YourClass, string Method, string ReplaceMethod, BindingFlags bf1 = BindingFlags.Static, BindingFlags bf2 = BindingFlags.NonPublic)
        {
            MelonLogger.Log($"Attempting to patch {ReplaceMethod}");
            HInstance.Patch(AccessTools.Method(PatchClass, Method, null, null), GetPatch(YourClass, ReplaceMethod, bf1, bf2));
            MelonLogger.Log($"Successfully patched {ReplaceMethod}");
        }

        private HarmonyMethod GetPatch(Type YourClass, string MethodName, BindingFlags stat, BindingFlags pub)
        {
            return new HarmonyMethod(YourClass.GetMethod(MethodName, stat | pub));
        }
    }
}
