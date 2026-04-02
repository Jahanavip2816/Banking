using Microsoft.EntityFrameworkCore;

public class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Account>> GetAll()
        => await _context.Accounts
            .AsNoTracking()
            .ToListAsync();

    public async Task<Account> GetById(int id)
        => await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id);

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
    public async Task<Account> GetByEmail(string email)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<List<Account>> Search(string keyword)
    {
        keyword = keyword?.ToLower() ?? "";

        return await _context.Accounts
            .AsNoTracking()
            .Where(a =>
                a.AccountHolderName.ToLower().Contains(keyword) ||
                a.Email.ToLower().Contains(keyword))
            .ToListAsync();
    }
}