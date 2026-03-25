using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class UserController : ControllerBase
{
    private readonly BankingDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    public UserController(
        BankingDbContext context,
        PasswordService passwordService,
        JwtService jwtService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    // REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        var exists = await _context.Users
            .AnyAsync(x => x.Username == user.Username || x.Email == user.Email);

        if (exists)
            return BadRequest("User already exists");

        user.Password = _passwordService.HashPassword(user.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(User login)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username.ToLower() == login.Username.ToLower()
                || x.Email.ToLower() == login.Username.ToLower());

        if (user == null)
            return Unauthorized("Invalid username/email");

        var valid = _passwordService.VerifyPassword(login.Password, user.Password);

        if (!valid)
            return Unauthorized("Invalid password");

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            token = token
        });
    }
}