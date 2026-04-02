using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _repoMock;
    private readonly AccountService _service;
    private readonly PasswordService _passwordService;

    public AccountServiceTests()
    {
        _repoMock = new Mock<IAccountRepository>();
        _passwordService = new PasswordService();

        _service = new AccountService(
            _repoMock.Object,
            _passwordService
        );
    }

    
    [Fact]
    public async Task Create_Should_AddAccount_WhenValid()
    {
        var email = "test@mail.com";

        _repoMock.Setup(x => x.GetByEmail(email))
                 .ReturnsAsync((Account)null);

        _repoMock.Setup(x => x.Add(It.IsAny<Account>()))
                 .Returns(Task.CompletedTask);

        var dto = new AccountDto
        {
            AccountHolderName = "John",
            Phone = "1234567890",
            Password = "password",
            Pin = "1234"
        };

        await _service.Create(dto, email);

        _repoMock.Verify(x => x.Add(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task Create_Should_Throw_WhenAccountExists()
    {
        var email = "test@mail.com";

        _repoMock.Setup(x => x.GetByEmail(email))
                 .ReturnsAsync(new Account { Email = email });

        var dto = new AccountDto
        {
            AccountHolderName = "John",
            Phone = "1234567890",
            Password = "password"
        };

        await Assert.ThrowsAsync<Exception>(() =>
            _service.Create(dto, email));
    }

    [Fact]
    public async Task Create_Should_Throw_WhenPasswordMissing()
    {
        var email = "test@mail.com";

        _repoMock.Setup(x => x.GetByEmail(email))
                 .ReturnsAsync((Account)null);

        var dto = new AccountDto
        {
            AccountHolderName = "John",
            Phone = "1234567890",
            Password = null
        };

        await Assert.ThrowsAsync<Exception>(() =>
            _service.Create(dto, email));
    }

    [Fact]
    public async Task GetAll_Should_ReturnAccounts()
    {
        var list = new List<Account>
        {
            new Account { Email = "a@mail.com" },
            new Account { Email = "b@mail.com" }
        };

        _repoMock.Setup(x => x.GetAll())
                 .ReturnsAsync(list);

        var result = await _service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Search_Should_ReturnFilteredAccounts()
    {
        var list = new List<Account>
        {
            new Account { Email = "john@mail.com" }
        };

        _repoMock.Setup(x => x.Search("john"))
                 .ReturnsAsync(list);

        var result = await _service.Search("john");

        Assert.Single(result);
    }
}