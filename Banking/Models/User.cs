using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]

    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[\W_]).{6,}$",
        ErrorMessage = "Password must contain at least one uppercase letter and one special character"
    )]
    public string Password { get; set; }

    [Required]
    public string SecurityQuestion { get; set; }

    [Required]
    public string SecurityAnswerHash { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}