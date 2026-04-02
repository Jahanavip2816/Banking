using System.ComponentModel.DataAnnotations;

public class AccountDto
{
    [Required]
    public string AccountHolderName { get; set; }

    [Required]
    [RegularExpression(@"^[0-9+\-]+$",
        ErrorMessage = "Phone must contain only digits, '+' or '-' characters.")]
    public string Phone { get; set; }

    [Required]
    [MinLength(6)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter and one special character.")]
    public string Password { get; set; }

    [Required]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be exactly 4 digits.")]
    public string Pin { get; set; }
}