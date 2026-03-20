using Microsoft.AspNetCore.Mvc;
using QuestPDF;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _service;
    private readonly BankingDbContext _context;

    public ReportsController(ReportService service, BankingDbContext context)
    {
        _service = service;
        _context = context;
    }

    // CSV DOWNLOAD
    [HttpGet("transactions/{accountId}/csv")]
    public async Task<IActionResult> GetCsv(int accountId)
    {
        var file = await _service.GenerateCsv(accountId);

        return File(file,
            "text/csv",
            $"transactions_Account{accountId}.csv");
    }

    // FULL REPORT PDF
    [HttpGet("transactions/{accountId}/pdf")]
    public async Task<IActionResult> GetPdf(int accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);

        if (account == null)
            return NotFound("Account not found");

        var transactions = _context.Transactions
            .Where(t => t.AccountId == accountId)
            .ToList();

        var pdf = _service.GeneratePdf(account, transactions);

        return File(pdf, "application/pdf",
            $"transactions_Account{accountId}.pdf");
    }
    // ATM RECEIPT PDF
    [HttpGet("transactions/{accountId}/pdf-receipt")]
    public async Task<IActionResult> GetReceipt(int accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);

        if (account == null)
            return NotFound("Account not found");

        var transactions = _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderBy(t => t.Date)
            .ToList();

        var pdf = _service.GenerateReceiptPdf(account, transactions);

        return File(pdf, "application/pdf",
            $"receipt_Account{accountId}.pdf");
    }
}
