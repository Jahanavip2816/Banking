public interface IAccountRepository
{
    Task<List<Account>> GetAll();
    Task<Account> GetById(int id);
    Task Add(Account account);
    Task Update(Account account);
    Task Delete(Account account);
    Task<List<Account>> Search(string keyword);
}