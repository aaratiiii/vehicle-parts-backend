using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace VehicleParts.API.Services;

public class InvoicePdfService
{
    public byte[] GenerateInvoicePdf(
        int invoiceId,
        string customerName,
        string customerEmail,
        decimal finalAmount
    )
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.Header().Column(column =>
                {
                    column.Item().Text("VehicleParts")
                        .FontSize(28)
                        .Bold()
                        .FontColor(Colors.Teal.Darken2);

                    column.Item().Text("Official Sales Invoice")
                        .FontSize(14)
                        .FontColor(Colors.Grey.Darken1);

                    column.Item().PaddingTop(10).LineHorizontal(1);
                });

                page.Content().PaddingTop(30).Column(column =>
                {
                    column.Spacing(18);

                    column.Item().Text($"Invoice No: INV-{invoiceId}")
                        .FontSize(16)
                        .Bold();

                    column.Item().Text($"Customer Name: {customerName}")
                        .FontSize(13);

                    column.Item().Text($"Customer Email: {customerEmail}")
                        .FontSize(13);

                    column.Item().PaddingVertical(10).LineHorizontal(1);

                    column.Item().Background(Colors.Teal.Lighten5).Padding(20).Column(box =>
                    {
                        box.Item().Text("Invoice Summary")
                            .FontSize(16)
                            .Bold()
                            .FontColor(Colors.Teal.Darken2);

                        box.Item().PaddingTop(10).Text($"Final Amount: Rs. {finalAmount}")
                            .FontSize(22)
                            .Bold()
                            .FontColor(Colors.Teal.Darken2);
                    });

                    column.Item().Text("Thank you for choosing VehicleParts.")
                        .FontSize(13)
                        .FontColor(Colors.Grey.Darken2);
                });

                page.Footer()
                    .AlignCenter()
                    .Text("VehicleParts Management System | Official Invoice")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken1);
            });
        });

        return pdf.GeneratePdf();
    }
}