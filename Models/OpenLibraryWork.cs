namespace BookTrack.Models;

public sealed record OpenLibraryWork(
    string? Title,
    string? Description,
    List<string> Subjects,
    int? CoverId);
