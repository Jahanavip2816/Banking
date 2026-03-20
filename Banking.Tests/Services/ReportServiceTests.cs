using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuestPDF.Infrastructure;

public class ReportServiceTests
{
    static ReportServiceTests()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Fact]
    public async Task GenerateCsv_Should_Return_Data()
    {
        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;

        using var context = new BankingDbContext(options);

        context.Transactions.Add(new Transaction
        {
            Id = 1,
            AccountId = 1,
            Amount = 100,
            Type = "Deposit",
            Date = DateTime.UtcNow
        });

        context.SaveChanges();

        var service = new ReportService(context);

        var result = await service.GenerateCsv(1);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void GeneratePdf_Should_Return_Bytes()
    {
        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB_Pdf")
            .Options;
        using var context = new BankingDbContext(options);

        var service = new ReportService(context);

        var account = new Account
        {
            AccountHolderName = "Test",
            Balance = 1000
        };

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Amount = 100,
                Type = "Deposit",
                Date = DateTime.UtcNow
            }
        };

        var result = service.GeneratePdf(account, transactions);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}