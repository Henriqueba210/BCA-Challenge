
using Auction.Application.Features.Vehicle.Common;

namespace Auction.Application.Features.Auction.Common;

public class AuctionDto
{
    public Guid Id { get; set; }
    public VehicleDto Vehicle { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime StartedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public required List<BidDto> Bids { get; set; }
}

public class BidDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; }
    public string Bidder { get; set; } = default!;
}
