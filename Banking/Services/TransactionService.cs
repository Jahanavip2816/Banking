using Banking.DTO;
using Microsoft.EntityFrameworkCore;

public class TransactionService
{
    private readonly ITransactionRepository _transRepo;
    private readonly IAccountRepository _accRepo;

    public TransactionService(ITransactionRepository transRepo, IAccountRepository accRepo)
    {
        _transRepo = transRepo;
        _accRepo = accRepo;
    }

    public async Task Deposit(TransactionDto dto)
    {
        var account = await _accRepo.GetById(dto.AccountId);

        if (account == null)
            throw new Exception("Account not found");

        if (dto.Amount <= 0)
            throw new Exception("Amount must be positive");

        account.Balance += dto.Amount;
        await _accRepo.Update(account);

        await _transRepo.Add(new Transaction
        {
            AccountId = dto.AccountId,
            Amount = dto.Amount,
            Type = "Deposit",
            Description = dto.Description
        });
    }

    public async Task Withdraw(TransactionDto dto)
    {
        var account = await _accRepo.GetById(dto.AccountId);

        if (account == null)
            throw new Exception("Account not found");

        if (dto.Amount <= 0)
            throw new Exception("Amount must be positive");

        if (account.Balance < dto.Amount)
            throw new Exception("Insufficient balance");

        account.Balance -= dto.Amount;
        await _accRepo.Update(account);

        await _transRepo.Add(new Transaction
        {
            AccountId = dto.AccountId,
            Amount = dto.Amount,
            Type = "Withdraw",
            Description = dto.Description
        });
    }

    public async Task<List<TransactionResponseDto>> GetTransactionsByAccount(int accountId)
    {
        var account = await _accRepo.GetById(accountId);

        if (account == null)
            throw new Exception("Account not found");

        var transactions = await _transRepo.GetByAccount(accountId);

        return transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            AccountId = t.AccountId,
            Amount = t.Amount,
            Type = t.Type,
            Description = t.Description,
            Date = t.Date
        }).ToList();
    }
    public async Task<object> GetPagedFiltered(
    int accountId, int page, int size, string type, string sort)
    {
        var account = await _accRepo.GetById(accountId);

        if (account == null)
            throw new Exception("Account not found");

        var (transactions, total) =
            await _transRepo.GetPagedFiltered(accountId, page, size, type, sort);

        var result = transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            AccountId = t.AccountId,
            Amount = t.Amount,
            Type = t.Type,
            Description = t.Description,
            Date = t.Date
        });

        return new
        {
            page,
            size,
            totalRecords = total,
            data = result
        };
    }
}