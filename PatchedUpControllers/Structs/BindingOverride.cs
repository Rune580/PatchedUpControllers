namespace PatchedUpControllers.Structs;

[Serializable]
public struct BindingOverride
{
    public string Action;
    public string Override;

    public BindingOverride(string action, string @override)
    {
        Action = action;
        Override = @override;
    }
}