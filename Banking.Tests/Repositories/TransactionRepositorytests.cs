using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TransactionRepositorytests
{
    private BankingDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BankingDbContext(options);
    }

    [Fact]
    public async Task Add_Should_InsertTransaction()
    {
        var context = GetDbContext();
        var repo = new TransactionRepository(context);

        var transaction = new Transaction
        {
            AccountId = 1,
            Amount = 100,
            Type = "deposit"
        };

        await repo.Add(transaction);

        Assert.Equal(1, context.Transactions.Count());
    }

    
    [Fact]
    public async Task AddRange_Should_InsertMultiple()
    {
        var context = GetDbContext();
        var repo = new TransactionRepository(context);

        var list = new List<Transaction>
        {
            new Transaction { AccountId = 1, Amount = 100, Type = "deposit" },
            new Transaction { AccountId = 1, Amount = 200, Type = "withdraw" }
        };

        await repo.AddRange(list);

        Assert.Equal(2, context.Transactions.Count());
    }

    [Fact]
    public async Task GetByAccount_Should_ReturnCorrectData()
    {
        var context = GetDbContext();

        context.Transactions.Add(new Transaction { AccountId = 1, Amount = 100, Type = "deposit" });
        context.Transactions.Add(new Transaction { AccountId = 2, Amount = 200, Type = "deposit" });

        context.SaveChanges();

        var repo = new TransactionRepository(context);

        var result = await repo.GetByAccount(1);

        Assert.Single(result);
    }

   
    [Fact]
    public async Task GetPagedFiltered_Should_ReturnPagedData()
    {
        var context = GetDbContext();

        for (int i = 1; i <= 10; i++)
        {
            context.Transactions.Add(new Transaction
            {
                AccountId = 1,
                Amount = i * 10,
                Type = "deposit",
                Date = DateTime.UtcNow.AddDays(-i)
            });
        }

        context.SaveChanges();

        var repo = new TransactionRepository(context);

        var (data, total) = await repo.GetPagedFiltered(1, 1, 5, null, "desc");

        Assert.Equal(10, total);
        Assert.Equal(5, data.Count);
    }
}