using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ReportService
{
    private readonly BankingDbContext _context;

    public ReportService(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateCsv(int accountId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .ToListAsync();

        if (!transactions.Any())
            throw new Exception("No transactions found");

        var sb = new StringBuilder();

        sb.AppendLine("TransactionId,Date,Type,Amount,Description");

        foreach (var t in transactions)
        {
            sb.AppendLine($"{t.Id},{t.Date:yyyy-MM-dd},{t.Type},{t.Amount},{t.Description}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
    public byte[] GeneratePdf(Account account, List<Transaction> transactions)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text($"Transactions for Account: {account.AccountHolderName}").FontSize(20).Bold();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40); 
                        columns.RelativeColumn(1);  
                        columns.RelativeColumn(1);  
                        columns.RelativeColumn(1);  
                        columns.RelativeColumn(2);  
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Id").Bold();
                        header.Cell().Text("Date").Bold();
                        header.Cell().Text("Type").Bold();
                        header.Cell().Text("Amount").Bold();
                        header.Cell().Text("Description").Bold();
                    });

                    foreach (var t in transactions)
                    {
                        table.Cell().Text(t.Id.ToString());
                        table.Cell().Text(t.Date.ToString("yyyy-MM-dd"));
                        table.Cell().Text(t.Type);
                        table.Cell().Text(t.Amount.ToString("C"));
                        table.Cell().Text(t.Description ?? "");
                    }
                });
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated on ");
                    x.Span($"{System.DateTime.Now:yyyy-MM-dd HH:mm}");
                });
            });
        });

        using var ms = new MemoryStream();
        document.GeneratePdf(ms);
        return ms.ToArray();
    }
    public byte[] GenerateReceiptPdf(Account account, List<Transaction> transactions)
    {
        var lastTransaction = transactions.LastOrDefault();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A6);
                page.Margin(10);

                page.Content().AlignCenter().Column(col =>
                {
                    col.Spacing(5);

                    col.Item().Text("ABC BANK").Bold().FontSize(16).AlignCenter();
                    col.Item().Text("ATM TRANSACTION RECEIPT").FontSize(10).AlignCenter();

                    col.Item().LineHorizontal(1);

                    col.Item().Text($"Account No: XXXX{account.Id}");
                    col.Item().Text($"Name: {account.AccountHolderName}");
                    col.Item().Text($"Date: {DateTime.Now:dd-MM-yyyy HH:mm}");

                    col.Item().LineHorizontal(1);

                    if (lastTransaction != null)
                    {
                        col.Item().Text($"Txn ID: {lastTransaction.Id}");
                        col.Item().Text($"Type: {lastTransaction.Type}");
                        col.Item().Text($"Amount: ₹ {lastTransaction.Amount}");
                        col.Item().Text($"Description: {lastTransaction.Description}");
                    }

                    col.Item().LineHorizontal(1);

                    col.Item().Text($"Available Balance: ₹ {account.Balance}").Bold();

                    col.Item().LineHorizontal(1);

                    col.Item().Text("Status: SUCCESS").Bold().AlignCenter();
                    col.Item().Text("Thank You For Banking With Us").AlignCenter();
                    col.Item().Text("** Please collect your card **").FontSize(8).AlignCenter();
                });
            });
        });

        using var ms = new MemoryStream();
        document.GeneratePdf(ms);
        return ms.ToArray();
    }
}