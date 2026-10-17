using System.Text.Json.Serialization;

namespace BookTrack.Models;

public sealed class OpenLibraryBook
{
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("author_name")]
    public List<string> Authors { get; init; } = [];

    [JsonPropertyName("cover_i")]
    public int? CoverId { get; init; }

    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishYear { get; init; }
}
