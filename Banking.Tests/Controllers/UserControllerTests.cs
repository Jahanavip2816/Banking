using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UserControllerTests
{
    private BankingDbContext GetDb()
    {
        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BankingDbContext(options);
    }

    private UserController GetController(
        BankingDbContext context,
        Mock<IPasswordService> passwordMock,
        Mock<IJwtService> jwtMock)
    {
        return new UserController(
            context,
            passwordMock.Object,
            jwtMock.Object
        );
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenSuccess()
    {
        var context = GetDb();
        var passwordMock = new Mock<IPasswordService>();
        var jwtMock = new Mock<IJwtService>();

        passwordMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                    .Returns("hashed");

        var controller = GetController(context, passwordMock, jwtMock);

        var user = new User
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "Password@123",
            SecurityQuestion = "Pet?",
            SecurityAnswerHash = "Dog"
        };

        var result = await controller.Register(user);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User registered successfully", ok.Value);
    }

   
    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserExists()
    {
        var context = GetDb();

        context.Users.Add(new User
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "hash",
            SecurityQuestion = "Pet?",
            SecurityAnswerHash = "hash"
        });

        context.SaveChanges();

        var passwordMock = new Mock<IPasswordService>();
        var jwtMock = new Mock<IJwtService>();

        var controller = GetController(context, passwordMock, jwtMock);

        var user = new User
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "Password@123",
            SecurityQuestion = "Pet?",
            SecurityAnswerHash = "Dog"
        };

        var result = await controller.Register(user);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenValid()
    {
        var context = GetDb();

        context.Users.Add(new User
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "hashed",
            SecurityQuestion = "Pet?",
            SecurityAnswerHash = "hash"
        });

        context.SaveChanges();

        var passwordMock = new Mock<IPasswordService>();
        var jwtMock = new Mock<IJwtService>();

        passwordMock.Setup(x => x.VerifyPassword("pass", "hashed"))
                    .Returns(true);

        jwtMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
               .Returns("fake-jwt");

        var controller = GetController(context, passwordMock, jwtMock);

        var result = await controller.Login(new LoginDto
        {
            Username = "test",
            Password = "pass"
        });

        var ok = Assert.IsType<OkObjectResult>(result);
    }

  
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenInvalidPassword()
    {
        var context = GetDb();

        context.Users.Add(new User
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "hashed",
            SecurityQuestion = "Pet?",
            SecurityAnswerHash = "hash"
        });

        context.SaveChanges();

        var passwordMock = new Mock<IPasswordService>();
        var jwtMock = new Mock<IJwtService>();

        passwordMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(false);

        var controller = GetController(context, passwordMock, jwtMock);

        var result = await controller.Login(new LoginDto
        {
            Username = "test",
            Password = "wrong"
        });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

   
    [Fact]
    public async Task GetAllUsers_ShouldReturnList()
    {
        var context = GetDb();

        context.Users.Add(new User
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "hash",
            SecurityQuestion = "Pet?",
            SecurityAnswerHash = "hash"
        });

        context.SaveChanges();

        var controller = GetController(
            context,
            new Mock<IPasswordService>(),
            new Mock<IJwtService>());

        var result = await controller.GetAllUsers();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
    }

  
    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenMissing()
    {
        var context = GetDb();

        var controller = GetController(
            context,
            new Mock<IPasswordService>(),
            new Mock<IJwtService>());

        var result = await controller.GetUser(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

   
    [Fact]
    public async Task DeleteUser_ShouldReturnOk_WhenDeleted()
    {
        var context = GetDb();

        var user = new User
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "hash",
            SecurityQuestion = "Pet?",
            SecurityAnswerHash = "hash"
        };

        context.Users.Add(user);
        context.SaveChanges();

        var controller = GetController(
            context,
            new Mock<IPasswordService>(),
            new Mock<IJwtService>());

        var result = await controller.DeleteUser(user.Id);

        Assert.IsType<OkObjectResult>(result);
    }
}