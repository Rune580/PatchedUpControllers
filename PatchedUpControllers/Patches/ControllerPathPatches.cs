using Controllers;
using HarmonyLib;
using Kitchen;
using KitchenData;
using PatchedUpControllers.Utils;

// ReSharper disable InconsistentNaming

namespace PatchedUpControllers.Patches;

[HarmonyPatch(typeof(GameCreator), "PerformInitialSetup")]
public static class GameCreator_InitialSetup_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        var controllerIcons = GameData.Main.GlobalLocalisation.ControllerIcons.PathMapsByController;

        foreach (var (type, icons) in controllerIcons)
        {
            switch (type)
            {
                case ControllerType.Xbox:
                    icons.FixMappings(PSPathMap);
                    break;
                case ControllerType.Playstation:
                    icons.FixMappings(PSPathMap);
                    break;
            }
        }
    }

    private static void FixMappings(this List<ControllerPathMap> pathMaps, Dictionary<string, string> mapping)
    {
        for (var i = 0; i < pathMaps.Count; i++)
        {
            ControllerPathMap pathMap = pathMaps[i];
            
            if (!mapping.TryGetValue(pathMap.Control, out var button))
                continue;
            
            pathMap.Button = button;
            pathMaps[i] = pathMap;
        }
    }

    private static readonly Dictionary<string, string> PSPathMap = new()
    {
        {"leftStickPress", "Stick-L-Press"},
        {"rightStickPress", "Stick-R-Press"}
    };
}