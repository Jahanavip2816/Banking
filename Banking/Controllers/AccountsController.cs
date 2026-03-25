using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

 
    [HttpPost]
    public async Task<IActionResult> Create(AccountDto dto)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return Unauthorized("Invalid token");

        try
        {
            await _service.Create(dto, email);
            return StatusCode(201, "Account created successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAccount()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return Unauthorized("Invalid token");

        var accounts = await _repo.GetAll();

        var account = accounts.FirstOrDefault(a => a.Email == email);

        if (account == null)
            return NotFound("No account found");

        return Ok(account);
    }

   
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await _service.GetAll();
        return Ok(accounts);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        if (account.Email != email)
            return Unauthorized("Access denied ❌");

        return Ok(account);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AccountDto dto)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        if (account.Email != email)
            return Unauthorized("You can update only your account ❌");

        account.AccountHolderName = dto.AccountHolderName;
        account.Phone = dto.Phone;

        await _repo.Update(account);

        return Ok("Account updated successfully");
    }

 
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        if (account.Email != email)
            return Unauthorized("You can delete only your account ❌");

        await _repo.Delete(account);

        return Ok("Account deleted successfully");
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string keyword)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var results = await _service.Search(keyword);

        var filtered = results.Where(a => a.Email == email);

        return Ok(filtered);
    }
}