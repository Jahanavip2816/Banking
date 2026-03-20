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
        user.Password = _passwordService.HashPassword(user.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    // LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login(User login)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == login.Username);

        if (user == null)
            return Unauthorized("Invalid username");

        var valid = _passwordService.VerifyPassword(login.Password, user.Password);

        if (!valid)
            return Unauthorized("Invalid password");

        var token = _jwtService.GenerateToken(user.Username);

        return Ok(new
        {
            Token = token
        });
    }
}