using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
    public decimal Amount { get; set; }

    [Required]
    public string Type { get; set; }

    [StringLength(250)]
    public string Description { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [NotMapped]
    [Required(ErrorMessage = "PIN is required for transactions")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be exactly 4 digits")]
    public string Pin { get; set; }
}