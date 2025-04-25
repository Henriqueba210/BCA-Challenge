using System.ComponentModel.DataAnnotations.Schema;

namespace Auction.Infrastructure.Models;

public class AuctionEntity
{
    public Guid Id { get; set; }
    public string VehicleVin { get; set; } = default!;
    [ForeignKey("VehicleVin")]
    public VehicleEntity Vehicle { get; set; } = default!;
    public int Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<BidEntity> Bids { get; set; } = new();
}

public class BidEntity
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; }
    public string Bidder { get; set; } = default!;
    public AuctionEntity Auction { get; set; } = default!;
}
