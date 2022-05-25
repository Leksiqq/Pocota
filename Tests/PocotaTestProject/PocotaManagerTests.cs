using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Net.Leksi.Pocota.Core;
using System.Diagnostics;

namespace PocotaTestProject;

public class PocotaManagerTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.AutoFlush = true;
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestThrowIfConfigured()
    {
        IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddPocotaCore(services =>
        {
            services.AddTransient<Poco1_1, Poco1>();
            services.AddTransient<Poco1_2, Poco1>();
            services.AddTransient<Poco1_3, Poco1>();
            services.AddTransient<Poco2_1, Poco2>();
            services.AddTransient<Poco2_2, Poco2>();
            services.AddTransient<Poco2_3, Poco2>();
            services.AddKeyMapping<Poco2, Poco1>();
            services.AddKeyMapping<Poco1>(new string[] { "ID" });
        })).Build();
        PocotaManager pm = host.Services.GetRequiredService<PocotaManager>();
        var ex = Assert.Catch<InvalidOperationException>(() =>
        {
            pm.AddTransient<Poco3_1, Poco3>();
        });
        Assert.That(ex.Message, Is.EqualTo($"{typeof(PocotaManager)} is already configured"));
        ex = Assert.Catch<InvalidOperationException>(() =>
        {
            pm.AddKeyMapping<Poco3, Poco2>();
        });
        Assert.That(ex.Message, Is.EqualTo($"{typeof(PocotaManager)} is already configured"));
        ex = Assert.Catch<InvalidOperationException>(() =>
        {
            pm.AddKeyMapping<Poco1>(new string[] { "ID" });
        });
        Assert.That(ex.Message, Is.EqualTo($"{typeof(PocotaManager)} is already configured"));
    }

    [Test]
    public void TestThrowIfNotTransient()
    {
        IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddPocotaCore(services =>
        {
            var ex = Assert.Catch<InvalidOperationException>(() =>
            {
                services.AddScoped<Poco1_1, Poco1>();
            });
            Assert.That(ex.Message,
                Is.EqualTo(
                    $"{typeof(Poco1_1)} must be added as {nameof(ServiceLifetime.Transient)}, but is added as {nameof(ServiceLifetime.Scoped)}"));
            ex = Assert.Catch<InvalidOperationException>(() =>
            {
                services.AddSingleton<Poco2_2, Poco2>();
            });
            Assert.That(ex.Message,
                Is.EqualTo(
                    $"{typeof(Poco2_2)} must be added as {nameof(ServiceLifetime.Transient)}, but is added as {nameof(ServiceLifetime.Singleton)}"));
        })).Build();
    }

    [Test]
    public void TestThrowIfNotClass()
    {
        IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddPocotaCore(services =>
        {
            var ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping(typeof(int), typeof(Poco1));
            });
            Assert.That(ex.Message, Is.EqualTo($"targetType must be a class"));
            ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping(typeof(int), new string[] { "ID" });
            });
            Assert.That(ex.Message, Is.EqualTo($"targetType must be a class"));
            ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping<Poco1>(typeof(int));
            });
            Assert.That(ex.Message, Is.EqualTo($"example must be a class"));
            ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping(typeof(Poco1), typeof(int));
            });
            Assert.That(ex.Message, Is.EqualTo($"example must be a class"));
            ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping<Poco1_1>(typeof(Poco1));
            });
            Assert.That(ex.Message, Is.EqualTo($"targetType must be a class"));
            ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping(typeof(Poco1_1), typeof(Poco1));
            });
            Assert.That(ex.Message, Is.EqualTo($"targetType must be a class"));
            ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping(typeof(Poco2), typeof(Poco1_1));
            });
            Assert.That(ex.Message, Is.EqualTo($"example must be a class"));
            ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping<Poco2, Poco1_1>();
            });
            Assert.That(ex.Message, Is.EqualTo($"example must be a class"));
            ex = Assert.Catch<ArgumentException>(() =>
            {
                services.AddKeyMapping<Poco2>(typeof(Poco1_1));
            });
            Assert.That(ex.Message, Is.EqualTo($"example must be a class"));
        })).Build();
    }

    [Test]
    public void TestThrowIfAlreadyMapped()
    {
        IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddPocotaCore(services =>
        {
            services.AddTransient<Poco1_1, Poco1>();
            services.AddTransient<Poco1_2, Poco1>();
            services.AddTransient<Poco1_3, Poco1>();
            services.AddTransient<Poco2_1, Poco2>();
            services.AddTransient<Poco2_2, Poco2>();
            services.AddTransient<Poco2_3, Poco2>();
            services.AddTransient<Poco3_1, Poco3>();
            services.AddTransient<Poco3_2, Poco3>();
            services.AddTransient<Poco3_3, Poco3>();
            services.AddKeyMapping<Poco1>(new string[] { "ID" });
            var ex = Assert.Catch<InvalidOperationException>(() =>
            {
                services.AddKeyMapping<Poco1>(new string[] { "ID" });
            });
            Assert.That(ex.Message, Is.EqualTo($"Key for {typeof(Poco1)} is already mapped"));
            ex = Assert.Catch<InvalidOperationException>(() =>
            {
                services.AddKeyMapping<Poco1, Poco2>();
            });
            Assert.That(ex.Message, Is.EqualTo($"Key for {typeof(Poco1)} is already mapped"));
            ex = Assert.Catch<InvalidOperationException>(() =>
            {
                services.AddKeyMapping<Poco1>(typeof(Poco2));
            });
            Assert.That(ex.Message, Is.EqualTo($"Key for {typeof(Poco1)} is already mapped"));
            ex = Assert.Catch<InvalidOperationException>(() =>
            {
                services.AddKeyMapping(typeof(Poco1), typeof(Poco2));
            });
            Assert.That(ex.Message, Is.EqualTo($"Key for {typeof(Poco1)} is already mapped"));
        })).Build();
    }

    [Test]
    public void TestThrowIfExampleLoopDetected()
    {
        var ex = Assert.Catch<Exception>(() =>
        {
            IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddPocotaCore(services =>
            {
                services.AddTransient<Poco1_1, Poco1>();
                services.AddTransient<Poco1_2, Poco1>();
                services.AddTransient<Poco1_3, Poco1>();
                services.AddTransient<Poco2_1, Poco2>();
                services.AddTransient<Poco2_2, Poco2>();
                services.AddTransient<Poco2_3, Poco2>();
                services.AddTransient<Poco3_1, Poco3>();
                services.AddTransient<Poco3_2, Poco3>();
                services.AddTransient<Poco3_3, Poco3>();
                services.AddKeyMapping<Poco2, Poco3>();
                services.AddKeyMapping<Poco3, Poco2>();
            })).Build();
        });
        Assert.That(ex.Message, Is.EqualTo($"Example loop detected: {typeof(Poco2)}"));
    }

    [Test]
    public void TestThrowIfKeyNotMapped()
    {
        var ex = Assert.Catch<Exception>(() =>
        {
            IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddPocotaCore(services =>
            {
                services.AddTransient<Poco1_1, Poco1>();
                services.AddTransient<Poco1_2, Poco1>();
                services.AddTransient<Poco1_3, Poco1>();
                services.AddTransient<Poco2_1, Poco2>();
                services.AddTransient<Poco2_2, Poco2>();
                services.AddTransient<Poco2_3, Poco2>();
                services.AddKeyMapping<Poco1, Poco3>();
                services.AddKeyMapping<Poco2, Poco3>();
            })).Build();
        });
        Assert.That(ex.Message, Is.EqualTo($"Keys not mapped for: {typeof(Poco1)}, {typeof(Poco2)}, {typeof(Poco3)}"));
    }

    [Test]
    public void TestThrowIfKeyMappedForNotRegistered()
    {
        var ex = Assert.Catch<Exception>(() =>
        {
            IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddPocotaCore(services =>
            {
                services.AddTransient<Poco1_1, Poco1>();
                services.AddTransient<Poco1_2, Poco1>();
                services.AddTransient<Poco1_3, Poco1>();
                services.AddTransient<Poco2_1, Poco2>();
                services.AddTransient<Poco2_2, Poco2>();
                services.AddTransient<Poco2_3, Poco2>();
                services.AddKeyMapping<Poco3>(new[] { "ID" });
            })).Build();
        });
        Assert.That(ex.Message, Is.EqualTo($"Keys are mapped for not registered types: {typeof(Poco3)}"));
    }

    [Test]
    public void TestKeyRing()
    {
        IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddPocotaCore(services =>
        {
            services.AddTransient<Poco1_1, Poco1>();
            services.AddTransient<Poco1_2, Poco1>();
            services.AddTransient<Poco1_3, Poco1>();
            services.AddTransient<Poco2_1, Poco2>();
            services.AddTransient<Poco2_2, Poco2>();
            services.AddTransient<Poco2_3, Poco2>();
            services.AddTransient<Poco3_1, Poco3>();
            services.AddTransient<Poco3_2, Poco3>();
            services.AddTransient<Poco3_3, Poco3>();
            services.AddKeyMapping<Poco1>(new[] { "ID" });
            services.AddKeyMapping<Poco2>(new[] { "ID1", "ID2" });
            services.AddKeyMapping<Poco3>(new[] { "ID1", "ID2", "ID3" });
        })).Build();

        PocotaManager pm = host.Services.GetRequiredService<PocotaManager>();

        Poco1_1 poco1_1 = pm.GetRequiredService<Poco1_1>();
        KeyRing? keyRing = pm.GetKeyRing(poco1_1);
        Assert.That(keyRing, Is.Not.Null);
        Assert.That(keyRing.Key, Is.Not.Null);
        Assert.That(keyRing.Key.Length, Is.EqualTo(1));
        keyRing["ID"] = 123;
        Assert.Catch<IndexOutOfRangeException>(() =>
        {
            keyRing["ID1"] = 124;
        });
        keyRing = pm.GetKeyRing(poco1_1);
        Assert.That(keyRing, Is.Not.Null);
        Assert.That(keyRing.Key, Is.Not.Null);
        Assert.That(keyRing.Key.Length, Is.EqualTo(1));
        Assert.That(keyRing["ID"], Is.EqualTo(123));
        Poco2_2 poco2_2 = pm.GetRequiredService<Poco2_2>();
        keyRing = pm.GetKeyRing(poco2_2);
        Assert.That(keyRing, Is.Not.Null);
        Assert.That(keyRing.Key, Is.Not.Null);
        Assert.That(keyRing.Key.Length, Is.EqualTo(2));
        keyRing["ID1"] = 124;
        keyRing["ID2"] = "test";
        Assert.Catch<IndexOutOfRangeException>(() =>
        {
            keyRing["ID3"] = "out";
        });
        keyRing = pm.GetKeyRing(poco2_2);
        Assert.That(keyRing, Is.Not.Null);
        Assert.That(keyRing.Key, Is.Not.Null);
        Assert.That(keyRing.Key.Length, Is.EqualTo(2));
        Assert.That(keyRing["ID1"], Is.EqualTo(124));
        Assert.That(keyRing["ID2"], Is.EqualTo("test"));
        Poco3_3 poco3_3 = pm.GetRequiredService<Poco3_3>();
        keyRing = pm.GetKeyRing(poco3_3);
        Assert.That(keyRing, Is.Not.Null);
        Assert.That(keyRing.Key, Is.Not.Null);
        Assert.That(keyRing.Key.Length, Is.EqualTo(3));
        keyRing["ID1"] = 125;
        keyRing["ID2"] = "test1";
        keyRing["ID3"] = "test2";
        Assert.Catch<IndexOutOfRangeException>(() =>
        {
            keyRing["ID4"] = "out1";
        });
        keyRing = pm.GetKeyRing(poco3_3);
        Assert.That(keyRing, Is.Not.Null);
        Assert.That(keyRing.Key, Is.Not.Null);
        Assert.That(keyRing.Key.Length, Is.EqualTo(3));
        Assert.That(keyRing["ID1"], Is.EqualTo(125));
        Assert.That(keyRing["ID2"], Is.EqualTo("test1"));
        Assert.That(keyRing["ID3"], Is.EqualTo("test2"));


        poco1_1 = null;
        Assert.Catch<ArgumentNullException>(() =>
        {
            keyRing = pm.GetKeyRing(poco1_1);
        });
        keyRing = pm.GetKeyRing(new object());
        Assert.That(keyRing, Is.Null);
        keyRing = pm.GetKeyRing(1);
        Assert.That(keyRing, Is.Null);
    }

    public interface Poco1_1 { }
    public interface Poco1_2 { }
    public interface Poco1_3 { }
    public interface Poco2_1 { }
    public interface Poco2_2 { }
    public interface Poco2_3 { }
    public interface Poco3_1 { }
    public interface Poco3_2 { }
    public interface Poco3_3 { }
    public class Poco1 : Poco1_1, Poco1_2, Poco1_3 { }
    public class Poco2 : Poco2_1, Poco2_2, Poco2_3 { }
    public class Poco3 : Poco3_1, Poco3_2, Poco3_3 { }

}