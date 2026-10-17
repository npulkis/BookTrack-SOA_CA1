namespace BookTrack.Models;

public sealed class ReadingListEntry(OpenLibraryBook book)
{
    public OpenLibraryBook Book { get; } = book;

    public ReadingStatus Status { get; set; } = ReadingStatus.WantToRead;
}

public enum ReadingStatus
{
    WantToRead,
    Reading,
    Finished
}
