namespace Auction.Application.Features.Vehicle.Common;

public class VehicleDto
{
    public string Vin { get; set; }
    public string Type { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal StartingBid { get; set; }
    public int? NumberOfDoors { get; set; }
    public int? NumberOfSeats { get; set; }
    public int? LoadCapacity { get; set; }
}
