using BookTrack.Models;

namespace BookTrack.Services;

public sealed class ReadingListService
{
    private readonly List<OpenLibraryBook> books = [];

    public IReadOnlyList<OpenLibraryBook> Books => books;

    public bool Contains(OpenLibraryBook book)
    {
        return books.Any(savedBook => savedBook.Key == book.Key);
    }

    public void Add(OpenLibraryBook book)
    {
        if (!Contains(book))
        {
            books.Add(book);
        }
    }
}
