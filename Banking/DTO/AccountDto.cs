using System.ComponentModel.DataAnnotations;

public class AccountDto
{
    [Required]
    public string AccountHolderName { get; set; }

    [Required]
    public string Phone { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } 
}