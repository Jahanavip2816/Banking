using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/transactions")]
[Authorize] 
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _service;
    private readonly ITransactionRepository _repo;

    public TransactionsController(
        TransactionService service,
        ITransactionRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(TransactionDto dto)
    {
        await _service.Deposit(dto);
        return Ok("Deposit successful");
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(TransactionDto dto)
    {
        await _service.Withdraw(dto);
        return Ok("Withdrawal successful");
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetByAccount(int accountId)
    {
        var result = await _service.GetTransactionsByAccount(accountId);
        return Ok(result);
    }

    [HttpGet("account/{accountId}/paged")]
    public async Task<IActionResult> GetPaged(
    int accountId,
    int page = 1,
    int size = 5,
    string type = null,
    string sort = "desc")
    {
        var result = await _service.GetPagedFiltered(accountId, page, size, type, sort);
        return Ok(result);
    }
}