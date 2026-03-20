using Xunit;
using Moq;
using System.Threading.Tasks;

public class TransactionServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly TransactionService _service;

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
    public async Task Deposit_Should_UpdateBalance()
    {
        var account = new Account { Id = 1, Balance = 1000 };

        _accountRepoMock.Setup(x => x.GetById(1))
                        .ReturnsAsync(account);

        await _service.Deposit(new TransactionDto
        {
            AccountId = 1,
            Amount = 500,
            Description = "Test"
        });

        Assert.Equal(1500, account.Balance);
    }


[Fact]
    public async Task Deposit_Should_Throw_When_InvalidAmount()
    {
        await Assert.ThrowsAsync<Exception>(() =>
            _service.Deposit(new TransactionDto
            {
                AccountId = 1,
                Amount = -100
            }));
    }

    [Fact]
    public async Task Deposit_Should_Throw_When_Account_NotFound()
    {
        _accountRepoMock.Setup(x => x.GetById(It.IsAny<int>()))
                        .ReturnsAsync((Account?)null);

        await Assert.ThrowsAsync<Exception>(() =>
            _service.Deposit(new TransactionDto
            {
                AccountId = 1,
                Amount = 100
            }));
    }
}