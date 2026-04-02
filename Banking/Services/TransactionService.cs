using Banking.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transRepo;
    private readonly IAccountRepository _accRepo;

    public TransactionService(
        ITransactionRepository transRepo,
        IAccountRepository accRepo)
    {
        _transRepo = transRepo;
        _accRepo = accRepo;
    }

  
    private void ValidatePin(string enteredPin, string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash))
            throw new Exception("PIN not set for this account");

        if (!BCrypt.Net.BCrypt.Verify(enteredPin, storedHash))
            throw new Exception("Invalid PIN ❌");
    }

   
    public async Task Deposit(TransferDto dto)
    {
        var account = await _accRepo.GetById(dto.AccountId);

        if (account == null)
            throw new Exception("Account not found");

        ValidatePin(dto.Pin, account.PinHash);

        if (dto.Amount <= 0)
            throw new Exception("Amount must be positive");

        account.Balance += dto.Amount;
        await _accRepo.Update(account);

        await _transRepo.Add(new Transaction
        {
            AccountId = account.Id,
            Amount = dto.Amount,
            Type = "deposit",
            Description = dto.Description
        });
    }

   
    public async Task Withdraw(TransferDto dto)
    {
        var account = await _accRepo.GetById(dto.AccountId);

        if (account == null)
            throw new Exception("Account not found");

        ValidatePin(dto.Pin, account.PinHash);

        if (dto.Amount <= 0)
            throw new Exception("Amount must be positive");

        if (account.Balance < dto.Amount)
            throw new Exception("Insufficient balance");

        account.Balance -= dto.Amount;
        await _accRepo.Update(account);

        await _transRepo.Add(new Transaction
        {
            AccountId = account.Id,
            Amount = dto.Amount,
            Type = "withdraw",
            Description = dto.Description
        });
    }

   
    public async Task Transfer(TransferDto dto)
    {
        if (dto.ReceiverAccountId == null)
            throw new Exception("Receiver account is required");

        var sender = await _accRepo.GetById(dto.AccountId);
        var receiver = await _accRepo.GetById(dto.ReceiverAccountId.Value);

        if (sender == null || receiver == null)
            throw new Exception("Invalid sender or receiver account");

        ValidatePin(dto.Pin, sender.PinHash);

        if (dto.Amount <= 0)
            throw new Exception("Amount must be positive");

        if (sender.Balance < dto.Amount)
            throw new Exception("Insufficient balance");

        sender.Balance -= dto.Amount;
        receiver.Balance += dto.Amount;

        await _accRepo.Update(sender);
        await _accRepo.Update(receiver);

       
        await _transRepo.Add(new Transaction
        {
            AccountId = sender.Id,
            Amount = dto.Amount,
            Type = "transfer",
            Description = $"Sent to Account {receiver.Id}. {dto.Description}"
        });

      
        await _transRepo.Add(new Transaction
        {
            AccountId = receiver.Id,
            Amount = dto.Amount,
            Type = "transfer",
            Description = $"Received from Account {sender.Id}. {dto.Description}"
        });
    }

  
    public async Task<List<TransactionResponseDto>> GetTransactionsByAccount(int accountId)
    {
        var account = await _accRepo.GetById(accountId);

        if (account == null)
            throw new Exception("Account not found");

        var transactions = (await _transRepo.GetByAccount(accountId))
            .OrderBy(t => t.Date)
            .ToList();

        decimal balance = 0;

        var result = transactions.Select(t =>
        {
            if (t.Type == "deposit" || t.Description.Contains("Received"))
                balance += t.Amount;
            else
                balance -= t.Amount;

            return new TransactionResponseDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type,
                Description = t.Description,
                Date = t.Date,
                RunningBalance = balance
            };
        }).ToList();

        return result;
    }

    public async Task<object> GetPagedFiltered(
        int accountId, int page, int size, string type, string sort)
    {
        var account = await _accRepo.GetById(accountId);

        if (account == null)
            throw new Exception("Account not found");

        var allTransactions = (await _transRepo.GetByAccount(accountId))
            .OrderBy(t => t.Date)
            .ToList();

        decimal balance = 0;

        var runningList = allTransactions.Select(t =>
        {
            if (t.Type == "deposit" || t.Description.Contains("Received"))
                balance += t.Amount;
            else
                balance -= t.Amount;

            return new TransactionResponseDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type,
                Description = t.Description,
                Date = t.Date,
                RunningBalance = balance
            };
        }).ToList();

        if (!string.IsNullOrWhiteSpace(type))
        {
            if (type.ToLower() == "transfer")
            {
                runningList = runningList
                    .Where(t => t.Type.ToLower().Contains("transfer"))
                    .ToList();
            }
            else
            {
                runningList = runningList
                    .Where(t => t.Type.ToLower() == type.ToLower())
                    .ToList();
            }
        }

        runningList = sort?.ToLower() == "asc"
            ? runningList.OrderBy(t => t.Date).ToList()
            : runningList.OrderByDescending(t => t.Date).ToList();

        var total = runningList.Count;

        var data = runningList
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return new
        {
            page,
            size,
            totalRecords = total,
            data
        };
    }
}