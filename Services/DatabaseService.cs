using SQLite;
using JournalApp.Models;

namespace JournalApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db");
            _db = new SQLiteAsyncConnection(dbPath);

            _db.CreateTableAsync<JournalEntry>().Wait();
        }

        /* Read helpers used by dashboard, list page, calendar and editor pages */
        public Task<List<JournalEntry>> GetEntriesAsync()
        {
            return _db.Table<JournalEntry>()
                      .OrderByDescending(e => e.EntryDate)
                      .ToListAsync();
        }

        public Task<List<JournalEntry>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var s = startDate.Date;
            var e = endDate.Date;

            return _db.Table<JournalEntry>()
                      .Where(x => x.EntryDate >= s && x.EntryDate <= e)
                      .OrderBy(x => x.EntryDate)
                      .ToListAsync();
        }

        public Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
        {
            var d = date.Date;

            return _db.Table<JournalEntry>()
                      .FirstOrDefaultAsync(x => x.EntryDate == d);
        }

        public async Task<bool> EntryExistsAsync(DateTime date)
        {
            var d = date.Date;

            return await _db.Table<JournalEntry>()
                            .Where(x => x.EntryDate == d)
                            .CountAsync() > 0;
        }

        /* Save logic for one-entry-per-day: insert if missing, otherwise update that day's entry */
        public async Task SaveEntryAsync(JournalEntry entry)
        {
            var existing = await GetEntryByDateAsync(entry.EntryDate);

            if (existing is null)
            {
                entry.EntryDate = entry.EntryDate.Date;
                entry.CreatedAt = DateTime.Now;
                entry.UpdatedAt = DateTime.Now;
                await _db.InsertAsync(entry);
                return;
            }

            CopyEditableFields(existing, entry);
            existing.UpdatedAt = DateTime.Now;
            await _db.UpdateAsync(existing);
        }

        /* Explicit update used by the edit page */
        public async Task UpdateEntryAsync(JournalEntry entry)
        {
            var existing = await GetEntryByDateAsync(entry.EntryDate);
            if (existing is null) return;

            CopyEditableFields(existing, entry);
            existing.UpdatedAt = DateTime.Now;
            await _db.UpdateAsync(existing);
        }

        /* Delete operations used by list page and settings */
        public async Task DeleteEntryAsync(DateTime date)
        {
            var entry = await GetEntryByDateAsync(date);
            if (entry is null) return;

            await _db.DeleteAsync(entry);
        }

        public Task DeleteAllEntriesAsync()
        {
            return _db.DeleteAllAsync<JournalEntry>();
        }

        /* Keeps field updates consistent across save and update */
        private static void CopyEditableFields(JournalEntry target, JournalEntry source)
        {
            target.Title = source.Title;
            target.Content = source.Content;
            target.PrimaryMood = source.PrimaryMood;
            target.SecondaryMood1 = source.SecondaryMood1;
            target.SecondaryMood2 = source.SecondaryMood2;
            target.Tags = source.Tags;
        }
    }
}
