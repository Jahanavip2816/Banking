using System.ComponentModel.DataAnnotations;

public class AccountDto
{
    [Required]
    public string AccountHolderName { get; set; } = null!;

    [Required]
    public string Phone { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}