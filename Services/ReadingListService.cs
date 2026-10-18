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
    private ReadingListData data = new();

    public ReadingListService(IWebHostEnvironment environment)
    {
        filePath = Path.Combine(environment.ContentRootPath, "App_Data", "reading-list.json");
    }

    public IReadOnlyList<ReadingListEntry> Entries => data.Entries;
    public int GoalYear => data.GoalYear;
    public int AnnualGoal => data.AnnualGoal;

    public async Task LoadAsync()
    {
        await FileLock.WaitAsync();
        try
        {
            var (loadedData, needsSave) = await ReadDataAsync();
            data = loadedData;
            if (needsSave)
            {
                await SaveDataAsync(data);
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

    public int GetFinishedCount(int year)
    {
        return Entries.Count(entry => entry.Status == ReadingStatus.Finished && entry.FinishedDate?.Year == year);
    }

    public Task AddAsync(OpenLibraryBook book)
    {
        return UpdateAsync(savedData =>
        {
            if (!savedData.Entries.Any(entry => entry.Book.Key == book.Key))
            {
                savedData.Entries.Add(new ReadingListEntry(book));
            }
        });
    }

    public Task RemoveAsync(ReadingListEntry entry)
    {
        return UpdateAsync(savedData =>
            savedData.Entries.RemoveAll(savedBook => savedBook.Book.Key == entry.Book.Key));
    }

    public Task UpdateStatusAsync(ReadingListEntry entry, ReadingStatus status)
    {
        return UpdateAsync(savedData =>
        {
            var savedEntry = savedData.Entries.FirstOrDefault(savedBook => savedBook.Book.Key == entry.Book.Key);
            if (savedEntry is not null)
            {
                savedEntry.Status = status;
                savedEntry.FinishedDate = status == ReadingStatus.Finished
                    ? savedEntry.FinishedDate ?? DateOnly.FromDateTime(DateTime.Today)
                    : null;
            }
        });
    }

    public Task UpdateReviewAsync(ReadingListEntry entry, int? rating, string? notes)
    {
        if (rating is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between one and five stars.");
        }

        return UpdateAsync(savedData =>
        {
            var savedEntry = savedData.Entries.FirstOrDefault(savedBook => savedBook.Book.Key == entry.Book.Key);
            if (savedEntry is not null)
            {
                savedEntry.Rating = rating;
                savedEntry.Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            }
        });
    }

    public Task UpdateGoalAsync(int year, int bookCount)
    {
        if (year < 1 || bookCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(bookCount), "Year and book goal must be positive numbers.");
        }

        return UpdateAsync(savedData =>
        {
            savedData.GoalYear = year;
            savedData.AnnualGoal = bookCount;
        });
    }

    private async Task UpdateAsync(Action<ReadingListData> update)
    {
        await FileLock.WaitAsync();
        try
        {
            var (latestData, _) = await ReadDataAsync();
            update(latestData);
            await SaveDataAsync(latestData);
            data = latestData;
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<(ReadingListData Data, bool NeedsSave)> ReadDataAsync()
    {
        if (!File.Exists(filePath))
        {
            return (new ReadingListData(), true);
        }

        await using var stream = File.OpenRead(filePath);
        using var document = await JsonDocument.ParseAsync(stream);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            var oldEntries = document.RootElement.Deserialize<List<ReadingListEntry>>(JsonOptions) ?? [];
            return (new ReadingListData { Entries = oldEntries }, true);
        }

        return (document.RootElement.Deserialize<ReadingListData>(JsonOptions) ?? new ReadingListData(), false);
    }

    private async Task SaveDataAsync(ReadingListData savedData)
    {
        var directory = Path.GetDirectoryName(filePath)!;
        Directory.CreateDirectory(directory);

        var temporaryFilePath = $"{filePath}.tmp";
        await using (var stream = File.Create(temporaryFilePath))
        {
            await JsonSerializer.SerializeAsync(stream, savedData, JsonOptions);
        }

        File.Move(temporaryFilePath, filePath, overwrite: true);
    }
}
