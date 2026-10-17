using System.Text.Json.Serialization;

namespace BookTrack.Models;

public sealed class OpenLibrarySearchResponse
{
    [JsonPropertyName("docs")]
    public List<OpenLibraryBook> Books { get; init; } = [];
}
