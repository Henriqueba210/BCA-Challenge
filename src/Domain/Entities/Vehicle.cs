using Domain.Common;
using Domain.ValueObjects;
using Domain.Enums;

namespace Domain.Entities;

public abstract class BaseVehicle : BaseEntity<Vin>
{
    public VehicleType Type { get; protected set; }
    public string Manufacturer { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public decimal StartingBid { get; set; }
}

public class Sedan : BaseVehicle
{
    public int NumberOfDoors { get; set; }
    public Sedan() { Type = VehicleType.Sedan; }
}

public class Hatchback : BaseVehicle
{
    public int NumberOfDoors { get; set; }
    public Hatchback() { Type = VehicleType.Hatchback; }
}

public class SUV : BaseVehicle
{
    public int NumberOfSeats { get; set; }
    public SUV() { Type = VehicleType.SUV; }
}

public class Truck : BaseVehicle
{
    public double LoadCapacity { get; set; }
    public Truck() { Type = VehicleType.Truck; }
}
