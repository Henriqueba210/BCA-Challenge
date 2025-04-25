using Auction.Domain.Enums;

namespace Auction.Domain.Entities;

public class AuctionEntity
{
    public Guid Id { get; set; }
    public string VehicleVin { get; set; } = default!;
    public BaseVehicle Vehicle { get; set; } = default!;
    public AuctionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public required List<Bid> Bids { get; set; }
}

public class Bid
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; }
    public string Bidder { get; set; } = default!;
}
