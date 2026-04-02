using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _service;
    private readonly IAccountRepository _repo;
    private readonly BankingDbContext _context;

    public AccountsController(
        IAccountService service,
        IAccountRepository repo,
        BankingDbContext context)
    {
        _service = service;
        _repo = repo;
        _context = context;
    }


[HttpPost]
    public async Task<IActionResult> Create(AccountDto dto)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            Console.WriteLine("EMAIL FROM TOKEN: " + email);

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return BadRequest("User not found ❌");

            var existing = await _repo.GetByEmail(email);

            if (existing != null)
                return BadRequest("Account already exists ❌");

            var account = new Account
            {
                AccountHolderName = dto.AccountHolderName,
                Phone = dto.Phone,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PinHash = BCrypt.Net.BCrypt.HashPassword(dto.Pin),
                Balance = 0,
                CreatedDate = DateTime.Now,
                UserId = user.Id
            };

            await _repo.Add(account);

            return Ok("Account created successfully ✅");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
            Console.WriteLine("INNER: " + ex.InnerException?.Message);

            return StatusCode(500, new
            {
                message = ex.InnerException?.Message
            });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAccount()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return Unauthorized("Invalid token");

        var account = await _repo.GetByEmail(email);

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

        if (string.IsNullOrEmpty(email))
            return Unauthorized("Invalid token");

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

        if (string.IsNullOrEmpty(email))
            return Unauthorized("Invalid token");

        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        if (account.Email != email)
            return Unauthorized("You can update only your account ❌");

        account.AccountHolderName = dto.AccountHolderName;
        account.Phone = dto.Phone;

        await _repo.Update(account);

        return Ok(new { message = "Account updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return Unauthorized("Invalid token");

        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        if (account.Email != email)
            return Unauthorized("You can delete only your account ❌");

        await _repo.Delete(account);

        return Ok(new { message = "Account deleted successfully" });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string keyword)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return Unauthorized("Invalid token");

        var results = await _service.Search(keyword);

        var filtered = results.Where(a => a.Email == email);

        return Ok(filtered);
    }

    // ✅ UPDATE PIN
    [HttpPut("{id}/pin")]
    public async Task<IActionResult> UpdatePin(int id, string newPin)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var account = await _repo.GetById(id);

        if (account == null)
            return NotFound("Account not found");

        if (account.Email != email)
            return Unauthorized("Access denied ❌");

        account.PinHash = BCrypt.Net.BCrypt.HashPassword(newPin);

        await _repo.Update(account);

        return Ok("PIN updated successfully");
    }
}