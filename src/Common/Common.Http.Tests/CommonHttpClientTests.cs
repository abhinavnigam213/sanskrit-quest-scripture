using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace SanskritQuest.Common.Http.Tests;

public class CommonHttpClientTests
{
    private class TestResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    [Fact]
    public async Task GetAsync_CallsUnderlyingClient_AndDeserializesJson()
    {
        // Arrange
        var responseObj = new TestResponse { Message = "Success" };
        var json = JsonSerializer.Serialize(responseObj);
        
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            });

        using var client = new HttpClient(handlerMock.Object);
        var commonClient = new CommonHttpClient(client);

        // Act
        var result = await commonClient.GetAsync<TestResponse>("https://api.example.com/test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
    }

    [Fact]
    public async Task Requests_SupportDynamicHeaderConfiguration()
    {
        // Arrange
        var responseObj = new TestResponse { Message = "Ok" };
        var json = JsonSerializer.Serialize(responseObj);
        
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Headers.Contains("X-Custom-Header") && 
                    req.Headers.Authorization != null && 
                    req.Headers.Authorization.Parameter == "my-token"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            });

        using var client = new HttpClient(handlerMock.Object);
        var commonClient = new CommonHttpClient(client);

        // Act
        var result = await commonClient.GetAsync<TestResponse>(
            "https://api.example.com/test",
            configureHeaders: headers =>
            {
                headers.Add("X-Custom-Header", "value");
                headers.Authorization = new AuthenticationHeaderValue("Bearer", "my-token");
            }
        );

        // Assert
        Assert.NotNull(result);
    }
}
