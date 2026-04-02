using Banking.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionService> _serviceMock;
    private readonly Mock<IAccountRepository> _accRepoMock;
    private readonly TransactionsController _controller;

    public TransactionsControllerTests()
    {
        _serviceMock = new Mock<ITransactionService>();
        _accRepoMock = new Mock<IAccountRepository>();

        _controller = new TransactionsController(
            _serviceMock.Object,
            _accRepoMock.Object
        );
    }

    private void SetUser(string email)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, email)
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

  
    [Fact]
    public async Task Deposit_ReturnsOk_WhenSuccess()
    {
        var account = new Account { Id = 1, Email = "test@mail.com" };

        _accRepoMock.Setup(x => x.GetById(1))
                    .ReturnsAsync(account);

        _serviceMock.Setup(x => x.Deposit(It.IsAny<TransferDto>()))
                    .Returns(Task.CompletedTask);

        SetUser("test@mail.com");

        var dto = new TransactionDto { AccountId = 1, Amount = 100 };

        var result = await _controller.Deposit(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Deposit_ReturnsUnauthorized_WhenNotOwner()
    {
        var account = new Account { Id = 1, Email = "other@mail.com" };

        _accRepoMock.Setup(x => x.GetById(1))
                    .ReturnsAsync(account);

        SetUser("test@mail.com");

        var result = await _controller.Deposit(new TransactionDto { AccountId = 1 });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Withdraw_ReturnsOk_WhenSuccess()
    {
        var account = new Account { Id = 1, Email = "test@mail.com" };

        _accRepoMock.Setup(x => x.GetById(1))
                    .ReturnsAsync(account);

        _serviceMock.Setup(x => x.Withdraw(It.IsAny<TransferDto>()))
                    .Returns(Task.CompletedTask);

        SetUser("test@mail.com");

        var result = await _controller.Withdraw(new TransactionDto { AccountId = 1 });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Transfer_ReturnsOk_WhenSuccess()
    {
        var account = new Account { Id = 1, Email = "test@mail.com" };

        _accRepoMock.Setup(x => x.GetById(1))
                    .ReturnsAsync(account);

        _serviceMock.Setup(x => x.Transfer(It.IsAny<TransferDto>()))
                    .Returns(Task.CompletedTask);

        SetUser("test@mail.com");

        var dto = new TransferDto { AccountId = 1, ReceiverAccountId = 2 };

        var result = await _controller.Transfer(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetByAccount_ReturnsOk()
    {
        var account = new Account { Id = 1, Email = "test@mail.com" };

        _accRepoMock.Setup(x => x.GetById(1))
                    .ReturnsAsync(account);

        _serviceMock.Setup(x => x.GetTransactionsByAccount(1))
                    .ReturnsAsync(new List<TransactionResponseDto>());

        SetUser("test@mail.com");

        var result = await _controller.GetByAccount(1);

        Assert.IsType<OkObjectResult>(result);
    }
}