using System.Windows;

namespace Net.Leksi.Pocota.Client;

public interface IEditWindow
{
    EditWindowCore EditWindowCore { get; }
    Window? LaunchedBy { get; set; }
    object? Value { get; set; }
}
