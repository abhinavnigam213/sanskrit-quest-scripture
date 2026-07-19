using System;
using System.Net.Http;

namespace SanskritQuest.Common.Http;

public class CommonHttpClientFactory : ICommonHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CommonHttpClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public HttpClient CreateClient(string name)
    {
        return _httpClientFactory.CreateClient(name);
    }

    public ICommonHttpClient CreateCommonClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);
        return new CommonHttpClient(client);
    }
}
