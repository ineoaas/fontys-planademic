using Planademic.BLL.Services;
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
}
