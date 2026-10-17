using BookTrack.Models;

namespace BookTrack.Services;

public sealed class ReadingListService
{
    private readonly List<ReadingListEntry> entries = [];

    public IReadOnlyList<ReadingListEntry> Entries => entries;

    public bool Contains(OpenLibraryBook book)
    {
        return entries.Any(entry => entry.Book.Key == book.Key);
    }

    public void Add(OpenLibraryBook book)
    {
        if (!Contains(book))
        {
            entries.Add(new ReadingListEntry(book));
        }
    }

    public void Remove(ReadingListEntry entry)
    {
        entries.Remove(entry);
    }
}
