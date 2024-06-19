using System.Collections.ObjectModel;

namespace Net.Leksi.Pocota.Client;

public class MyObservableCollection<T>(EntityProperty entityProperty) : ObservableCollection<T>

{
    protected override void ClearItems()
    {
        if (!entityProperty.IsReadonly)
        {
            base.ClearItems();
        }
    }
    protected override void InsertItem(int index, T item)
    {
        if (!entityProperty.IsReadonly)
        {
            base.InsertItem(index, item);
        }
    }
    protected override void RemoveItem(int index)
    {
        if (!entityProperty.IsReadonly)
        {
            base.RemoveItem(index);
        }
    }
    protected override void MoveItem(int oldIndex, int newIndex)
    {
        if (!entityProperty.IsReadonly)
        {
            base.MoveItem(oldIndex, newIndex);
        }
    }
    protected override void SetItem(int index, T item)
    {
        if (!entityProperty.IsReadonly)
        {
            base.SetItem(index, item);
        }
    }
}
