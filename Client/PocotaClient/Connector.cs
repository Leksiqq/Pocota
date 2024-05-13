using Net.Leksi.Pocota.Contract;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
namespace Net.Leksi.Pocota.Client;

public abstract class Connector
{
    private Uri? _baseAddress;
    private TimeSpan _timeout;
    private HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializationOptions = new();
    private readonly ExceptionJsonConverter _exceptionJsonConverter = new();
    protected readonly IServiceProvider _services;
    protected readonly JsonSerializerOptions _serializerOptions = new();
    public HttpStatusCode StatusCode { get; private set; }
    public Uri? BaseAddress
    {
        get => _baseAddress;
        set
        {
            if (_baseAddress != value)
            {
                _baseAddress = value;
                ReplaceHttpClient();
            }
        }
    }
    public TimeSpan Timeout
    {
        get => _timeout;
        set
        {
            if (_timeout != value)
            {
                _timeout = value;
                ReplaceHttpClient();
            }
        }
    }
    public Connector(IServiceProvider services)
    {
        _services = services;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add(PocotaHeader.WithFieldsAccess, string.Empty);
        _timeout = _httpClient.Timeout;
        _baseAddress = _httpClient.BaseAddress;
        _serializationOptions.Converters.Add(_exceptionJsonConverter);
    }
    public void AddConverter(JsonConverter converter)
    {
        _serializerOptions.Converters.Add(converter);
    }
    public async Task GetPocotaConfigAsync(string path, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new(HttpMethod.Get, path);
        _services.GetRequiredService<PocotaContext>().PocotaConfig = await GetResponseAsync<PocotaConfig>(request, _serializerOptions, cancellationToken);
    }
    public async Task GetResponseAsyncEnumerable<T>(
        ICollection<T>? target,
        HttpRequestMessage request,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken
    )
    {
        TieStream? stream = null;
        IAsyncEnumerator<T?> en;
        try
        {
            stream = await GetResponseStreamAsync<T>(request, cancellationToken);
            en = JsonSerializer.DeserializeAsyncEnumerable<T>(
                stream,
                jsonSerializerOptions,
                cancellationToken
            ).GetAsyncEnumerator(cancellationToken);
            if (target is { })
            {
                while (await en.MoveNextAsync())
                {
                    target.Add(en.Current!);
                }
            }
            else
            {
                while (await en.MoveNextAsync()) { }
            }
        }
        catch
        {
            if (stream is { })
            {
                new StreamReader(stream).ReadToEnd();
                if (stream.FindException())
                {
                    throw BuildRemoteException(stream);
                }
            }
            throw;
        }
    }
    public async ValueTask<T?> GetResponseAsync<T>(
        HttpRequestMessage request,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken
    )
    {
        TieStream? stream = null;
        try
        {
            stream = await GetResponseStreamAsync<T>(request, cancellationToken);
            T? result = await JsonSerializer.DeserializeAsync<T>(
                stream,
                jsonSerializerOptions,
                cancellationToken
            );
            return result;
        }
        catch
        {
            if (stream is { })
            {
                new StreamReader(stream).ReadToEnd();
                if (stream.FindException())
                {
                    throw BuildRemoteException(stream);
                }
            }
            throw;
        }
    }
    private void ReplaceHttpClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = _baseAddress,
            Timeout = _timeout
        };
        _httpClient.DefaultRequestHeaders.Add(PocotaHeader.WithFieldsAccess, string.Empty);
    }
    private async Task<TieStream> GetResponseStreamAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );
        StatusCode = response.StatusCode;
        IEnumerable<string>? value = response.Headers.Contains(PocotaHeader.ExceptionBoundary)
            ? response.Headers.GetValues(PocotaHeader.ExceptionBoundary)
            : null;

        return new TieStream(
            await response.Content!.ReadAsStreamAsync(cancellationToken),
            value?.FirstOrDefault()
        );
    }
    private PocotaRemoteException BuildRemoteException(TieStream stream)
    {
        PocotaRemoteException exception = new("The remote exception");
        _exceptionJsonConverter.Target = exception;
        JsonSerializer.Deserialize<Exception>(stream.ExceptionData, _serializationOptions);
        return exception;
    }

}
