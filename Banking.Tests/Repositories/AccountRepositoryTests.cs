using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AccountRepositoryTests
{
    private BankingDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BankingDbContext(options);
    }

    [Fact]
    public async Task Add_Should_InsertAccount()
    {
        var context = GetDbContext();
        var repo = new AccountRepository(context);

        var account = new Account
        {
            AccountHolderName = "John",
            Email = "john@mail.com",
            Phone = "1234567890",
            PasswordHash = "hashed_password",
            PinHash = "1234" // ✅ FIX
        };

        await repo.Add(account);

        var result = context.Accounts.FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal("john@mail.com", result.Email);
    }

    [Fact]
    public async Task GetByEmail_Should_ReturnCorrectAccount()
    {
        var context = GetDbContext();

        context.Accounts.Add(new Account
        {
            AccountHolderName = "Alice",
            Email = "alice@mail.com",
            Phone = "9999999999",
            PasswordHash = "hash",
            PinHash = "1234" 
        });

        await context.SaveChangesAsync();

        var repo = new AccountRepository(context);

        var result = await repo.GetByEmail("alice@mail.com");

        Assert.NotNull(result);
        Assert.Equal("Alice", result.AccountHolderName);
    }

    [Fact]
    public async Task Delete_Should_RemoveAccount()
    {
        var context = GetDbContext();

        var account = new Account
        {
            AccountHolderName = "Bob",
            Email = "bob@mail.com",
            Phone = "8888888888",
            PasswordHash = "hash",
            PinHash = "1234" 
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var repo = new AccountRepository(context);

        await repo.Delete(account);

        Assert.Empty(context.Accounts);
    }

    [Fact]
    public async Task GetById_Should_ReturnAccount()
    {
        var context = GetDbContext();

        var account = new Account
        {
            AccountHolderName = "Test",
            Email = "test@mail.com",
            Phone = "123",
            PasswordHash = "hash",
            PinHash = "1234" 
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var repo = new AccountRepository(context);

        var result = await repo.GetById(account.Id);

        Assert.NotNull(result);
    }

 
    [Fact]
    public async Task Update_Should_ModifyAccount()
    {
        var context = GetDbContext();

        var account = new Account
        {
            AccountHolderName = "Old",
            Email = "old@mail.com",
            Phone = "123",
            PasswordHash = "hash",
            PinHash = "1234" 
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var repo = new AccountRepository(context);

        account.AccountHolderName = "New";

        await repo.Update(account);

        var updated = context.Accounts.First();

        Assert.Equal("New", updated.AccountHolderName);
    }

   
    [Fact]
    public async Task Search_Should_ReturnMatchingAccounts()
    {
        var context = GetDbContext();

        context.Accounts.AddRange(
            new Account
            {
                AccountHolderName = "John Doe",
                Email = "john@mail.com",
                Phone = "1",
                PasswordHash = "hash",
                PinHash = "1234" 
            },
            new Account
            {
                AccountHolderName = "Jane",
                Email = "jane@mail.com",
                Phone = "2",
                PasswordHash = "hash",
                PinHash = "1234" 
            }
        );

        await context.SaveChangesAsync();

        var repo = new AccountRepository(context);

        var result = await repo.Search("john");

        Assert.Single(result);
    }
}