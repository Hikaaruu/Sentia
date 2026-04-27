using AutoMapper;
using FluentAssertions;
using Sentia.Application.Features.Users.Queries.GetAllUsers;
using Sentia.Application.Mappings;
using Sentia.Application.UnitTests.Infrastructure;
using Sentia.Domain.Entities;

namespace Sentia.Application.UnitTests.Features.Users.Queries;

public class GetAllUsersQueryHandlerTests
{
    private static IMapper CreateMapper() =>
        new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>()).CreateMapper();

    private static GetAllUsersQueryHandler CreateSut(TestApplicationDbContext context) =>
        new(context, CreateMapper());

    private static User MakeUser(string id, string userName) =>
        new() { Id = id, UserName = userName };

    [Fact]
    public async Task Handle_ReturnsUsersExcludingCurrentUser()
    {
        await using var context = TestApplicationDbContext.Create();
        context.Users.AddRange(
            MakeUser("current", "current_user"),
            MakeUser("user-2", "alice"),
            MakeUser("user-3", "bob"));
        await context.SaveChangesAsync(CancellationToken.None);
        var query = new GetAllUsersQuery(Page: 1, PageSize: 10, CurrentUserId: "current");

        var result = await CreateSut(context).Handle(query, CancellationToken.None);

        result.Items.Should().NotContain(u => u.Id == "current");
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedTotalCount()
    {
        await using var context = TestApplicationDbContext.Create();
        context.Users.AddRange(
            MakeUser("current", "current_user"),
            MakeUser("user-2", "alice"),
            MakeUser("user-3", "bob"),
            MakeUser("user-4", "charlie"));
        await context.SaveChangesAsync(CancellationToken.None);
        var query = new GetAllUsersQuery(Page: 1, PageSize: 10, CurrentUserId: "current");

        var result = await CreateSut(context).Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_Page2_SkipsFirstPageItems()
    {
        await using var context = TestApplicationDbContext.Create();
        context.Users.AddRange(
            MakeUser("current", "current_user"),
            MakeUser("u2", "alice"),
            MakeUser("u3", "bob"),
            MakeUser("u4", "charlie"),
            MakeUser("u5", "dave"));
        await context.SaveChangesAsync(CancellationToken.None);
        var query = new GetAllUsersQuery(Page: 2, PageSize: 2, CurrentUserId: "current");

        var result = await CreateSut(context).Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.Should().NotContain(u => u.Id == "current");
    }

    [Fact]
    public async Task Handle_UsersReturnedOrderedByUsername()
    {
        await using var context = TestApplicationDbContext.Create();
        context.Users.AddRange(
            MakeUser("current", "current_user"),
            MakeUser("u3", "charlie"),
            MakeUser("u1", "alice"),
            MakeUser("u2", "bob"));
        await context.SaveChangesAsync(CancellationToken.None);
        var query = new GetAllUsersQuery(Page: 1, PageSize: 10, CurrentUserId: "current");

        var result = await CreateSut(context).Handle(query, CancellationToken.None);

        result.Items.Select(u => u.UserName)
            .Should().BeInAscendingOrder();
    }
}
