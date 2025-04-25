using Auction.Application.Common.Validation;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Vehicle.Abstractions;

using FluentValidation;

namespace Auction.Application.Features.Auction.Commands.Validator;

public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
{
    public PlaceBidCommandValidator(IVehicleRepository vehicleRepository)
    {
        RuleFor(x => x.Vin)
            .NotEmpty()
            .MustExistInVehicles(vehicleRepository);
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Bid amount must be greater than zero.");
        RuleFor(x => x.Bidder)
            .NotEmpty().WithMessage("Bidder is required.");
    }
}
