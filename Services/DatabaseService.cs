using SQLite;
using JournalApp.Models;

namespace JournalApp.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "journal.db"
        );

        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<JournalEntry>().Wait();
    }

    public Task<int> SaveEntryAsync(JournalEntry entry)
    {
        return _db.InsertOrReplaceAsync(entry);
    }

    public Task<List<JournalEntry>> GetEntriesAsync()
    {
        return _db.Table<JournalEntry>().ToListAsync();
    }
    public Task<int> DeleteEntryAsync(DateTime entryDate)
    {
        return _db.DeleteAsync<JournalEntry>(entryDate);
    }

}