
using System.Collections;

namespace Net.Leksi.Pocota.Client;

public class SimpleListItemProperty(IList source, Type itemType) : Property(string.Empty, itemType)
{
    public int Position { get; set; }
    public override object? Value
    {
        get => source[Position];
        set
        {
            source[Position] = value;
            NotifyPropertyChanged();
        }
    }
}
