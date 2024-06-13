using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client;
public partial class WindowsWindow : Window, INotifyPropertyChanged, IValueConverter
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly StringBuilder _sb = new();
    private Window? _activeWindow = null;
    private Dictionary<Window, int> _levels = [];
    public ObservableCollection<Window> Windows { get; private init; } = [];
    public Window? ActiveWindow
    {
        get => _activeWindow; 
        set 
        { 
            _activeWindow = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveWindow)));
        }
    }
    public WindowsWindow()
    {
        InitializeComponent();
        ListView.Focus();
    }
    public void Clear()
    {
        Windows.Clear();
        _levels.Clear();
    }
    public void Add(Window window, int level)
    {
        Windows.Add(window);
        _levels.Add(window, level);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is Window window)
        {
            _sb.Clear();
            if (_levels[window] is int level && level > 0)
            {
                for(int i = 0; i < level - 1; ++i)
                {
                    _sb.Append(System.Convert.ToChar(160)).Append(System.Convert.ToChar(160));
                }
                _sb.Append("└─ ");
            }
            _sb.Append(window.Title);
            return _sb.ToString();
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if(sender == Goto)
        {
            DialogResult = true;
        }
        else if (sender == Cancel)
        {
            DialogResult = false;
        }
    }

    private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DialogResult = true;
    }
}
