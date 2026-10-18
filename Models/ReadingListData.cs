namespace BookTrack.Models;

public sealed class ReadingListData
{
    public List<ReadingListEntry> Entries { get; set; } = [];

    public int GoalYear { get; set; } = DateTime.Today.Year;

    public int AnnualGoal { get; set; } = 12;
}
