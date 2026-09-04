using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Bilty;

namespace ZMAllied.Infrastructure.Services
{
    public class BiltyPrintService : IBiltyPrintService
    {
        public BiltyPrintService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] Generate(Bilty bilty)
        {
            var qrCode = new PngByteQRCode(
                new QRCodeGenerator().CreateQrCode(
                    $"ZM Allied Bilty verification: {bilty.BiltyNumber} | ID: {bilty.Id}",
                    QRCodeGenerator.ECCLevel.Q
                )
            );

            var qrImage = qrCode.GetGraphic(4);

            return Document.Create(doc =>
            {
                foreach (var copy in new[] { "OFFICE COPY", "DRIVER COPY", "RECEIVER COPY", "SENDER COPY" })
                {
                    doc.Page(page => BuildPage(page, bilty, copy, qrImage));
                }
            }).GeneratePdf();
        }

        #region Private Methods

        private static void BuildPage(PageDescriptor page, Bilty bilty, string copy, byte[] qrImage)
        {
            page.Size(PageSizes.A4);
            page.Margin(26);
            page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9));

            // Header
            page.Header()
                .Background("#1E3A63")
                .Padding(12)
                .Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Z&M ENTERPRISES / ZM ALLIED")
                            .FontColor(Colors.White)
                            .FontSize(18)
                            .Bold();

                        col.Item().Text("Digital Bilty Receipt - Digital Transport Receipt")
                            .FontColor("#F2C230")
                            .FontSize(10);
                    });

                    row.ConstantItem(150)
                        .Background("#D9B332")
                        .Padding(8)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text(copy + " / NAKAL")
                        .Bold();
                });

            // Content
            page.Content()
                .PaddingTop(12)
                .Column(col =>
                {
                    col.Spacing(7);

                    // Bilty Info Row
                    col.Item()
                        .Border(1)
                        .BorderColor("#C8D2DF")
                        .Padding(7)
                        .Row(row =>
                        {
                            row.RelativeItem().Text($"Bilty No: {bilty.BiltyNumber}").Bold();
                            row.RelativeItem().Text($"Date: {bilty.BiltyDate:dd-MMM-yyyy}").Bold();
                            row.RelativeItem().Text($"Branch: {bilty.Office.Name}").Bold();
                            row.ConstantItem(48).Image(qrImage);
                        });

                    // Route & Vehicle Info
                    col.Item()
                        .Background("#EAF1F8")
                        .Padding(7)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Text($"Route: {bilty.LoadingPoint ?? "N/A"} -> {bilty.OffloadingPoint ?? "N/A"}")
                                .Bold();

                            row.RelativeItem()
                                .Text($"Truck: {bilty.Vehicle.RegistrationNumber}")
                                .Bold();

                            row.RelativeItem()
                                .Text($"Driver: {bilty.Driver.Name} | Ph: {bilty.Driver.Phone ?? "N/A"} | CNIC: {bilty.Driver.CNIC ?? "N/A"}");
                        });

                    // Sender & Receiver
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Element(x => PartyBox(x, "SENDER / BHEJNEWALA", bilty.Consignor?.Name, bilty.Consignor?.Phone, bilty.Consignor?.Address));
                        row.ConstantItem(14);
                        row.RelativeItem().Element(x => PartyBox(x, "RECEIVER / WASOOL KUNINDA", bilty.Consignee?.Name, bilty.Consignee?.Phone, bilty.Consignee?.Address));
                    });

                    // Items Table
                    col.Item().Element(x => BuildItemsTable(x, bilty));

                    // Charges Table
                    col.Item().Element(x => BuildChargesTable(x, bilty));

                    // Terms
                    col.Item().Text("Contract Terms & Conditions / Sharaait o Zawabit")
                        .Bold()
                        .AlignRight();

                    col.Item()
                        .Text(bilty.PaymentTerms ?? "Goods are carried subject to the agreed terms. Sender is responsible for accurate consignment information. Payment is due according to the terms above.")
                        .FontSize(7);

                    // Signatures
                    col.Item()
                        .PaddingTop(14)
                        .Row(row =>
                        {
                            row.RelativeItem().Text("____________________\nSender Sign / Bhejnewalay ka Dastakhat");
                            row.RelativeItem().Text("____________________\nDriver Sign / Driver ka Dastakhat");
                            row.RelativeItem().Text("____________________\nAuthorized Stamp & Sign");
                        });
                });

            // Footer
            page.Footer()
                .AlignCenter()
                .Text($"Electronically generated multi-copy document - Verification: Bilty {bilty.BiltyNumber} (ID {bilty.Id})")
                .FontSize(7)
                .FontColor(Colors.Grey.Darken1);
        }

        private static void PartyBox(IContainer container, string title, string? name, string? phone, string? address)
        {
            container
                .Border(1)
                .BorderColor("#C8D2DF")
                .Padding(8)
                .Column(col =>
                {
                    col.Item().Text(title).Bold().FontColor("#1E3A63");
                    col.Item().Text($"Name: {name ?? "N/A"}");
                    col.Item().Text($"Phone: {phone ?? "N/A"}");
                    col.Item().Text($"Address: {address ?? "N/A"}");
                });
        }

        private static void BuildItemsTable(IContainer container, Bilty bilty)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(5);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#1E3A63").Padding(5).Text("GOODS DESCRIPTION").FontColor(Colors.White);
                    header.Cell().Background("#1E3A63").Padding(5).Text("PACKAGES").FontColor(Colors.White);
                    header.Cell().Background("#1E3A63").Padding(5).Text("WEIGHT").FontColor(Colors.White);
                    header.Cell().Background("#1E3A63").Padding(5).Text("AMOUNT").FontColor(Colors.White);
                });

                foreach (var item in bilty.BiltyItems)
                {
                    table.Cell().BorderBottom(1).BorderColor("#C8D2DF").Padding(5).Text(item.Description);
                    table.Cell().BorderBottom(1).BorderColor("#C8D2DF").Padding(5).Text($"{item.Quantity} {item.Unit}");
                    table.Cell().BorderBottom(1).BorderColor("#C8D2DF").Padding(5).Text($"{item.Weight} kg");
                    table.Cell().BorderBottom(1).BorderColor("#C8D2DF").Padding(5).Text($"Rs. {item.Amount:N2}");
                }
            });
        }

        private static void BuildChargesTable(IContainer container, Bilty bilty)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(1);
                });

                void AddRow(string label, string value, string? backgroundColor = null)
                {
                    table.Cell()
                        .Background(backgroundColor ?? Colors.White)
                        .Padding(5)
                        .Text(label);

                    table.Cell()
                        .Background(backgroundColor ?? Colors.White)
                        .Padding(5)
                        .Text(value)
                        .AlignRight();
                }

                table.Header(header =>
                {
                    header.Cell().Background("#1E3A63").Padding(5).Text("CHARGES SUMMARY / KHARCHAY").FontColor(Colors.White);
                    header.Cell().Background("#1E3A63").Padding(5).Text("AMOUNT (RS.)").FontColor(Colors.White);
                });

                AddRow("Freight Charges", $"Rs. {bilty.Freight:N2}");
                AddRow("Labor Charges", "Rs. 0.00");
                AddRow("TOTAL AMOUNT", $"Rs. {bilty.Freight:N2}", "#D9B332");
                AddRow("Advance Paid", $"Rs. {bilty.Advance:N2}");
                AddRow("BALANCE DUE TO PAY", $"Rs. {bilty.Balance:N2}", "#FFF2CC");
            });
        }

        #endregion
    }
}