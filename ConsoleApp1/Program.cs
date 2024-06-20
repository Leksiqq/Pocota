using ContosoPizza.Client;
using Net.Leksi.Pocota.Client;
using System.ComponentModel;
using System.Windows;

FindPizzasEnvelope findPizzas = new();
Queue<IField> fields = [];
for(int i = 0; i < 10; i++)
{
    IField field = new Field { Target = findPizzas, PropertyName = "stage" };
    Console.WriteLine($"Field: {field.GetHashCode()}");
    field.PropertyChanged += Field_PropertyChanged;
    fields.Enqueue( field );
}
for(int i = 1; fields.Count > 0; i++)
{
    GC.Collect();
    findPizzas.stage = i;
    if(i % 3 == 0)
    {
        Console.WriteLine($"removed: {fields.Dequeue().GetHashCode()}");
    }
}

void Field_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
{
    Console.WriteLine($"{sender!.GetHashCode()}: {(sender as IField)!.Value}");
}