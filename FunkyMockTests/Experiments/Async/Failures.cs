namespace FunkyMockTests.Experiments.Async;

using Xunit;
using System.Diagnostics;
using System.Text.Json;

public interface IHttpService
{
    Task<HttpResult<HttpBinResponse>> ReadValues(string path);
}

public class Failures
{
    [Fact]
    public async Task fake_http_call_that_works()
    {
        var service = new FakeService {
            OnReadValues = _ => Task.FromResult(new HttpResult<HttpBinResponse>(0, new HttpBinResponse{Url = "https://httpbin.org/get"}, TimeSpan.Zero)),
        };

        var actual = await service.ReadValues("");

        Assert.Equal("https://httpbin.org/get", actual.Response?.Url ?? "-missing-");
    }
    [Fact]
    public async Task real_http_call_that_times_out()
    {
         var c = new HttpClient(new HttpClientHandler());
         c.Timeout = TimeSpan.FromMilliseconds(100);
        var service = new HttpService(c);

        await Assert.ThrowsAsync<TaskCanceledException>(async () => await service.ReadValues(""));
    }
    [Fact]
    public async Task using_Task_FromCanceled()
    {
        var service = new FakeService {
            OnReadValues = _ => Task.FromCanceled<HttpResult<HttpBinResponse>>(new CancellationToken(true)),
        };

        await Assert.ThrowsAsync<TaskCanceledException>(async () => await service.ReadValues(""));
    }
    [Fact]
    public async Task throwing_TaskCanceledException()
    {
        var service = new FakeService {
            OnReadValues = _ => throw new TaskCanceledException(),
        };

        await Assert.ThrowsAsync<TaskCanceledException>(async () => await service.ReadValues(""));
    }

}

public class HttpService : IHttpService
{
    private readonly HttpClient _client;
    public HttpService(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResult<HttpBinResponse>> ReadValues(string path)
    {
        var t0 = Stopwatch.StartNew();
        var req = new HttpRequestMessage(HttpMethod.Get, "https://httpbin.org/get") {
            Headers = { {"accept","application/json" } }
        };
        var res = await _client.SendAsync(req);
        if (!res.IsSuccessStatusCode)
        {
            return await Task.FromResult(new HttpResult<HttpBinResponse>((int)res.StatusCode, null, t0.Elapsed));
        }

        var body = await res.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var value = JsonSerializer.Deserialize<HttpBinResponse>(body,options);
        return new HttpResult<HttpBinResponse>((int)res.StatusCode, value, t0.Elapsed);
    }
}

public record class HttpResult<T>(int StatusCode, T? Response, TimeSpan Duration) where T : class;

public class HttpBinResponse
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public Dictionary<string,string>? Headers { get; set; }
    public string? Origin { get; set; }
    public string? Url { get; set; }
}
