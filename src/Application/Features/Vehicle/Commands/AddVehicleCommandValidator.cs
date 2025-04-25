using FluentValidation;
using Domain.Enums;

namespace Auction.Application.Features.Vehicle.Commands;

public class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(x => x.Vin).NotEmpty().Length(3, 32);
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(type => Enum.TryParse<VehicleType>(type, out _))
            .WithMessage("Invalid vehicle type.");
        RuleFor(x => x.Manufacturer).NotEmpty();
        RuleFor(x => x.Model).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(1900, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.StartingBid).GreaterThan(0);

        When(x => x.Type == nameof(VehicleType.Sedan) || x.Type == nameof(VehicleType.Hatchback), ()
         => RuleFor(x => x.NumberOfDoors).NotNull().InclusiveBetween(2, 5));
        When(x => x.Type == nameof(VehicleType.SUV), () => RuleFor(x => x.NumberOfSeats).NotNull().InclusiveBetween(2, 10));
        When(x => x.Type == nameof(VehicleType.Truck), () => RuleFor(x => x.LoadCapacity).NotNull().GreaterThan(0));
    }
}
