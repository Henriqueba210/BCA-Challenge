using System;
using System.Collections.Generic;

namespace Auction.Api.Contracts;

public class AuctionResponseDto
{
    public Guid Id { get; set; }
    public VehicleResponseDto Vehicle { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime StartedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<BidResponseDto> Bids { get; set; } = new();
}

public class BidResponseDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; }
    public string Bidder { get; set; } = default!;
}
