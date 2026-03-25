public class AccountService
{
    private readonly IAccountRepository _repo;
    private readonly PasswordService _passwordService;

    public AccountService(IAccountRepository repo, PasswordService passwordService)
    {
        _repo = repo;
        _passwordService = passwordService;
    }

    public async Task Create(AccountDto dto, string email) // ✅ accept email
    {
        // ❌ Prevent multiple accounts
        var existing = (await _repo.GetAll())
            .FirstOrDefault(a => a.Email == email);

        if (existing != null)
            throw new Exception("User already has an account ❌");

        var account = new Account
        {
            AccountHolderName = dto.AccountHolderName,
            Email = email, // ✅ from JWT
            Phone = dto.Phone,
            PasswordHash = _passwordService.HashPassword(dto.Password)
        };

        await _repo.Add(account);
    }

    public async Task<List<Account>> GetAll()
        => await _repo.GetAll();

    public async Task<List<Account>> Search(string keyword)
        => await _repo.Search(keyword);
}