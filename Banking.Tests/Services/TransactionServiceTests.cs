using Xunit;
using Moq;
using System.Threading.Tasks;

public class TransactionServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly ITransactionService _service;

    public TransactionServiceTests()
    {
        _accountRepoMock = new Mock<IAccountRepository>();
        _transactionRepoMock = new Mock<ITransactionRepository>();

        _service = new TransactionService(
            _transactionRepoMock.Object,
            _accountRepoMock.Object
        );
    }

    [Fact]
    public async Task Deposit_Should_UpdateBalance_And_AddTransaction()
    {
        var account = new Account
        {
            Id = 1,
            Balance = 1000,
            PinHash = BCrypt.Net.BCrypt.HashPassword("1234")
        };

        _accountRepoMock.Setup(x => x.GetById(1))
                        .ReturnsAsync(account);

        _accountRepoMock.Setup(x => x.Update(It.IsAny<Account>()))
                        .Returns(Task.CompletedTask);

        _transactionRepoMock.Setup(x => x.Add(It.IsAny<Transaction>()))
                            .Returns(Task.CompletedTask);

        await _service.Deposit(new TransferDto
        {
            AccountId = 1,
            Amount = 500,
            Pin = "1234"
        });

        Assert.Equal(1500, account.Balance);

        _accountRepoMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Once);
        _transactionRepoMock.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
    }

    
    [Fact]
    public async Task Withdraw_Should_DecreaseBalance_And_AddTransaction()
    {
        var account = new Account
        {
            Id = 1,
            Balance = 1000,
            PinHash = BCrypt.Net.BCrypt.HashPassword("1234")
        };

        _accountRepoMock.Setup(x => x.GetById(1))
                        .ReturnsAsync(account);

        _accountRepoMock.Setup(x => x.Update(It.IsAny<Account>()))
                        .Returns(Task.CompletedTask);

        _transactionRepoMock.Setup(x => x.Add(It.IsAny<Transaction>()))
                            .Returns(Task.CompletedTask);

        await _service.Withdraw(new TransferDto
        {
            AccountId = 1,
            Amount = 200,
            Pin = "1234"
        });

        Assert.Equal(800, account.Balance);

        _accountRepoMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Once);
        _transactionRepoMock.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
    }

    [Fact]
    public async Task Transfer_Should_MoveMoney_And_CreateTwoTransactions()
    {
        var sender = new Account
        {
            Id = 1,
            Balance = 1000,
            PinHash = BCrypt.Net.BCrypt.HashPassword("1234")
        };

        var receiver = new Account
        {
            Id = 2,
            Balance = 500
        };

        _accountRepoMock.Setup(x => x.GetById(1))
                        .ReturnsAsync(sender);

        _accountRepoMock.Setup(x => x.GetById(2))
                        .ReturnsAsync(receiver);

        _accountRepoMock.Setup(x => x.Update(It.IsAny<Account>()))
                        .Returns(Task.CompletedTask);

        _transactionRepoMock.Setup(x => x.Add(It.IsAny<Transaction>()))
                            .Returns(Task.CompletedTask);

        await _service.Transfer(new TransferDto
        {
            AccountId = 1,
            ReceiverAccountId = 2,
            Amount = 300,
            Pin = "1234"
        });

        Assert.Equal(700, sender.Balance);
        Assert.Equal(800, receiver.Balance);

        _accountRepoMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));

        _transactionRepoMock.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Exactly(2));
    }
}