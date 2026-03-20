using Microsoft.EntityFrameworkCore;

public class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Account>> GetAll()
        => await _context.Accounts.ToListAsync();

    public async Task<Account> GetById(int id)
        => await _context.Accounts.FindAsync(id);

    public async Task Add(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Account account)
    {
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Account>> Search(string keyword)
    {
        return await _context.Accounts
            .Where(a => a.AccountHolderName.Contains(keyword) || a.Email.Contains(keyword))
            .ToListAsync();
    }
}