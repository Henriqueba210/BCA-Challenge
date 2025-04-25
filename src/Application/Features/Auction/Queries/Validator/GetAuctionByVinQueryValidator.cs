using Auction.Application.Common.Validation;
using Auction.Application.Features.Auction.Queries.Query;
using Auction.Application.Features.Vehicle.Abstractions;

using FluentValidation;

namespace Auction.Application.Features.Auction.Queries.Validator;

public class GetAuctionByVinQueryValidator : AbstractValidator<GetAuctionByVinQuery>
{
    public GetAuctionByVinQueryValidator(IVehicleRepository vehicleRepository)
    {
        RuleFor(x => x.Vin)
            .NotEmpty()
            .MustExistInVehicles(vehicleRepository);
    }
}
