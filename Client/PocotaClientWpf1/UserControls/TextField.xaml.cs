using System.Globalization;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class TextField : UserControl, IValueConverter, IFieldOwner, ICommand, IInputElement
{
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }
    private const int s_defaultChangeHeight = 5;
    public static readonly DependencyProperty FieldProperty = DependencyProperty.Register(
       nameof(Field), typeof(IField),
       typeof(TextField)
    );
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
       nameof(Target), typeof(object),
       typeof(TextField)
    );
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
       nameof(PropertyName), typeof(string),
       typeof(TextField)
    );
    public static readonly DependencyProperty ChangeHeightProperty = DependencyProperty.Register(
       nameof(ChangeHeight), typeof(int),
       typeof(TextField)
    );
    private object? _value;
    private string? _badFormat = null;
    private ObjectEditor? _objectEditor = null;
    private int _expectedCaretIndex = -1;
    private double _initialHeight = 0;
    private IField.WaitingForFlags _waitingFor = IField.WaitingForFlags.Any;
    public IField? Field 
    {
        get => (IField?)GetValue(FieldProperty);
        set => SetValue(FieldProperty, value);
    }
    public object? Target
    {
        get => (object?)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }
    public string? PropertyName
    {
        get => (string?)GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }
    public int ChangeHeight
    {
        get => (int)GetValue(ChangeHeightProperty);
        set => SetValue(ChangeHeightProperty, value);
    }
    public bool IsInsertMode { get; private set; } = true;
    private ObjectEditor? Editor
    {
        get
        {
            if (_objectEditor is null)
            {
                if (GetAncestors(this).OfType<ObjectEditor>().FirstOrDefault() is ObjectEditor oe)
                {
                    _objectEditor = oe;
                }
            }
            return _objectEditor;
        }
    }

    public TextField()
    {
        ChangeHeight = s_defaultChangeHeight;
        InitializeComponent();
    }
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(Field?.IsReady ?? false)
        {
            if ("Foreground".Equals(parameter))
            {
                return _badFormat is { } ? Brushes.Red : Brushes.Black;
            }
            if ("Text".Equals(parameter))
            {
                _value = value;
                if (_badFormat is { })
                {
                    return _badFormat;
                }
                return ToString(value, Field.Type);
            }
        }
        return value;
    }
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Field?.IsReady ?? false)
        {
            if ("Text".Equals(parameter))
            {
                if (value is string valueAsString)
                {
                    if (Field.IsNullable)
                    {
                        if (string.IsNullOrWhiteSpace(valueAsString))
                        {
                            _badFormat = null;
                            return null;
                        }
                    }
                    else if (string.IsNullOrWhiteSpace(valueAsString))
                    {
                        _badFormat = null;
                        return Activator.CreateInstance(Field.Type);
                    }
                    try
                    {
                        if (TryParse(valueAsString, Field.Type, out object? res))
                        {
                            _badFormat = null;
                            return res;
                        }
                        return BadFormat(valueAsString);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return BadFormat(valueAsString);
                    }
                }
            }
        }
        return value;
    }
    public bool CanExecute(object? parameter)
    {
        bool res = Field?.IsReady ?? false
        && (
            "Undo".Equals(parameter)
            || "Increase".Equals(parameter)
            || "Decrease".Equals(parameter)
            || ("Clear".Equals(parameter) && !Field.IsClean && !Field.IsReadonly)
        );
        return res;
    }
    public void Execute(object? parameter)
    {
        if(
            Field?.IsReady ?? false
            && (
                "Undo".Equals(parameter)
                || "Increase".Equals(parameter)
                || "Decrease".Equals(parameter)
                || ("Clear".Equals(parameter) && !Field.IsClean && !Field.IsReadonly)
            )
        )
        {
            if("Undo".Equals(parameter))
            {
                //TODO Execute
            }
            else if ("Clear".Equals(parameter))
            {
                _badFormat = null;
                Field.Clear();
            }
            else if ("Increase".Equals(parameter) || "Decrease".Equals(parameter))
            {
                if ("Increase".Equals(parameter))
                {
                    if(_initialHeight <= 0.1) 
                    {
                        _initialHeight = TextBox.ActualHeight;
                    }
                    TextBox.Height = TextBox.ActualHeight + TextBox.FontSize * ChangeHeight;
                    if(TextBox.Height >= Buttons.ActualWidth && Buttons.Orientation is not Orientation.Vertical)
                    {
                        Buttons.Orientation = Orientation.Vertical;
                    }
                }
                else if ("Decrease".Equals(parameter))
                {
                    if(_initialHeight > 0.1)
                    {
                        if (TextBox.ActualHeight - _initialHeight >= TextBox.FontSize * ChangeHeight)
                        {
                            TextBox.Height = TextBox.ActualHeight - TextBox.FontSize * ChangeHeight;
                        }
                        else
                        {
                            TextBox.Height = _initialHeight;
                        }
                        if (TextBox.Height < Buttons.ActualHeight && Buttons.Orientation is not Orientation.Horizontal)
                        {
                            Buttons.Orientation = Orientation.Horizontal;
                        }
                    }
                }
            }
        }
    }
    public void OnFieldAssigned()
    {
        if(Field is { })
        {
            TextBox.DataContext = Field;
            UndoButton.Visibility = Field.EntityProperty?.Entity.State is EntityState.Unchanged || Field.EntityProperty?.Entity.State is EntityState.Modified
                ? Visibility.Visible : Visibility.Collapsed;
            IncreaseTextButton.Visibility = Field.Type == typeof(string) ? Visibility.Visible : Visibility.Collapsed;
            DecreaseTextButton.Visibility = Field.Type == typeof(string) ? Visibility.Visible : Visibility.Collapsed;
            TextBox.AcceptsReturn = Field.Type == typeof(string);
            TextBox.VerticalScrollBarVisibility = Field.Type == typeof(string) ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
        }
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == FieldProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.Field))
            {
                if (e.NewValue is IField newField)
                {
                    newField.Owner = this;
                }
            }
        }
        else if (e.Property == PropertyNameProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.PropertyName))
            {
                if (_waitingFor is IField.WaitingForFlags.None)
                {
                    Field = new Field { Target = Target, PropertyName = PropertyName, Owner = this };
                }
            }
        }
        else if (e.Property == TargetProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.Target))
            {
                if (_waitingFor is IField.WaitingForFlags.None)
                {
                    Field = new Field { Target = Target, PropertyName = PropertyName, Owner = this };
                }
            }
        }
        base.OnPropertyChanged(e);
    }
    private static string? ToString(object value, Type type)
    {
        if (value is { })
        {
            return value.ToString();
        }
        return string.Empty;
    }
    private static bool TryParse(string valueAsString, Type type, out object? res)
    {
        if (type == typeof(DateOnly))
        {
            if (DateOnly.TryParse(valueAsString, out DateOnly don))
            {
                res = don;
            }
            else
            {
                res = null;
            }
        }
        else if (type == typeof(TimeOnly))
        {
            if (TimeOnly.TryParse(valueAsString, CultureInfo.CurrentCulture.DateTimeFormat, out TimeOnly ton))
            {
                res = ton;
            }
            else
            {
                res = null;
            }
        }
        else if (type == typeof(TimeSpan))
        {
            if (TimeSpan.TryParse(valueAsString, out TimeSpan ts))
            {
                res = ts;
            }
            else
            {
                res = null;
            }
        }
        else
        {
            res = System.Convert.ChangeType(valueAsString, type);
        }
        if (res?.ToString() != valueAsString)
        {
            Console.WriteLine($"{res?.ToString()} != {valueAsString}");
            return false;
        }
        return true;
    }
    private object? BadFormat(string s)
    {
        _badFormat = s;
        Console.WriteLine($"_badFormat: {_badFormat}");
        return _value;
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
        if(sender is TextBox)
        {
            if (e.Key is Key.Insert)
            {
                IsInsertMode = !IsInsertMode;
                if (Editor is { })
                {
                    Editor.CurrentInput = this;
                }
            }
        }
    }
    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if(sender is TextBox && Editor is { })
        {
            Editor.CurrentInput = this;
        }
    }
    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox && Editor is { })
        {
            Editor.CurrentInput = null;
        }
    }

    private static IEnumerable<DependencyObject> GetAncestors(DependencyObject obj)
    {
        for (var cur = obj; cur is not null; cur = VisualTreeHelper.GetParent(cur))
        {
            yield return cur;
        }
    }
    private static bool IsNumeric(object value)
    {
        return (value is Byte ||
                value is Int16 ||
                value is Int32 ||
                value is Int64 ||
                value is SByte ||
                value is UInt16 ||
                value is UInt32 ||
                value is UInt64 ||
                value is BigInteger ||
                value is Decimal ||
                value is Double ||
                value is Single);
    }
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(sender is TextBox tb)
        {
            if(_expectedCaretIndex != -1 && _expectedCaretIndex != tb.CaretIndex)
            {
                tb.CaretIndex = _expectedCaretIndex;
            }
            _expectedCaretIndex = -1;
        }
    }
    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is TextBox tb)
        {
            if (!string.IsNullOrEmpty(e.TextComposition.Text))
            {
                _expectedCaretIndex = tb.CaretIndex + e.TextComposition.Text.Length;
            }
        }
    }
}
