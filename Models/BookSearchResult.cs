namespace BookTrack.Models;

public sealed record BookSearchResult(
    string? Title,
    IReadOnlyList<string> Authors,
    string? CoverUrl,
    OpenLibraryBook? OpenLibraryBook = null,
    BigBookBook? BigBookBook = null);
