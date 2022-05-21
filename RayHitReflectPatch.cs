using BepInEx;
using HarmonyLib;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace RayHitReflectPatch
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)] // necessary for most modding stuff here
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RayHitReflectPatch : BaseUnityPlugin
    {
        private const string ModId = "pykess.rounds.plugins.rayhitreflectpatch";
        private const string ModName = "RayHitReflectPatch";
        public const string Version = "0.0.0";
        private string CompatibilityModName => ModName.Replace(" ", "");

        public static RayHitReflectPatch instance;

        private Harmony harmony;

#if DEBUG
        public static readonly bool DEBUG = true;
#else
        public static readonly bool DEBUG = false;
#endif
        internal static void Log(string str)
        {
            if (DEBUG)
            {
                UnityEngine.Debug.Log($"[{ModName}] {str}");
            }
        }

        [HarmonyPatch(typeof(RayHitReflect), nameof(RayHitReflect.DoHitEffect))]
        class RayHitReflectPatchDoHitEffect
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var code in instructions)
                {
                    if (code.opcode == OpCodes.Ldc_I4_S && code.OperandIs(10))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_S, 0);
                    }
                    else
                    {
                        yield return code;
                    }
                }
            }
        }

        private void Awake()
        {
            instance = this;
            
            harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        private void Start()
        {

        }

        private void OnDestroy()
        {
            harmony.UnpatchAll();
        }

        internal static string GetConfigKey(string key) => $"{RayHitReflectPatch.ModName}_{key}";
    }
}