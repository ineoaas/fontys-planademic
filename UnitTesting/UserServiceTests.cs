using Moq;
using Planademic.BLL.Services;
using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace UnitTesting;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_repoMock.Object);
    }

    [Fact]
    public async Task ValidateLogin_ReturnsNull_WhenUserNotFound()
    {
        _repoMock.Setup(r => r.GetByEmailAsync("notfound@test.com"))
                 .ReturnsAsync((User?)null);

        var result = await _sut.ValidateLoginAsync("notfound@test.com", "pass", "Student");

        Assert.Null(result);
    }
}
