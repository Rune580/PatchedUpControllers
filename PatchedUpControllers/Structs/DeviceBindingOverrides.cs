namespace PatchedUpControllers.Structs;

[Serializable]
public struct DeviceBindingOverrides
{
    public Dictionary<string, List<BindingOverride>> DeviceOverrides = new();

    public DeviceBindingOverrides()
    {
        DeviceOverrides = new Dictionary<string, List<BindingOverride>>();
    }
    
    public DeviceBindingOverrides(string initialDevice)
    {
        DeviceOverrides = new Dictionary<string, List<BindingOverride>>
        {
            [initialDevice] = new()
        };
    }

    public List<BindingOverride> this[string deviceName]
    {
        get => DeviceOverrides[deviceName];
        set => DeviceOverrides[deviceName] = value;
    }
}