using Banking.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ITransactionService
{
    Task Deposit(TransferDto dto);
    Task Withdraw(TransferDto dto);
    Task Transfer(TransferDto dto);
    Task<List<TransactionResponseDto>> GetTransactionsByAccount(int accountId);
    Task<object> GetPagedFiltered(int accountId, int page, int size, string type, string sort);
}