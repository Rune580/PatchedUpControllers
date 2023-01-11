// ReSharper disable InconsistentNaming

using Controllers;
using HarmonyLib;
using Kitchen;
using PatchedUpControllers.Saving;
using PatchedUpControllers.Structs;
using PatchedUpControllers.Utils;
using UnityEngine.InputSystem;

namespace PatchedUpControllers.Patches;

[HarmonyPatch(typeof(InputSource), nameof(InputSource.GetBindingString))]
public class InputSource_OnGetBindingString_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(int player, Dictionary<int, PlayerData> ___Players, ref string __result)
    {
        if (!___Players.TryGetValue(player, out var playerData))
            return false;

        List<BindingOverride> overrides = new List<BindingOverride>();

        InputDevice device = playerData.InputData.Device;
        InputActionMap map = playerData.InputData.Map;

        foreach (var action in map.actions)
        {
            foreach (var binding in action.bindings)
            {
                if(string.IsNullOrEmpty(binding.overridePath))
                    continue;
                
                overrides.Add(new BindingOverride(binding.action, binding.overridePath));
            }
        }

        string profile = Players.Main.Get(player).Profile.Name;
        if (!SaveManager.TryGetOverrides(profile, out var bindingOverrides))
            bindingOverrides = new DeviceBindingOverrides(device.name);

        bindingOverrides[device.name] = overrides;
        SaveManager.SetOverrides(profile, bindingOverrides);
        
        __result = SaveManager.GetOverridesJson(profile);

        return false;
    }
}

[HarmonyPatch(typeof(InputSource), nameof(InputSource.SetBindingString))]
public class InputSource_OnSetBindingString_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(int player, string json, Dictionary<int, PlayerData> ___Players)
    {
        if (string.IsNullOrEmpty(json) || !___Players.TryGetValue(player, out var playerData))
            return false;
        
        InputDevice device = playerData.InputData.Device;
        InputActionMap map = playerData.InputData.Map;

        DeviceBindingOverrides bindingOverrides = json.DeserializeAs<DeviceBindingOverrides>();

        foreach (var (deviceName, overrides) in bindingOverrides.DeviceOverrides)
        {
            if (deviceName != device.name)
                continue;

            foreach (var bindingOverride in overrides)
                map.FindAction(bindingOverride.Action).ApplyBindingOverride(bindingOverride.Override);
        }

        return false;
    }
}