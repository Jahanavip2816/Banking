using System.ComponentModel.DataAnnotations;

public class Account
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Account holder name is required")]
    [StringLength(100)]
    public string AccountHolderName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    [RegularExpression(@"^\+?[0-9\-]+$",
        ErrorMessage = "Phone must contain digits, optional '+' at start, and '-' only.")]
    public string Phone { get; set; }

    [Required]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be exactly 4 digits.")]
    public string? PinHash { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Balance { get; set; } = 0;

    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}