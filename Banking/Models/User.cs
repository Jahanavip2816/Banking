using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]  
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}