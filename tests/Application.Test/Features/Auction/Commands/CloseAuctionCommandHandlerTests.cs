using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Auction.Commands.Handler;
using Auction.Domain.Entities;
using Auction.Domain.Enums;

namespace Application.Test.Features.Auction.Commands;

public class CloseAuctionCommandHandlerTests
{
    private readonly IFixture _fixture;

    public CloseAuctionCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenAuctionNotFound()
    {
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        auctionRepo.Setup(r => r.GetByVehicleVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((AuctionEntity?)null);
        var handler = new CloseAuctionCommandHandler(auctionRepo.Object);
        var command = _fixture.Build<CloseAuctionCommand>().Create();
        var result = await handler.Handle(command, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenAuctionNotActive()
    {
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        auctionRepo.Setup(r => r.GetByVehicleVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_fixture.Build<AuctionEntity>().With(x => x.Status, AuctionStatus.Closed).Create());
        var handler = new CloseAuctionCommandHandler(auctionRepo.Object);
        var command = _fixture.Build<CloseAuctionCommand>().Create();
        var result = await handler.Handle(command, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAuctionClosed()
    {
        var auctionRepo = _fixture.Freeze<Mock<IAuctionRepository>>();
        auctionRepo.Setup(r => r.GetByVehicleVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_fixture.Build<AuctionEntity>().With(x => x.Status, AuctionStatus.Active).Create());
        auctionRepo.Setup(r => r.UpdateAsync(It.IsAny<AuctionEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(_fixture.Create<AuctionEntity>());
        var handler = new CloseAuctionCommandHandler(auctionRepo.Object);
        var command = _fixture.Build<CloseAuctionCommand>().Create();
        var result = await handler.Handle(command, CancellationToken.None);
        result.Status.ShouldBe(ResultStatus.Ok);
    }
}