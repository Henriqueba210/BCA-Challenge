using Auction.Application.Common.Validation;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Domain.Entities;

using FluentValidation;

namespace Application.Test.Common.Validation.Extensions;

public class DummyCommand
{
    public string Vin { get; set; } = string.Empty;
}

internal class DummyCommandValidator : AbstractValidator<DummyCommand>
{
    public DummyCommandValidator(IVehicleRepository vehicleRepository)
    {
        RuleFor(x => x.Vin).MustExistInVehicles(vehicleRepository);
    }
}

public class VehicleValidationExtensionsTests
{
    [Fact]
    public async Task MustExistInVehicles_Should_Fail_When_Vehicle_Not_Found()
    {
        var repo = new Mock<IVehicleRepository>();
        repo.Setup(r => r.GetByVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((BaseVehicle?)null);
        var validator = new DummyCommandValidator(repo.Object);

        var result = await validator.ValidateAsync(new DummyCommand { Vin = "NOTFOUND" });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("does not exist"));
    }

    [Fact]
    public async Task MustExistInVehicles_Should_Pass_When_Vehicle_Exists()
    {
        var repo = new Mock<IVehicleRepository>();
        repo.Setup(r => r.GetByVinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new BaseVehicle());
        var validator = new DummyCommandValidator(repo.Object);

        var result = await validator.ValidateAsync(new DummyCommand { Vin = "EXISTS" });

        result.IsValid.ShouldBeTrue();
    }
}