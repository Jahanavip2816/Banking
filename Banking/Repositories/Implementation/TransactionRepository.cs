using Microsoft.EntityFrameworkCore;

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingDbContext _context;

    public TransactionRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task Add(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetByAccount(int accountId)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .ToListAsync();
    }
    public async Task<(List<Transaction>, int)> GetPagedFiltered(
    int accountId, int page, int size, string type, string sort)
    {
        var query = _context.Transactions
            .Where(t => t.AccountId == accountId);

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(t => t.Type.ToLower() == type.ToLower());
        }

        // ✅ SORT (Date)
        query = sort?.ToLower() == "asc"
            ? query.OrderBy(t => t.Date)
            : query.OrderByDescending(t => t.Date);

        var total = await query.CountAsync();

        var data = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (data, total);
    }
}