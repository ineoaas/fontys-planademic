using Planademic.BLL.Services;
using Planademic.Domain;
using UnitTesting.Fakes;

namespace UnitTesting;

public class UserServiceTests
{
    // TC-001-04: Login rejected for invalid credentials (user not found)
    [Fact]
    public async Task ReturnNullWhenUserNotFound()
    {
        var fakeRepo = new FakeUserRepository();
        fakeRepo.UserToReturn = null;

        var userService = new UserService(fakeRepo);
        var result = await userService.ValidateLoginAsync("notfound@test.com", "pass", "Student");

        Assert.Null(result);
    }

    // TC-001-04: Login rejected for invalid credentials (wrong role)
    [Fact]
    public async Task ReturnNullWhenRoleDoesNotMatch()
    {
        var fakeRepo = new FakeUserRepository();
        fakeRepo.UserToReturn = new User { Email = "a@test.com", PasswordHash = "pass", Role = "Student" };

        var userService = new UserService(fakeRepo);
        var result = await userService.ValidateLoginAsync("a@test.com", "pass", "Teacher");

        Assert.Null(result);
    }

    // TC-001-04: Login rejected for invalid credentials (wrong password)
    [Fact]
    public async Task ReturnNullWhenPasswordIsWrong()
    {
        var fakeRepo = new FakeUserRepository();
        fakeRepo.UserToReturn = new User { Email = "a@test.com", PasswordHash = "correct", Role = "Student" };

        var userService = new UserService(fakeRepo);
        var result = await userService.ValidateLoginAsync("a@test.com", "wrong", "Student");

        Assert.Null(result);
    }

    // TC-001-03: Registered user can log in with correct credentials
    [Fact]
    public async Task ReturnUserWhenCredentialsAreValid()
    {
        var fakeRepo = new FakeUserRepository();
        fakeRepo.UserToReturn = new User { Email = "a@test.com", PasswordHash = "correct", Role = "Student" };

        var userService = new UserService(fakeRepo);
        var result = await userService.ValidateLoginAsync("a@test.com", "correct", "Student");

        Assert.NotNull(result);
        Assert.Equal("a@test.com", result.Email);
    }

    // TC-001-01: New student can register with valid credentials
    [Fact]
    public async Task ReturnsErrorWhenEmailAlreadyExists()
    {
        var fakeRepo = new FakeUserRepository();
        fakeRepo.EmailExists = true;

        var userService = new UserService(fakeRepo);
        var (success, error, user) = await userService.RegisterAsync("taken@test.com", "pass", "Jan", "Jansen");

        Assert.False(success);
        Assert.NotNull(error);
        Assert.Null(user);
    }

    // TC-001-01: New student can register with valid credentials
    [Fact]
    public async Task ReturnsUserWhenEmailIsNew()
    {
        var fakeRepo = new FakeUserRepository();
        fakeRepo.EmailExists = false;

        var userService = new UserService(fakeRepo);
        var (success, error, user) = await userService.RegisterAsync("new@test.com", "pass", "Jan", "Jansen");

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(user);
        Assert.Equal("new@test.com", user.Email);
    }

    // TC-001-01: New student can register with valid credentials
    [Fact]
    public async Task SavesUserToRepository()
    {
        var fakeRepo = new FakeUserRepository();
        fakeRepo.EmailExists = false;

        var userService = new UserService(fakeRepo);
        await userService.RegisterAsync("new@test.com", "pass", "Jan", "Jansen");

        Assert.NotNull(fakeRepo.AddedUser);
        Assert.Equal("new@test.com", fakeRepo.AddedUser.Email);
    }

    // TC-001-01: New student can register with valid credentials
    [Fact]
    public async Task AssignStudentRolle()
    {
        var fakeRepo = new FakeUserRepository();
        fakeRepo.EmailExists = false;

        var userService = new UserService(fakeRepo);
        var (success, error, user) = await userService.RegisterAsync("student@test.com", "pass", "Jan", "Jansen");

        Assert.True(success);
        Assert.Equal("Student", user?.Role);
    }
}
