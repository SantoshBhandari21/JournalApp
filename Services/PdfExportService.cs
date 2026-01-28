using JournalApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace JournalApp.Services
{
    public class PdfExportService
    {
        /* Generate a PDF file from journal entries and gives the saved file path */
        public string ExportJournalsToPdf(
            List<JournalEntry> entries,
            DateTime from,
            DateTime to)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf"
            );

            /* Build the PDF layout including header and journal content */
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

                    page.Content().Column(column =>
                    {
                        foreach (var entry in entries)
                        {
                            column.Item()
                                  .PaddingBottom(10)
                                  .BorderBottom(1)
                                  .Column(content =>
                                  {
                                      content.Item().Text(entry.EntryDate.ToString("yyyy-MM-dd")).Bold();
                                      content.Item().Text(entry.Title ?? "(No Title)").Italic();
                                      content.Item().Text(entry.Content ?? "");
                                      content.Item().Text($"Mood: {entry.PrimaryMood}");
                                      content.Item().Text($"Tags: {entry.Tags}");
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
