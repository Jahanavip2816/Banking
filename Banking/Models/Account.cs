using System.ComponentModel.DataAnnotations;
using System.Transactions;

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
    [Phone]
    public string Phone { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal Balance { get; set; } = 0;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}