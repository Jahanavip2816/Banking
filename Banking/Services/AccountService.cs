using BCrypt.Net;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;
    private readonly IPasswordService _passwordService;

    public AccountService(IAccountRepository repo, IPasswordService passwordService)
    {
        _repo = repo;
        _passwordService = passwordService;
    }

    public async Task Create(AccountDto dto, string email)
    {
        var existing = await _repo.GetByEmail(email);

        if (existing != null)
            throw new Exception("User already has an account ❌");

        if (string.IsNullOrEmpty(dto.Password))
            throw new Exception("Password is required ❌");

        if (string.IsNullOrEmpty(dto.Phone))
            throw new Exception("Phone is required ❌");

        var account = new Account
        {
            AccountHolderName = dto.AccountHolderName,
            Email = email,
            Phone = dto.Phone,
            PasswordHash = _passwordService.HashPassword(dto.Password),
            PinHash = string.IsNullOrEmpty(dto.Pin)
                ? null
                : BCrypt.Net.BCrypt.HashPassword(dto.Pin),
            Balance = 0,
            CreatedDate = DateTime.Now
        };

        await _repo.Add(account);
    }

    public async Task<List<Account>> GetAll()
        => await _repo.GetAll();

    public async Task<List<Account>> Search(string keyword)
        => await _repo.Search(keyword);
}