using HarmonyLib;
using KitchenMods;

namespace PatchedUpControllers;

public class PatchedUpControllersMod : IModInitializer
{
    public const string AUTHOR = "rune580";
    public const string MOD_NAME = "patchedup_controllers";
    public const string MOD_ID = $"com.{AUTHOR}.{MOD_NAME}";
    public void PostActivate(Mod mod)
    {
        Harmony harmony = new Harmony(MOD_ID);
        
        harmony.PatchAll();
    }

    public void PreInject()
    {
        
    }

    public void PostInject()
    {
        
    }
}