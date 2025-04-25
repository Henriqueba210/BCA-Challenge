using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Application.Features.Vehicle.Queries.Handler;
using Auction.Application.Features.Vehicle.Queries.Query;
using Auction.Domain.Entities;

namespace Application.Test.Features.Vehicle.Queries;

public class SearchVehiclesQueryHandlerTests
{
    private readonly IFixture _fixture;

    public SearchVehiclesQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    }

    [Fact]
    public async Task Handle_ShouldReturnVehicles_WhenFound()
    {
        var repoMock = _fixture.Freeze<Mock<IVehicleRepository>>();
        var vehicles = _fixture.CreateMany<BaseVehicle>(3);
        repoMock.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([.. vehicles]);
        var handler = new SearchVehiclesQueryHandler(repoMock.Object);
        var query = _fixture.Create<SearchVehiclesQuery>();
        var result = await handler.Handle(query, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoVehiclesFound()
    {
        var repoMock = _fixture.Freeze<Mock<IVehicleRepository>>();
        repoMock.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BaseVehicle>());
        var handler = new SearchVehiclesQueryHandler(repoMock.Object);
        var query = _fixture.Create<SearchVehiclesQuery>();
        var result = await handler.Handle(query, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldBeEmpty();
    }
}