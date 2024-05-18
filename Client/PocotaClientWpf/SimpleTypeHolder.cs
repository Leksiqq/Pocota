using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;
public class SimpleTypeHolder<T>(IList<T> source): INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly PropertyChangedEventArgs _valueChangedEventArgs = new(nameof(Value));
    public int Position { get; set; }
    public T Value 
    { 
        get => source[Position];
        set
        {
            source[Position] = value;
            PropertyChanged?.Invoke(this, _valueChangedEventArgs);
        }
    }
}
