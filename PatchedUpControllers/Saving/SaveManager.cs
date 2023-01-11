using MessagePack;
using Newtonsoft.Json;
using PatchedUpControllers.Structs;
using PatchedUpControllers.Utils;
using UnityEngine;

namespace PatchedUpControllers.Saving;

public static class SaveManager
{
    private static BindingOverridesFixModSave _saveData = new();
    private static readonly Dictionary<string, DeviceBindingOverrides> ControlOverrides = new();

    public static void SetOverridesJson(string profile, string json)
    {
        _saveData.ControlOverrides[profile] = json;
        
        Save();
    }

    public static string GetOverridesJson(string profile)
    {
        Load();

        if (!_saveData.ControlOverrides.TryGetValue(profile, out var json))
            return "";

        return json;
    }

    public static bool TryGetOverridesJson(string profile, out string json)
    {
        Load();
        return _saveData.ControlOverrides.TryGetValue(profile, out json);
    }

    public static bool TryGetOverrides(string profile, out DeviceBindingOverrides deviceBindingOverrides)
    {
        Load();
        return ControlOverrides.TryGetValue(profile, out deviceBindingOverrides);
    }

    public static void SetOverrides(string profile, DeviceBindingOverrides deviceBindingOverrides)
    {
        ControlOverrides[profile] = deviceBindingOverrides;
        Save();
    }

    public static void Save()
    {
        foreach (var (profile, overrides) in ControlOverrides)
            _saveData.ControlOverrides[profile] = overrides.Serialize();
        
        byte[] data = MessagePackSerializer.Serialize(_saveData, MessagePackSerializerOptions.Standard);
        File.WriteAllBytes(SaveDataPath(), data);
    }

    public static void Load(bool throwIfFailed = false)
    {
        string path = SaveDataPath();
        
        if (!File.Exists(path))
            return;

        FileStream stream = File.OpenRead(path);
        _saveData = MessagePackSerializer.Deserialize<BindingOverridesFixModSave>(stream, MessagePackSerializerOptions.Standard);
        stream.Dispose();
        
        try
        {
            foreach (var (profile, json) in _saveData.ControlOverrides)
                ControlOverrides[profile] = json.DeserializeAs<DeviceBindingOverrides>();
        }
        catch (JsonReaderException) // save file is in an invalid state. generate a default and overwrite
        {
            if (throwIfFailed)
                throw;
            
            _saveData = new BindingOverridesFixModSave();
            ControlOverrides.Clear();
            
            Save();
            Load(true);
        }
    }

    private static string SaveDataPath()
    {
        return Path.Combine(Application.persistentDataPath, "bindings_overrides_fix.data");
    }
}