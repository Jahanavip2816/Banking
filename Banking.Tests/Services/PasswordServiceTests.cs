using Xunit;

public class PasswordServiceTests
{
    [Fact]
    public void HashPassword_Should_Not_Be_Equal_To_Input()
    {
        var service = new PasswordService();

        var hash = service.HashPassword("123456");

        Assert.NotEqual("123456", hash);
    }

    [Fact]
    public void VerifyPassword_Should_Return_True()
    {
        var service = new PasswordService();

        var hash = service.HashPassword("123456");

        var result = service.VerifyPassword("123456", hash);

        Assert.True(result);
    }
}