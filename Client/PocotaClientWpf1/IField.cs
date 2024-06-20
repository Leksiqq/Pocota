using System.ComponentModel;
using System.Windows;

namespace Net.Leksi.Pocota.Client;

public interface IField: INotifyPropertyChanged
{
    [Flags]
    public enum WaitingForFlags
    {
        None,
        PropertyName,
        Target,
        Field,
        Any,
    }
    IFieldOwner? Owner { get; set; }
    object? Target { get; set; }
    string? PropertyName { get; set; }
    EntityProperty? EntityProperty { get; }
    bool IsReady { get; }
    Type Type { get; }
    bool IsNullable { get; }
    bool IsCollection { get; }
    bool IsReadonly { get; }
    object? Value { get; set; }
    bool IsClean {  get; }
    void Clear();
    public static bool CanProcessProperty(WaitingForFlags propertyFlags, WaitingForFlags propertyFlag)
    {
        if(propertyFlags is not WaitingForFlags.Any && !propertyFlags.HasFlag(propertyFlag))
        {
            return false;
        }
        if(propertyFlag is WaitingForFlags.Field)
        {
            propertyFlags = WaitingForFlags.None;
        }
        else if (propertyFlag is WaitingForFlags.PropertyName)
        {
            propertyFlags = WaitingForFlags.Target;
        }
        else if (propertyFlag is WaitingForFlags.Target)
        {
            propertyFlags = WaitingForFlags.PropertyName;
        }
        return true;
    }
}
