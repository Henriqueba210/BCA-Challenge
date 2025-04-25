using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Application.Features.Vehicle.Commands.Command;
using Auction.Application.Features.Vehicle.Commands.Handler;
using Auction.Domain.Entities;

namespace Application.Test.Features.Vehicle.Commands;

public class AddVehicleCommandHandlerTests
{
    private readonly IFixture _fixture;

    public AddVehicleCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenVehicleWithVinExists()
    {
        // Arrange
        var repoMock = _fixture.Freeze<Mock<IVehicleRepository>>();
        var existingVehicle = _fixture.Build<Sedan>()
            .With(x => x.Vin, "VIN123") // Use Vin value object for Id
            .Create();
        repoMock.Setup(r => r.GetByVinAsync("VIN123", It.IsAny<CancellationToken>())).ReturnsAsync(existingVehicle);
        var handler = new AddVehicleCommandHandler(repoMock.Object);
        var command = _fixture.Build<AddVehicleCommand>()
            .With(x => x.Vin, "VIN123")
            .With(x => x.Type, "Sedan")
            .Create();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenVehicleIsAdded()
    {
        // Arrange
        var repoMock = _fixture.Freeze<Mock<IVehicleRepository>>();
        repoMock.Setup(r => r.GetByVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((BaseVehicle?)null);
        repoMock.Setup(r => r.AddAsync(It.IsAny<BaseVehicle>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Sedan());
        var handler = new AddVehicleCommandHandler(repoMock.Object);
        var command = _fixture.Build<AddVehicleCommand>()
            .With(x => x.Type, "Sedan")
            .Create();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
    }
}