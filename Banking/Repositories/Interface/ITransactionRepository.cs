public interface ITransactionRepository
{
    Task Add(Transaction transaction);

    Task AddRange(List<Transaction> transactions);

    Task<List<Transaction>> GetByAccount(int accountId);

    Task<(List<Transaction>, int)> GetPagedFiltered(
        int accountId, int page, int size, string type, string sort);
}