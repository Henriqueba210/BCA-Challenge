using System.Net;
using System.Net.Http.Json;
using Auction.Api.Contracts;
using Shouldly;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace WebApi.Integration.Test.Endpoints;

public class BidEndpointsTests : IClassFixture<AuctionApiFixture>
{
    private readonly HttpClient _client;

    public BidEndpointsTests(AuctionApiFixture factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PlaceBid_WithValidBid_ReturnsCreated_And_ResponseBodyIsAuction()
    {
        var vehicle = new VehicleRequestDto
        {
            Vin = "BIDVIN2",
            Type = "Sedan",
            Manufacturer = "BMW",
            Model = "320i",
            Year = 2021,
            StartingBid = 30000,
            NumberOfDoors = 4
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);
        var startAuction = new { Vin = "BIDVIN2" };
        await _client.PostAsJsonAsync("/api/auction/start", startAuction);

        var bid = new { Vin = "BIDVIN2", Bidder = "Jon Doe", Amount = 31000 };
        var response = await _client.PostAsJsonAsync("/api/bid", bid);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var auction = await response.Content.ReadFromJsonAsync<AuctionResponseDto>();
        auction.ShouldNotBeNull();
        auction.Status.ShouldBe("Active");
        auction.Bids.ShouldNotBeNull();
        auction.Bids.Count.ShouldBe(1);
        auction.Bids[0].Amount.ShouldBe(31000);
        auction.Vehicle.Vin.ShouldBe("BIDVIN2");
        auction.Vehicle.Type.ShouldBe("Sedan");
    }

    [Fact]
    public async Task PlaceBid_WhenAuctionNotActive_ReturnsBadRequest_WithProblemDetails()
    {
        var bid = new { Vin = "NOAUCVIN", Amount = 1000 };
        var response = await _client.PostAsJsonAsync("/api/bid", bid);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        if (response.Content.Headers.ContentLength > 0)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.ShouldNotBeNull();
            problem.Status.ShouldBe(400);
        }
    }

    [Fact]
    public async Task PlaceBid_WithTooLowAmount_ReturnsBadRequest_WithProblemDetails()
    {
        var vehicle = new VehicleRequestDto
        {
            Vin = "LOWBIDVIN2",
            Type = "SUV",
            Manufacturer = "Kia",
            Model = "Sportage",
            Year = 2020,
            StartingBid = 15000,
            NumberOfSeats = 5
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);
        var startAuction = new { Vin = "LOWBIDVIN2" };
        await _client.PostAsJsonAsync("/api/auction/start", startAuction);

        var bid = new { Vin = "LOWBIDVIN2", Amount = 10000 };
        var response = await _client.PostAsJsonAsync("/api/bid", bid);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        if (response.Content.Headers.ContentLength > 0)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.ShouldNotBeNull();
            problem.Status.ShouldBe(400);
        }
    }

    [Fact]
    public async Task PlaceBid_WithInvalidInput_ReturnsBadRequest_WithProblemDetails()
    {
        var bid = new { Vin = "", Amount = -1 };
        var response = await _client.PostAsJsonAsync("/api/bid", bid);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        if (response.Content.Headers.ContentLength > 0)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.ShouldNotBeNull();
            problem.Status.ShouldBe(400);
        }
    }
}
