using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _service;
    private readonly IAccountRepository _accRepo;

    public TransactionsController(
        ITransactionService service,
        IAccountRepository accRepo)
    {
        _service = service;
        _accRepo = accRepo;
    }

    private async Task<bool> IsOwner(int accountId)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return false;

        var account = await _accRepo.GetById(accountId);

        return account != null && account.Email == email;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(TransactionDto dto)
    {
        try
        {
            if (!await IsOwner(dto.AccountId))
                return Unauthorized("Access denied ❌");

            var transferDto = new TransferDto
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Description = dto.Description,
                Pin = dto.Pin
            };

            await _service.Deposit(transferDto);

            return Ok(new { message = "Deposit successful" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(TransactionDto dto)
    {
        try
        {
            if (!await IsOwner(dto.AccountId))
                return Unauthorized("Access denied ❌");

            var transferDto = new TransferDto
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Description = dto.Description,
                Pin = dto.Pin
            };

            await _service.Withdraw(transferDto);

            return Ok(new { message = "Withdrawal successful" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(TransferDto dto)
    {
        try
        {
            if (!await IsOwner(dto.AccountId))
                return Unauthorized("Access denied ❌");

            await _service.Transfer(dto);

            return Ok(new { message = "Transfer successful" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetByAccount(int accountId)
    {
        try
        {
            if (!await IsOwner(accountId))
                return Unauthorized("Access denied ❌");

            var result = await _service.GetTransactionsByAccount(accountId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("account/{accountId}/paged")]
    public async Task<IActionResult> GetPaged(
        int accountId,
        int page = 1,
        int size = 5,
        string type = null,
        string sort = "desc")
    {
        try
        {
            if (!await IsOwner(accountId))
                return Unauthorized("Access denied ❌");

            var result = await _service.GetPagedFiltered(
                accountId, page, size, type, sort);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}