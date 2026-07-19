using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SanskritQuest.Common.Http;

public class CommonHttpClient : ICommonHttpClient
{
    public HttpClient Client { get; }

    public CommonHttpClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public void AddHeader(string name, string value)
    {
        Client.DefaultRequestHeaders.Add(name, value);
    }

    public void RemoveHeader(string name)
    {
        Client.DefaultRequestHeaders.Remove(name);
    }

    public void SetBearerToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string requestUri, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        configureHeaders?.Invoke(request.Headers);
        using var response = await Client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
    {
        using var response = await PostAsync(requestUri, content, configureHeaders, jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(jsonOptions, cancellationToken);
    }

    public Task<HttpResponseMessage> PostAsync<TRequest>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(content, options: jsonOptions)
        };
        configureHeaders?.Invoke(request.Headers);
        return Client.SendAsync(request, cancellationToken);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
    {
        using var response = await PutAsync(requestUri, content, configureHeaders, jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(jsonOptions, cancellationToken);
    }

    public Task<HttpResponseMessage> PutAsync<TRequest>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(content, options: jsonOptions)
        };
        configureHeaders?.Invoke(request.Headers);
        return Client.SendAsync(request, cancellationToken);
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
    {
        using var response = await PatchAsync(requestUri, content, configureHeaders, jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(jsonOptions, cancellationToken);
    }

    public Task<HttpResponseMessage> PatchAsync<TRequest>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
        {
            Content = JsonContent.Create(content, options: jsonOptions)
        };
        configureHeaders?.Invoke(request.Headers);
        return Client.SendAsync(request, cancellationToken);
    }

    public async Task<TResponse?> DeleteAsync<TResponse>(string requestUri, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default)
    {
        using var response = await DeleteAsync(requestUri, configureHeaders, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(jsonOptions, cancellationToken);
    }

    public Task<HttpResponseMessage> DeleteAsync(string requestUri, Action<HttpRequestHeaders>? configureHeaders = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        configureHeaders?.Invoke(request.Headers);
        return Client.SendAsync(request, cancellationToken);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return Client.SendAsync(request, cancellationToken);
    }
}
