using System.ComponentModel.DataAnnotations;

public class TransactionDto
{
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public string  Description { get; set; }
}