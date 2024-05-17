using System.ComponentModel;

namespace Net.Leksi.Pocota.Client;
public class SimpleTypeHolder<T>(IList<T> source): INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public int Position { get; set; }
    public T Value 
    { 
        get => source[Position]; 
        set => source[Position] = value; 
    }
}
