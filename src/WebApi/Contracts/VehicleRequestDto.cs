namespace Auction.Api.Contracts;

public class VehicleRequestDto
{
    public string Vin { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Manufacturer { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public decimal StartingBid { get; set; }
    public int? NumberOfDoors { get; set; }
    public int? NumberOfSeats { get; set; }
    public double? LoadCapacity { get; set; }
}
