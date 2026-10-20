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

    public async Task<List<BigBookSimilarBook>> GetThemeRelatedBooksAsync(
        string? title,
        string? description,
        IReadOnlyCollection<string> subjects)
    {
        if (!HasApiKey)
        {
            return [];
        }

        var themes = subjects
            .Where(subject => !string.IsNullOrWhiteSpace(subject))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToList();
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) && themes.Count == 0)
        {
            return [];
        }

        var themeContext = themes.Count > 0
            ? string.Join(", ", themes)
            : description;
        var query = $"books about {themeContext}";

        var matches = await SearchBooksAsync(query);
        return matches
            .Where(book => !string.IsNullOrWhiteSpace(book.Title) && !IsSameBookTitle(book.Title, title))
            .DistinctBy(book => book.Title!.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(book => new BigBookSimilarBook { Id = book.Id, Title = book.Title, Image = book.Image })
            .Take(8)
            .ToList();
    }

    private static bool IsSameBookTitle(string recommendationTitle, string sourceTitle)
    {
        var candidate = recommendationTitle.Trim();
        var source = sourceTitle.Trim();

        if (string.Equals(candidate, source, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return candidate.StartsWith(source, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(candidate[source.Length..].Trim(), "A Novel", StringComparison.OrdinalIgnoreCase);
    }
}
