using System.Text.Json;

namespace BlazorApp.Services;

// Shared logic: read the response, throw on failure, otherwise deserialize.
internal static class HttpHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ReadAsync<T>(HttpResponseMessage httpResponse)
    {
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<T>(response, Options)!;
    }
}
