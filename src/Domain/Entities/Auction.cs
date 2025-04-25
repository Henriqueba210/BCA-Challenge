using Domain.Common;
using Domain.ValueObjects;
using Domain.Enums;

namespace Domain.Entities;

public class Auction : BaseEntity<Guid>
{
    public Vin VehicleVin { get; set; } = default!;
    public AuctionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<Bid> Bids { get; set; } = new();
}

public class Bid : BaseEntity<Guid>
{
    public Guid AuctionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; }
    public string Bidder { get; set; } = default!;
}
