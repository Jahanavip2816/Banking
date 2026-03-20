namespace Banking.DTO
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
