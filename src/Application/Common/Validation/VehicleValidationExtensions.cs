using System.Threading;
using System.Threading.Tasks;
using Auction.Application.Features.Vehicle.Abstractions;
using FluentValidation;

namespace Auction.Application.Common.Validation;

public static class VehicleValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustExistInVehicles<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IVehicleRepository vehicleRepository)
    {
        return ruleBuilder.MustAsync(async (vin, cancellation) =>
        {
            var vehicle = await vehicleRepository.GetByVinAsync(vin, cancellation);
            return vehicle != null;
        }).WithMessage(x => $"Vehicle with VIN '{{PropertyValue}}' does not exist.");
    }
}
