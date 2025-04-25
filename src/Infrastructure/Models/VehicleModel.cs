using System.ComponentModel.DataAnnotations;

using Auction.Domain.Enums;

namespace Auction.Infrastructure.Models;

public class VehicleEntity
{
    public int Id { get; set; } // Surrogate primary key

    [Required]
    public string Vin { get; set; } = default!;
    public VehicleType Type { get; set; }
    public string Manufacturer { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public decimal StartingBid { get; set; }
}

public class SedanEntity : VehicleEntity
{
    public int NumberOfDoors { get; set; }
}

public class HatchbackEntity : VehicleEntity
{
    public int NumberOfDoors { get; set; }
}

public class SuvEntity : VehicleEntity
{
    public int NumberOfSeats { get; set; }
}

public class TruckEntity : VehicleEntity
{
    public double LoadCapacity { get; set; }
}
