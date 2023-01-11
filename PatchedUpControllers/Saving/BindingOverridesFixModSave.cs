using MessagePack;

namespace PatchedUpControllers.Saving;

[MessagePackObject]
public class BindingOverridesFixModSave
{
    [Key(0)]
    public Dictionary<string, string> ControlOverrides = new();
}