using System.Net.Http;

namespace SanskritQuest.Common.Http;

public interface ICommonHttpClientFactory
{
    HttpClient CreateClient(string name);
    ICommonHttpClient CreateCommonClient(string name);
}
