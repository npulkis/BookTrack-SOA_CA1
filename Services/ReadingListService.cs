using System.Text.Json;
using System.Text.Json.Serialization;
using BookTrack.Models;

namespace BookTrack.Services;

public sealed class ReadingListService
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string filePath;

    public ReadingListService(IWebHostEnvironment environment)
    {
        filePath = Path.Combine(environment.ContentRootPath, "App_Data", "reading-list.json");
    }

    public IReadOnlyList<ReadingListEntry> Entries { get; private set; } = [];

    public async Task LoadAsync()
    {
        await FileLock.WaitAsync();
        try
        {
            Entries = await ReadEntriesAsync();
            if (!File.Exists(filePath))
            {
                await SaveEntriesAsync(Entries);
            }
        }
        finally
        {
            FileLock.Release();
        }
    }

    public bool Contains(OpenLibraryBook book)
    {
        return Entries.Any(entry => entry.Book.Key == book.Key);
    }

    public Task AddAsync(OpenLibraryBook book)
    {
        return UpdateAsync(entries =>
        {
            if (!entries.Any(entry => entry.Book.Key == book.Key))
            {
                entries.Add(new ReadingListEntry(book));
            }
        });
    }

    public Task RemoveAsync(ReadingListEntry entry)
    {
        return UpdateAsync(entries => entries.RemoveAll(savedBook => savedBook.Book.Key == entry.Book.Key));
    }

    public Task UpdateStatusAsync(ReadingListEntry entry, ReadingStatus status)
    {
        return UpdateAsync(entries =>
        {
            var savedEntry = entries.FirstOrDefault(savedBook => savedBook.Book.Key == entry.Book.Key);
            if (savedEntry is not null)
            {
                savedEntry.Status = status;
            }
        });
    }

    private async Task UpdateAsync(Action<List<ReadingListEntry>> update)
    {
        await FileLock.WaitAsync();
        try
        {
            var entries = await ReadEntriesAsync();
            update(entries);
            await SaveEntriesAsync(entries);
            Entries = entries;
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<List<ReadingListEntry>> ReadEntriesAsync()
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<List<ReadingListEntry>>(stream, JsonOptions) ?? [];
    }

    private async Task SaveEntriesAsync(IReadOnlyList<ReadingListEntry> entries)
    {
        var directory = Path.GetDirectoryName(filePath)!;
        Directory.CreateDirectory(directory);

        var temporaryFilePath = $"{filePath}.tmp";
        await using (var stream = File.Create(temporaryFilePath))
        {
            await JsonSerializer.SerializeAsync(stream, entries, JsonOptions);
        }

        File.Move(temporaryFilePath, filePath, overwrite: true);
    }
}
