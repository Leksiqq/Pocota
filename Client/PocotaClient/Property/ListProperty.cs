using Net.Leksi.Pocota.Contract;
using System.Collections;

namespace Net.Leksi.Pocota.Client;

public class ListProperty : Property
{
    private readonly Property _source;
    public override object? Value 
    { 
        get => _source.Value; 
        set => _source.Value = value; 
    }
    public override AccessKind Access
    {
        get => _source.Access;
        internal set => _source.Access = value;
    }
    public override PropertyState State
    {
        get => _source.State;
        internal set => _source.State = value;
    }
    public override bool IsReadonly => _source.IsReadonly;
    public override bool IsNullable => _source.IsNullable;
    public override object? Declarator => _source.Declarator;
    public int Count
    {
        get
        {
            return (Value as IList)?.Count ?? 0;
        }
    }
    internal ListProperty(Property source) : base(source.Name, source.Type)
    {
        _source = source;
        _source.PropertyChanged += _source_PropertyChanged;
    }

    private void _source_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        NotifyPropertyChanged();
    }
}
