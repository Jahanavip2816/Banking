using System.ComponentModel.DataAnnotations;

public class TransferDto
{
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    public int? ReceiverAccountId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
    public decimal Amount { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "PIN is required")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be exactly 4 digits")]
    public string Pin { get; set; }
}