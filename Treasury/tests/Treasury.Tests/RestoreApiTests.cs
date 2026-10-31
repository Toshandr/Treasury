using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Treasury.Api.Dtos;

namespace Treasury.Tests;

public class RestoreApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Url = "/api/constellations/restore";
    private readonly HttpClient _client;

    public RestoreApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Restore_WithMissingSymbols_ReturnsOk()
    {
        var response = await _client.PostAsJsonAsync(Url, new { reference = "DRAGON", damaged = "GOD" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RestoreResponse>();
        Assert.Equal("ANR", body!.MissingSymbols);
        Assert.False(body.IsComplete);
    }

    [Fact]
    public async Task Restore_WithAllSymbols_ReturnsComplete()
    {
        var response = await _client.PostAsJsonAsync(Url, new { reference = "DRAGON", damaged = "NOGARD" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RestoreResponse>();
        Assert.Equal("", body!.MissingSymbols);
        Assert.True(body.IsComplete);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"reference\":\"DRAGON\"}")]
    [InlineData("{\"damaged\":\"GOD\"}")]
    [InlineData("{\"reference\":\"\",\"damaged\":\"\"}")]
    [InlineData("{\"reference\":\"dragon\",\"damaged\":\"GOD\"}")]
    [InlineData("{\"reference\":\"DRAG1N\",\"damaged\":\"GD\"}")]
    [InlineData("{\"reference\":\"DRAGOON\",\"damaged\":\"GOD\"}")]
    [InlineData("{\"reference\":\"DRAGON\",\"damaged\":\"GOX\"}")]
    [InlineData("{\"reference\":\"DRAGON\",\"damaged\":\"GGO\"}")]
    [InlineData("{\"reference\":\"DRAGON\",\"damaged\":\"god\"}")]
    public async Task Restore_WithInvalidInput_ReturnsBadRequest(string json)
    {
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(Url, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
