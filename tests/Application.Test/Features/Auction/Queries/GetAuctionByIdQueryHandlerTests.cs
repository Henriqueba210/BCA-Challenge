using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Queries.Handler;
using Auction.Application.Features.Auction.Queries.Query;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Domain.Entities;

namespace Application.Test.Features.Auction.Queries;

public class GetAuctionByIdQueryHandlerTests
{
    private readonly IFixture _fixture;

    public GetAuctionByIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAuctionDoesNotExist()
    {
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        var vehicleRepo = _fixture.Freeze<Mock<IVehicleRepository>>();
        auctionRepo.Setup(r => r.GetByIdAsync(It.IsAny<System.Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((AuctionEntity?)null);
        var handler = new GetAuctionByIdQueryHandler(auctionRepo.Object, vehicleRepo.Object);
        var query = _fixture.Create<GetAuctionByIdQuery>();
        var result = await handler.Handle(query, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenVehicleNotFound()
    {
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        var vehicleRepo = _fixture.Freeze<Mock<IVehicleRepository>>();
        var auction = _fixture.Create<AuctionEntity>();
        auctionRepo.Setup(r => r.GetByIdAsync(It.IsAny<System.Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(auction);
        vehicleRepo.Setup(r => r.GetByVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((BaseVehicle?)null);
        var handler = new GetAuctionByIdQueryHandler(auctionRepo.Object, vehicleRepo.Object);
        var query = _fixture.Create<GetAuctionByIdQuery>();
        var result = await handler.Handle(query, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuction_WhenFound()
    {
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        var vehicleRepo = _fixture.Freeze<Mock<IVehicleRepository>>();
        var auction = _fixture.Create<AuctionEntity>();
        var vehicle = _fixture.Create<BaseVehicle>();
        auctionRepo.Setup(r => r.GetByIdAsync(It.IsAny<System.Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(auction);
        vehicleRepo.Setup(r => r.GetByVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);
        var handler = new GetAuctionByIdQueryHandler(auctionRepo.Object, vehicleRepo.Object);
        var query = _fixture.Create<GetAuctionByIdQuery>();
        var result = await handler.Handle(query, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
    }
}