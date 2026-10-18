namespace BookTrack.Models;

public sealed class ReadingListEntry(OpenLibraryBook book)
{
    public OpenLibraryBook Book { get; } = book;

    public ReadingStatus Status { get; set; } = ReadingStatus.WantToRead;

    public DateOnly? FinishedDate { get; set; }

    public int? Rating { get; set; }

    public string? Notes { get; set; }
}

public enum ReadingStatus
{
    WantToRead,
    Reading,
    Finished
}
