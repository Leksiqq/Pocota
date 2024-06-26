﻿using System.Windows;

namespace Net.Leksi.Pocota.Client;

public interface IEditWindow: IWindowLauncher
{
    EditWindowCore EditWindowCore { get; }
    Window? LaunchedBy { get; set; }
    Property? Property { get; set; }
}
