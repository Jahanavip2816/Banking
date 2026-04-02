using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class AccountsControllerTests
{
    private readonly Mock<IAccountService> _serviceMock;
    private readonly Mock<IAccountRepository> _repoMock;
    private readonly BankingDbContext _context;
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _serviceMock = new Mock<IAccountService>();
        _repoMock = new Mock<IAccountRepository>();

        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _context = new BankingDbContext(options);

        _controller = new AccountsController(
            _serviceMock.Object,
            _repoMock.Object,
            _context
        );

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "test@mail.com")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

   
    [Fact]
    public async Task Create_ReturnsOk_WhenSuccess()
    {
        _repoMock.Setup(x => x.GetByEmail("test@mail.com"))
                 .ReturnsAsync((Account)null);

        _context.Users.Add(new User
        {
            Id = 1,
            Email = "test@mail.com",
            Username = "testuser",
            Password = "hash",
            SecurityQuestion = "pet?",
            SecurityAnswerHash = "hash"
        });

        await _context.SaveChangesAsync();

        var dto = new AccountDto
        {
            AccountHolderName = "John",
            Phone = "1234567890",
            Password = "pass",
            Pin = "1234"
        };

        var result = await _controller.Create(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Account created successfully ✅", okResult.Value);
    }

   
    [Fact]
    public async Task Create_ReturnsUnauthorized_WhenNoEmail()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

        var result = await _controller.Create(new AccountDto());

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

   
    [Fact]
    public async Task Create_ReturnsBadRequest_WhenAccountExists()
    {
        _repoMock.Setup(x => x.GetByEmail("test@mail.com"))
                 .ReturnsAsync(new Account());

        var dto = new AccountDto
        {
            AccountHolderName = "John",
            Phone = "123",
            Password = "pass"
        };

        var result = await _controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}