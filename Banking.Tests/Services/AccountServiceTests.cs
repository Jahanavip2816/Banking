using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AccountServiceTests
{
    [Fact]
    public async Task Create_Should_Add_Account()
    {
        // Arrange
        var repoMock = new Mock<IAccountRepository>();
        var passwordService = new PasswordService();

        // No existing accounts
        repoMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<Account>());

        var service = new AccountService(repoMock.Object, passwordService);

        var dto = new AccountDto
        {
            AccountHolderName = "Jaanu",
            Phone = "1234567890",
            Password = "123456"
        };

        var email = "test@mail.com";

        // Act
        await service.Create(dto, email);

        // Assert
        repoMock.Verify(x => x.Add(It.Is<Account>(a =>
            a.Email == email &&
            a.AccountHolderName == dto.AccountHolderName
        )), Times.Once);
    }
    [Fact]
    public async Task Create_Should_Throw_Exception_If_Account_Exists()
    {
        // Arrange
        var repoMock = new Mock<IAccountRepository>();
        var passwordService = new PasswordService();

        repoMock.Setup(x => x.GetAll())
            .ReturnsAsync(new List<Account>
            {
            new Account { Email = "test@mail.com" }
            });

        var service = new AccountService(repoMock.Object, passwordService);

        var dto = new AccountDto
        {
            AccountHolderName = "Jaanu",
            Phone = "1234567890",
            Password = "123456"
        };

        var email = "test@mail.com";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.Create(dto, email)
        );

        Assert.Equal("User already has an account ❌", exception.Message);
    }
}