using Planademic.BLL.Services;
using Planademic.Domain;
using UnitTesting.Fakes;

namespace UnitTesting;

public class CourseServiceTests
{
    // TC-002-01: Teacher can create a new course
    [Fact]
    public async Task SavesCourseToRepository()
    {
        var fakeRepo = new FakeCourseRepository();
        var service = new CourseService(fakeRepo);

        var (success, error) = await service.CreateCourseAsync("Math 101", teacherId: 1);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(fakeRepo.AddedCourse);
        Assert.Equal("Math 101", fakeRepo.AddedCourse.Name);
        Assert.Equal(1, fakeRepo.AddedCourse.TeacherId);
    }

    // TC-002-02: A unique join code is generated upon course creation
    [Fact]
    public async Task GeneratesNonEmptyJoinCode()
    {
        var fakeRepo = new FakeCourseRepository();
        var service = new CourseService(fakeRepo);

        await service.CreateCourseAsync("Math 101", teacherId: 1);

        Assert.NotNull(fakeRepo.AddedCourse);
        Assert.NotEmpty(fakeRepo.AddedCourse.JoinCode);
    }

    [Fact]
    public async Task GeneratesDifferentJoinCodesForTwoCourses()
    {
        var fakeRepo1 = new FakeCourseRepository();
        var fakeRepo2 = new FakeCourseRepository();
        var service1 = new CourseService(fakeRepo1);
        var service2 = new CourseService(fakeRepo2);

        await service1.CreateCourseAsync("Math 101", teacherId: 1);
        await service2.CreateCourseAsync("Science 101", teacherId: 1);

        Assert.NotEqual(fakeRepo1.AddedCourse!.JoinCode, fakeRepo2.AddedCourse!.JoinCode);
    }

    // TC-003-01: Verify a Student can join a course with a valid join code
    [Fact]
    public async Task StudentCanJoinCourseWithValidJoinCode()
    {
        var fakeRepo = new FakeCourseRepository();
        var service = new CourseService(fakeRepo);
        var course = new Course { Id = 1, Name = "Any Course", JoinCode = "CorrectCode" };
        fakeRepo.CourseToReturn = course;

        var (success, error) = await service.JoinCourseAsync("CorrectCode", studentId: 2);

        Assert.True(success);
        Assert.Null(error);    
    }
    
    // TC-003-02: Verify joining a course is rejected for an invalid join code
    [Fact]
    public async Task StudentCannotJoinCourseWithInvalidJoinCode()
    {
        var fakeRepo = new FakeCourseRepository();
        var service = new CourseService(fakeRepo);
        fakeRepo.CourseToReturn = null; // Simulate invalid join code

        var (success, error) = await service.JoinCourseAsync("WrongCode", studentId: 2);

        Assert.False(success);
        Assert.NotNull(error);
    }
}
