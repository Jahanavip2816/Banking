using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _service;
    private readonly IAccountRepository _repo;

    public AccountsController(AccountService service, IAccountRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    // CREATE ACCOUNT
    [HttpPost]
    public async Task<IActionResult> Create(AccountDto dto)
    {
        await _service.Create(dto);
        return StatusCode(201, "Account created successfully");
    }

    // GET ALL ACCOUNTS
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await _service.GetAll();
        return Ok(accounts);
    }

    // GET ACCOUNT BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        return Ok(account);
    }

    // UPDATE ACCOUNT
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AccountDto dto)
    {
        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        account.AccountHolderName = dto.AccountHolderName;
        account.Email = dto.Email;
        account.Phone = dto.Phone;

        await _repo.Update(account);

        return Ok("Account updated successfully");
    }

    // DELETE ACCOUNT
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        await _repo.Delete(account);

        return Ok("Account deleted successfully");
    }

    // SEARCH ACCOUNT
    [HttpGet("search")]
    public async Task<IActionResult> Search(string keyword)
    {
        var result = await _service.Search(keyword);
        return Ok(result);
    }
}