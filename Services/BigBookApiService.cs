using System.Net.Http.Json;
using BookTrack.Models;

namespace BookTrack.Services;

public sealed class BigBookApiService(HttpClient httpClient, IConfiguration configuration)
{
    public bool HasApiKey => !string.IsNullOrWhiteSpace(configuration["BigBookApi:ApiKey"]);

    public async Task<List<BigBookBook>> SearchBooksAsync(string query)
    {
        var response = await httpClient.GetFromJsonAsync<BigBookSearchResponse>(
            $"search-books?query={Uri.EscapeDataString(query)}&number=10");

        return response?.Books.SelectMany(group => group).ToList() ?? [];
    }

    public async Task<BigBookBook?> GetBookAsync(int bookId)
    {
        return await httpClient.GetFromJsonAsync<BigBookBook>(bookId.ToString());
    }
}
