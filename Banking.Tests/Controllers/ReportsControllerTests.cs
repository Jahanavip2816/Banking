using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class ReportsControllerTests
{
    private BankingDbContext GetDb()
    {
        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BankingDbContext(options);
    }

    private ReportsController GetController(Mock<IReportService> mock, BankingDbContext context)
    {
        return new ReportsController(mock.Object, context);
    }
    [Fact]
    public async Task GetCsv_ShouldReturnFile()
    {
        var mock = new Mock<IReportService>();
        var context = GetDb();

        mock.Setup(s => s.GenerateCsv(1))
            .ReturnsAsync(new byte[] { 1, 2, 3 });

        var controller = GetController(mock, context);

        var result = await controller.GetCsv(1);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", file.ContentType);
    }

 
    [Fact]
    public async Task GetPdf_ShouldReturnNotFound_WhenAccountMissing()
    {
        var mock = new Mock<IReportService>();
        var context = GetDb();

        var controller = GetController(mock, context);

        var result = await controller.GetPdf(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

   
    [Fact]
    public async Task GetPdf_ShouldReturnFile_WhenValid()
    {
        var mock = new Mock<IReportService>();
        var context = GetDb();

        var account = new Account
        {
            Id = 1,
            AccountHolderName = "John",
            Email = "john@mail.com",
            Phone = "1234567890",
            PasswordHash = "hash",
            PinHash = "1234"
        };

        context.Accounts.Add(account);

        context.Transactions.Add(new Transaction
        {
            Id = 1,
            AccountId = 1,
            Amount = 100,
            Type = "Credit",
            Date = DateTime.Now
        });

        context.SaveChanges();

        mock.Setup(s => s.GeneratePdf(It.IsAny<Account>(), It.IsAny<List<Transaction>>()))
            .Returns(new byte[] { 1, 2, 3 });

        var controller = GetController(mock, context);

        var result = await controller.GetPdf(1);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", file.ContentType);
    }

   
    [Fact]
    public async Task GetReceipt_ShouldReturnFile()
    {
        var mock = new Mock<IReportService>();
        var context = GetDb();

        var account = new Account
        {
            Id = 1,
            AccountHolderName = "John",
            Email = "john@mail.com",
            Phone = "1234567890",
            PasswordHash = "hash",
            PinHash = "1234"
        };

        context.Accounts.Add(account);

        context.Transactions.Add(new Transaction
        {
            Id = 1,
            AccountId = 1,
            Amount = 200,
            Type = "Debit",
            Date = DateTime.Now
        });

        context.SaveChanges();

        mock.Setup(s => s.GenerateReceiptPdf(It.IsAny<Account>(), It.IsAny<List<Transaction>>()))
            .Returns(new byte[] { 1, 2, 3 });

        var controller = GetController(mock, context);

        var result = await controller.GetReceipt(1);

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", file.ContentType);
    }
}