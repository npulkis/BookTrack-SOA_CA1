using System.Text.Json.Serialization;

namespace BookTrack.Models;

public sealed class BigBookSearchResponse
{
    [JsonPropertyName("books")]
    public List<List<BigBookBook>> Books { get; init; } = [];
}

public sealed class BigBookBook
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("image")]
    public string? Image { get; init; }

    [JsonPropertyName("authors")]
    public List<BigBookAuthor> Authors { get; init; } = [];

    [JsonPropertyName("identifiers")]
    public BigBookIdentifiers? Identifiers { get; init; }
}

public sealed class BigBookAuthor
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

public sealed class BigBookIdentifiers
{
    [JsonPropertyName("open_library_id")]
    public string? OpenLibraryId { get; init; }
}
