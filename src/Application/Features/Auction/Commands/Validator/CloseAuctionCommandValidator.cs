using Auction.Application.Common.Validation;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Vehicle.Abstractions;

using FluentValidation;

namespace Auction.Application.Features.Auction.Commands.Validator;

public class CloseAuctionCommandValidator : AbstractValidator<CloseAuctionCommand>
{
    public CloseAuctionCommandValidator(IVehicleRepository vehicleRepository)
    {
        RuleFor(x => x.Vin)
            .NotEmpty()
            .MustExistInVehicles(vehicleRepository);
    }
}
