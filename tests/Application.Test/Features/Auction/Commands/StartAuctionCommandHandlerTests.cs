using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Auction.Commands.Handler;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Domain.Entities;
using Auction.Domain.Enums;

namespace Application.Test.Features.Auction.Commands;

public class StartAuctionCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

    [Fact]
    public async Task Handle_ShouldReturnError_WhenActiveAuctionExists()
    {
        var vehicleRepo = _fixture.Freeze<Mock<IVehicleRepository>>();
        vehicleRepo.Setup(r => r.GetByVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_fixture.Create<BaseVehicle>());
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        auctionRepo.Setup(r => r.GetByVehicleVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_fixture.Build<AuctionEntity>().With(x => x.Status, AuctionStatus.Active).Create());
        var handler = new StartAuctionCommandHandler(auctionRepo.Object);
        var command = _fixture.Build<StartAuctionCommand>().Create();
        var result = await handler.Handle(command, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAuctionStarted()
    {
        var vehicleRepo = _fixture.Freeze<Mock<IVehicleRepository>>();
        vehicleRepo.Setup(r => r.GetByVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_fixture.Create<BaseVehicle>());
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        auctionRepo.Setup(r => r.GetByVehicleVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((AuctionEntity?)null);
        auctionRepo.Setup(r => r.AddAsync(It.IsAny<AuctionEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(_fixture.Create<AuctionEntity>());
        var handler = new StartAuctionCommandHandler(auctionRepo.Object);
        var command = _fixture.Build<StartAuctionCommand>().Create();
        var result = await handler.Handle(command, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Ok);
    }
}