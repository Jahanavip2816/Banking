using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_Should_Return_Valid_Token()
    {
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(x => x["Jwt:Key"])
            .Returns("ThisIsMySuperSecureKey1234567890");

        configMock.Setup(x => x["Jwt:Issuer"])
            .Returns("TestIssuer");

        configMock.Setup(x => x["Jwt:Audience"])
            .Returns("TestAudience");

        configMock.Setup(x => x["Jwt:ExpiryMinutes"])
            .Returns("60");

        var service = new JwtService(configMock.Object);

        var user = new User
        {
            Username = "Jaanu",
            Email = "test@mail.com"
        };

        var token = service.GenerateToken(user);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    // ✅ BONUS TEST (ADD HERE)
    [Fact]
    public void GenerateToken_Should_Contain_Email_Claim()
    {
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(x => x["Jwt:Key"])
            .Returns("ThisIsMySuperSecureKey1234567890");

        configMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        configMock.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");
        configMock.Setup(x => x["Jwt:ExpiryMinutes"]).Returns("60");

        var service = new JwtService(configMock.Object);

        var user = new User
        {
            Username = "Jaanu",
            Email = "test@mail.com"
        };

        var token = service.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var emailClaim = jwtToken.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email);

        Assert.NotNull(emailClaim);
        Assert.Equal("test@mail.com", emailClaim.Value);
    }
}