using System.Text.Json;
using System.Net.Http.Json;
using BookTrack.Models;

namespace BookTrack.Services;

public sealed class OpenLibraryService(HttpClient httpClient)
{
    public async Task<List<OpenLibraryBook>> SearchBooksAsync(string query, string searchField)
    {
        var parameter = searchField == "author" ? "author" : "title";
        var encodedQuery = Uri.EscapeDataString(query);
        var response = await httpClient.GetFromJsonAsync<OpenLibrarySearchResponse>(
            $"search.json?{parameter}={encodedQuery}&limit=5&fields=key,title,author_name,cover_i,first_publish_year");

        return response?.Books ?? [];
    }

    public async Task<OpenLibraryWork> GetWorkAsync(string workId)
    {
        var encodedWorkId = Uri.EscapeDataString(workId);
        var json = await httpClient.GetStringAsync($"works/{encodedWorkId}.json");
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var covers = ReadIntegerArray(root, "covers");

        return new OpenLibraryWork(
            ReadString(root, "title"),
            ReadDescription(root),
            ReadStringArray(root, "subjects"),
            covers.Count > 0 ? covers[0] : null);
    }

    private static string? ReadString(JsonElement root, string propertyName)
    {
        return root.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static string? ReadDescription(JsonElement root)
    {
        if (!root.TryGetProperty("description", out var value))
        {
            return null;
        }

        // Open Library descriptions can be text or an object containing a value.
        if (value.ValueKind == JsonValueKind.String)
        {
            return value.GetString();
        }

        return value.ValueKind == JsonValueKind.Object && value.TryGetProperty("value", out var description) &&
               description.ValueKind == JsonValueKind.String
            ? description.GetString()
            : null;
    }

    private static List<string> ReadStringArray(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return value.EnumerateArray()
            .Where(item => item.ValueKind == JsonValueKind.String)
            .Select(item => item.GetString()!)
            .ToList();
    }

    private static List<int> ReadIntegerArray(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return value.EnumerateArray()
            .Where(item => item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out _))
            .Select(item => item.GetInt32())
            .ToList();
    }
}
