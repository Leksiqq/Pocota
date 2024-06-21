using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class FieldOwnerCore(IFieldOwner owner, DependencyProperty field, DependencyProperty target, DependencyProperty propertyName)
{
    private enum WaitingFor
    {
        None,
        PropertyName,
        Target,
        Field,
        Any,
    }
    private WaitingFor _expected = WaitingFor.Any;
    public void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == field)
        {
            if (CanProcessProperty(WaitingFor.Field))
            {
                if (e.NewValue is Field newField)
                {
                    newField.Owner = owner;
                }
            }
        }
        else if (e.Property == propertyName)
        {
            if (CanProcessProperty(WaitingFor.PropertyName))
            {
                if (_expected is WaitingFor.None)
                {
                    owner.Field = new Field { Target = owner.Target, PropertyName = owner.PropertyName, Owner = owner };
                }
            }
        }
        else if (e.Property == target)
        {
            if (CanProcessProperty(WaitingFor.Target))
            {
                if (_expected is WaitingFor.None)
                {
                    owner.Field = new Field { Target = owner.Target, PropertyName = owner.PropertyName, Owner = owner };
                }
            }
        }
    }
    private bool CanProcessProperty(WaitingFor got)
    {
        if (_expected is not WaitingFor.Any && _expected != got)
        {
            return false;
        }
        if (got is WaitingFor.Field)
        {
            _expected = WaitingFor.None;
        }
        else if (got is WaitingFor.PropertyName)
        {
            _expected = WaitingFor.Target;
        }
        else if (got is WaitingFor.Target)
        {
            _expected = WaitingFor.PropertyName;
        }
        return true;
    }
}
