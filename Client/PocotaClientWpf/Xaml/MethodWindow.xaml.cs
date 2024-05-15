using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client
{
    /// <summary>
    /// Логика взаимодействия для MethodWindow.xaml
    /// </summary>
    public partial class MethodWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MethodInfo _method;
        private readonly IServiceProvider _services;
        private readonly Dictionary<string, WeakReference<EditObject>> _editWindows = [];
        public ObservableCollection<Property> Parameters { get; private init; } = [];
        public WindowsList Windows { get; private init; }
        public string MethodName => _method is { } ? $"{Util.BuildTypeName(_method.DeclaringType!)}.{_method.Name}" : string.Empty;
        public MethodWindow(MethodInfo method)
        {
            _method = method;
            _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
            Windows = _services.GetRequiredService<WindowsList>();
            InitializeComponent();
            Metrics.MethodInfo = _method;
            foreach(ParameterInfo parameter in _method.GetParameters())
            {
                if(parameter.ParameterType != typeof(CancellationToken) && parameter.Name != "target")
                {
                    Parameters.Add(new ParameterInfoProperty(parameter));
                }
            }
            CalcColumnsWidth(ParametersView.ActualWidth);
            Windows.Touch();
        }
        public EditObject Launch(string parameterName)
        {
            if(_editWindows.TryGetValue(parameterName, out WeakReference<EditObject>? wr))
            {
                if(
                    wr.TryGetTarget(out EditObject? eo) 
                    && Application.Current.Windows.OfType<EditObject>().Any(w => w == eo)
                )
                {
                    return eo;
                }
                _editWindows.Remove(parameterName);
            }
            EditObject result = new(MethodName, parameterName)
            {
                Launcher = this
            };
            _editWindows.Add(parameterName, new WeakReference<EditObject>(result));
            return result;
        }
        protected override void OnClosed(EventArgs e)
        {
            Windows.Touch();
            base.OnClosed(e);
        }
        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                CalcColumnsWidth(e.NewSize.Width);
            }
        }
        private void CalcColumnsWidth(double width)
        {
            ParameterValueColumn.Width = width * 0.89 - ParameterNameColumn.ActualWidth;
        }
        private void ListViewItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalcColumnsWidth(ParametersView.ActualWidth);
        }
    }
}
