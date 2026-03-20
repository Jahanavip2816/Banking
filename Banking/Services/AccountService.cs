public class AccountService
{
    private readonly IAccountRepository _repo;
    private readonly PasswordService _passwordService;

    public AccountService(IAccountRepository repo, PasswordService passwordService)
    {
        _repo = repo;
        _passwordService = passwordService;
    }

    public async Task Create(AccountDto dto)
    {
        var account = new Account
        {
            AccountHolderName = dto.AccountHolderName,
            Email = dto.Email,
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