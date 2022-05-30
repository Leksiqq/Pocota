using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Net.Leksi.Pocota.Core;
using PocotaTestProject.Model;
using System.Diagnostics;
using System.Reflection;

namespace PocotaTestProject;

public class ObjectCacheTest
{
    private IHost _host;
    private int _genId = 0;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _host = Config.Configure();
    }

    [Test]
    public void Test()
    {
        IShipCall shipCall = BuildObject<IShipCall>();
        TypesForest tf = _host.Services.GetRequiredService<TypesForest>();
        Container man = _host.Services.GetRequiredService<Container>();
        Trace.WriteLine(tf.TreeToString(shipCall));
        KeyRing key1 = man.GetKeyRing(shipCall)!;
        Assert.That(key1, Is.Not.Null);
        Assert.That(key1.IsAssigned);
        Assert.That(key1["ID_LINE"], Is.EqualTo("key1"));
        Assert.That(key1["ID_ROUTE"], Is.EqualTo(2));
        ObjectCache oc = _host.Services.GetRequiredService<ObjectCache>();
        Assert.That(oc.Add(typeof(IShipCall), shipCall));

        IShipCallAdditionalInfo shipCallAI = _host.Services.GetRequiredService<IShipCallAdditionalInfo>();
        key1 = man.GetKeyRing(shipCallAI)!;
        Assert.That(key1, Is.Not.Null);
        Assert.That(key1.IsAssigned, Is.False);
        key1["ID_LINE"] = "key1";
        key1["ID_ROUTE"] = 2;

        if(shipCallAI is ShipCall shipCall1)
        {
            shipCall1.AdditionalInfo = "AdditionalInfo";
            Assert.That(oc.Add(typeof(IShipCallAdditionalInfo), shipCallAI), Is.False);
            object? obj;
            Assert.That(oc.TryGet(typeof(IShipCallForListing), man.GetKeyRing(shipCallAI)!, out obj), Is.False);
            Assert.That(oc.TryGet(typeof(IShipCall), shipCallAI, out obj));
            Assert.That(obj, Is.EqualTo(shipCall));
            Trace.WriteLine(tf.TreeToString(obj));
            Trace.WriteLine(tf.TreeToString<IShipCallForListing>(obj));
        }
        else
        {
            Assert.Fail();
        }

    }

    private T BuildObject<T>() where T : class
    {
        Container manager = _host.Services.GetRequiredService<Container>();
        T obj = _host.Services.GetRequiredService<T>();
        Type actualType = manager.GetActualType(typeof(T))!;
        KeyRing keyRing = manager.GetKeyRing(obj)!;
        Dictionary<string, Type>? dictionary = manager.GetPrimaryKeyDefinition(actualType);
        foreach(KeyValuePair<string, Type> kd in dictionary)
        {
            if(kd.Value == typeof(int))
            {
                keyRing[kd.Key] = ++_genId;
            }
            else
            {
                keyRing[kd.Key] = $"key{++_genId}";
            }
        }
        foreach(PropertyInfo pi in typeof(T).GetProperties())
        {
            if(pi.PropertyType == typeof(int))
            {
                actualType.GetProperty(pi.Name).SetValue(obj, ++_genId);
            }
            else if (pi.PropertyType == typeof(string))
            {
                actualType.GetProperty(pi.Name).SetValue(obj, $"value{++_genId}");
            }
        }
        return obj;
    }
}
