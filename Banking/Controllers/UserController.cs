using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class UserController : ControllerBase
{
    private readonly BankingDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public UserController(
        BankingDbContext context,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        var exists = await _context.Users
            .AnyAsync(x => x.Username == user.Username || x.Email == user.Email);

        if (exists)
            return BadRequest("User already exists");

        user.Password = _passwordService.HashPassword(user.Password);
        user.SecurityAnswerHash = _passwordService.HashPassword(user.SecurityAnswerHash);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto login)
    {
        if (string.IsNullOrWhiteSpace(login.Username) && string.IsNullOrWhiteSpace(login.Email))
        {
            return BadRequest("Enter username or email");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x =>
            (!string.IsNullOrEmpty(login.Username) && x.Username.ToLower() == login.Username.ToLower())
            ||
            (!string.IsNullOrEmpty(login.Email) && x.Email.ToLower() == login.Email.ToLower())
        );

        if (user == null)
            return Unauthorized("Invalid username/email");

        var valid = _passwordService.VerifyPassword(login.Password, user.Password);

        if (!valid)
            return Unauthorized("Invalid password");

        var token = _jwtService.GenerateToken(user);

        return Ok(new { token });
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.SecurityQuestion
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound("User not found");

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.SecurityQuestion
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound("User not found");

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;

        if (!string.IsNullOrWhiteSpace(updatedUser.Password))
        {
            user.Password = _passwordService.HashPassword(updatedUser.Password);
        }

        if (!string.IsNullOrWhiteSpace(updatedUser.SecurityQuestion))
        {
            user.SecurityQuestion = updatedUser.SecurityQuestion;
        }

        if (!string.IsNullOrWhiteSpace(updatedUser.SecurityAnswerHash))
        {
            user.SecurityAnswerHash =
                _passwordService.HashPassword(updatedUser.SecurityAnswerHash);
        }

        await _context.SaveChangesAsync();

        return Ok("User updated successfully ");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok("User deleted successfully ");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> GetSecurityQuestion([FromBody] string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

        if (user == null)
            return NotFound("User not found");

        return Ok(new { question = user.SecurityQuestion });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == dto.Email.ToLower());

        if (user == null)
            return NotFound("User not found");

        var validAnswer = _passwordService.VerifyPassword(
            dto.Answer,
            user.SecurityAnswerHash
        );

        if (!validAnswer)
            return BadRequest("Incorrect answer ");

        user.Password = _passwordService.HashPassword(dto.NewPassword);

        await _context.SaveChangesAsync();

        return Ok("Password reset successful ✅");
    }
}