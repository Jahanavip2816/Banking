public interface IReportService
{
    Task<byte[]> GenerateCsv(int accountId);
    byte[] GeneratePdf(Account account, List<Transaction> transactions);
    byte[] GenerateReceiptPdf(Account account, List<Transaction> transactions);
}