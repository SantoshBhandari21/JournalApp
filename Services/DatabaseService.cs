using SQLite;
using JournalApp.Models;

namespace JournalApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService()
        {
            var dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "journal.db");

            _db = new SQLiteAsyncConnection(dbPath);

            // Create table if it does not exist
            _db.CreateTableAsync<JournalEntry>().Wait();
        }

        // ---------------------------------------
        // READ: Get all journal entries
        // ---------------------------------------
        public async Task<List<JournalEntry>> GetEntriesAsync()
        {
            return await _db
                .Table<JournalEntry>()
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // ---------------------------------------
        // READ: Get a single entry by date
        // ---------------------------------------
        public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
        {
            return await _db
                .Table<JournalEntry>()
                .FirstOrDefaultAsync(e => e.EntryDate == date.Date);
        }

        // --------------------------------------------------
        // CREATE or UPDATE (One journal entry per day)
        // Used by JournalPage.razor
        // --------------------------------------------------
        public async Task SaveEntryAsync(JournalEntry entry)
        {
            var existing = await GetEntryByDateAsync(entry.EntryDate);

            if (existing == null)
            {
                entry.CreatedAt = DateTime.Now;
                entry.UpdatedAt = DateTime.Now;

                await _db.InsertAsync(entry);
            }
            else
            {
                existing.Title = entry.Title;
                existing.Content = entry.Content;
                existing.PrimaryMood = entry.PrimaryMood;
                existing.SecondaryMood1 = entry.SecondaryMood1;
                existing.SecondaryMood2 = entry.SecondaryMood2;
                existing.Tags = entry.Tags;
                existing.UpdatedAt = DateTime.Now;

                await _db.UpdateAsync(existing);
            }
        }

        // --------------------------------------------------
        // UPDATE: Explicit update (EditJournalPage)
        // --------------------------------------------------
        public async Task UpdateEntryAsync(JournalEntry entry)
        {
            var existing = await GetEntryByDateAsync(entry.EntryDate);

            if (existing == null)
                return;

            existing.Title = entry.Title;
            existing.Content = entry.Content;
            existing.PrimaryMood = entry.PrimaryMood;
            existing.SecondaryMood1 = entry.SecondaryMood1;
            existing.SecondaryMood2 = entry.SecondaryMood2;
            existing.Tags = entry.Tags;
            existing.UpdatedAt = DateTime.Now;

            await _db.UpdateAsync(existing);
        }

        // ---------------------------------------
        // DELETE: Delete entry by date
        // ---------------------------------------
        public async Task DeleteEntryAsync(DateTime date)
        {
            var entry = await GetEntryByDateAsync(date);

            if (entry != null)
            {
                await _db.DeleteAsync(entry);
            }
        }

        // ---------------------------------------
        // DELETE: Delete ALL entries (Settings)
        // ---------------------------------------
        public async Task DeleteAllEntriesAsync()
        {
            await _db.DeleteAllAsync<JournalEntry>();
        }

        // ---------------------------------------
        // EXTRA / SUPPORTING METHODS
        // ---------------------------------------

        // Get entries within a date range (PDF export, analytics)
        public async Task<List<JournalEntry>> GetEntriesByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            return await _db
                .Table<JournalEntry>()
                .Where(e => e.EntryDate >= startDate.Date &&
                            e.EntryDate <= endDate.Date)
                .OrderBy(e => e.EntryDate)
                .ToListAsync();
        }

        // Check if entry exists for a specific date
        public async Task<bool> EntryExistsAsync(DateTime date)
        {
            var count = await _db
                .Table<JournalEntry>()
                .Where(e => e.EntryDate == date.Date)
                .CountAsync();

            return count > 0;
        }
    }
}