using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;

public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_Should_Return_Token()
    {
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(x => x["Jwt:Key"])
                  .Returns("ThisIsMySuperSecureKey1234567890");

        configMock.Setup(x => x["Jwt:Issuer"])
                  .Returns("TestIssuer");

        configMock.Setup(x => x["Jwt:Audience"])
                  .Returns("TestAudience");

        var service = new JwtService(configMock.Object);

        var token = service.GenerateToken("Jaanu");

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }
}