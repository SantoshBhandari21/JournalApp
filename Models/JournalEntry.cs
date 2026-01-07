using SQLite;

namespace JournalApp.Models;

public class JournalEntry
{
    [PrimaryKey]
    public DateTime EntryDate { get; set; }

    public string? Title { get; set; }
    public string? Content { get; set; }

    // Moods
    public string PrimaryMood { get; set; } = "";
    public string? SecondaryMood1 { get; set; }
    public string? SecondaryMood2 { get; set; }

    // Tags (comma separated)
    public string? Tags { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}