using Xunit;
using Moq;
using System.Threading.Tasks;

public class AccountServiceTests
{
    [Fact]
    public async Task Create_Should_Add_Account()
    {
        var repoMock = new Mock<IAccountRepository>();
        var passwordService = new PasswordService();

        var service = new AccountService(repoMock.Object, passwordService);

        var dto = new AccountDto
        {
            AccountHolderName = "Test",
            Email = "test@mail.com",
            Phone = "1234567890",
            Password = "123456"
        };

        await service.Create(dto);

        repoMock.Verify(x => x.Add(It.IsAny<Account>()), Times.Once);
    }
}