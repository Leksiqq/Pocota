using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public partial class ObjectEditor : UserControl
{
    public ObservableCollection<Property> Properties { get; private init; } = [];
    public CollectionViewSource PropertiesViewSource { get; private init; } = new();
    public ObjectEditor()
    {
        PropertiesViewSource.Source = Properties;
        InitializeComponent();
    }
    private void CalcColumnsWidth(double width)
    {
        PropertyValueColumn.Width = width * 0.8 - PropertyNameColumn.ActualWidth;
    }
    private void PropertiesView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            CalcColumnsWidth(PropertiesView.ActualWidth);
        }
    }
}
