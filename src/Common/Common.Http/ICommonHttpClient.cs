using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SanskritQuest.Common.Http;

public interface ICommonHttpClient
{
    HttpClient Client { get; }

    // Instance-level header helpers
    void AddHeader(string name, string value);
    void RemoveHeader(string name);
    void SetBearerToken(string token);

    // Dynamic per-request HTTP verbs
    Task<T?> GetAsync<T>(string requestUri, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    
    Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsync<TRequest>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    
    Task<TResponse?> PutAsync<TRequest, TResponse>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PutAsync<TRequest>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    
    Task<TResponse?> PatchAsync<TRequest, TResponse>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PatchAsync<TRequest>(string requestUri, TRequest content, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    
    Task<TResponse?> DeleteAsync<TResponse>(string requestUri, Action<HttpRequestHeaders>? configureHeaders = null, JsonSerializerOptions? jsonOptions = null, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> DeleteAsync(string requestUri, Action<HttpRequestHeaders>? configureHeaders = null, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}
