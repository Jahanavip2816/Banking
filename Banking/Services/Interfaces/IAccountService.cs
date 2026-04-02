public interface IAccountService
{
    Task Create(AccountDto dto, string email);

    Task<List<Account>> GetAll();

    Task<List<Account>> Search(string keyword);
}