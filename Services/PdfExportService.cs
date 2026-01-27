using JournalApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace JournalApp.Services
{
    public class PdfExportService
    {
        public string ExportJournalsToPdf(
            List<JournalEntry> entries,
            DateTime from,
            DateTime to)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var filePath =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header()
                        .Text($"Journal Entries ({from:yyyy-MM-dd} → {to:yyyy-MM-dd})")
                        .FontSize(18)
                        .Bold();

                    page.Content().Column(col =>
                    {
                        foreach (var entry in entries)
                        {
                            col.Item().PaddingBottom(10).BorderBottom(1).Column(c =>
                            {
                                c.Item().Text(entry.EntryDate.ToString("yyyy-MM-dd")).Bold();
                                c.Item().Text(entry.Title ?? "(No Title)").Italic();
                                c.Item().Text(entry.Content ?? "");
                                c.Item().Text($"Mood: {entry.PrimaryMood}");
                                c.Item().Text($"Tags: {entry.Tags}");
                            });
                        }
                    });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }
    }
}