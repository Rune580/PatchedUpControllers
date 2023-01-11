// ReSharper disable InconsistentNaming

using System.Reflection;
using System.Reflection.Emit;
using Controllers;
using HarmonyLib;
using Kitchen;
using PatchedUpControllers.Saving;
using PatchedUpControllers.Utils;

namespace PatchedUpControllers.Patches;

[HarmonyPatch(typeof(ProfileManager), "OnBindingChange")]
public class ProfileManager_OnBindingChange_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(int player, string action)
    {
        PlayerProfile profile = Players.Main.Get(player).Profile;
        if (!profile.IsRealProfile)
            return false;
        
        string json = InputSourceIdentifier.DefaultInputSource.GetBindingString(player);
        SaveManager.SetOverridesJson(profile.Name, json);
        
        return false;
    }
}

[HarmonyPatch(typeof(ProfileManager), nameof(ProfileManager.ApplyBindings))]
public class ProfileManager_OnApplyBindings_Patch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = instructions.ToList();

        int removeIndex = codes.FindIndexOfInstruction(0, OpCodes.Ldfld);
        int replaceIndex = codes.FindIndexOfInstruction(1, OpCodes.Callvirt);

        codes[removeIndex] = new CodeInstruction(OpCodes.Pop);

        MethodInfo methodInfo = typeof(SaveManager).GetMethod(nameof(SaveManager.TryGetOverridesJson))!;
        CodeInstruction replacement = new CodeInstruction(OpCodes.Call, methodInfo);

        codes[replaceIndex] = replacement;

        return codes;
    }
}

[HarmonyPatch(typeof(ProfileManager), nameof(ProfileManager.Save))]
public class ProfileManager_OnSave_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        SaveManager.Save();
    }
}

[HarmonyPatch(typeof(ProfileManager), nameof(ProfileManager.Load))]
public class ProfileManager_OnLoad_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        SaveManager.Load();
    }
}