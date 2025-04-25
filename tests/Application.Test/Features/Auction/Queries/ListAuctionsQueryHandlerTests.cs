using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Queries.Handler;
using Auction.Application.Features.Auction.Queries.Query;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Domain.Entities;

namespace Application.Test.Features.Auction.Queries;

public class ListAuctionsQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

    [Fact]
    public async Task Handle_ShouldReturnAuctions_WhenFound()
    {
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        var vehicleRepo = _fixture.Freeze<Mock<IVehicleRepository>>();
        var auctions = _fixture.CreateMany<AuctionEntity>(2);
        auctionRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([.. auctions]);
        vehicleRepo.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<BaseVehicle>());
        var handler = new ListAuctionsQueryHandler(auctionRepo.Object, vehicleRepo.Object);
        // Explicitly set all filters to null to avoid filtering out results
        var query = new ListAuctionsQuery(Status: null, VehicleType: null, Manufacturer: null, Model: null, Year: null);
        var result = await handler.Handle(query, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoAuctionsFound()
    {
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        var vehicleRepo = _fixture.Freeze<Mock<IVehicleRepository>>();
        auctionRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<AuctionEntity>());
        vehicleRepo.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<BaseVehicle>());
        var handler = new ListAuctionsQueryHandler(auctionRepo.Object, vehicleRepo.Object);
        var query = _fixture.Create<ListAuctionsQuery>();
        var result = await handler.Handle(query, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldBeEmpty();
    }
}