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
            .AsNoTracking()
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<(List<Transaction>, int)> GetPagedFiltered(
        int accountId, int page, int size, string type, string sort)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId);

        if (!string.IsNullOrWhiteSpace(type))
        {
            var lowerType = type.ToLower();

            if (lowerType == "transfer")
            {
                query = query.Where(t => t.Type.ToLower().Contains("transfer"));
            }
            else
            {
                query = query.Where(t => t.Type.ToLower() == lowerType);
            }
        }

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

    public async Task AddRange(List<Transaction> transactions)
    {
        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }
}