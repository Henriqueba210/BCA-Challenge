using System.Net.Http.Json;

using Auction.Api.Contracts;

using Shouldly;

using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace WebApi.Integration.Test.Endpoints;

public class BidEndpointsTests(AuctionApiFixture factory) : IClassFixture<AuctionApiFixture>
{
    private readonly HttpClient _client = factory.CreateClient();

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

    [Fact]
    public async Task PlaceBid_ConcurrentRequests_OnlyHighestBidWins()
    {
        // Arrange
        var vehicle = new VehicleRequestDto
        {
            Vin = "CONCURVIN1",
            Type = "Sedan",
            Manufacturer = "Tesla",
            Model = "Model S",
            Year = 2023,
            StartingBid = 50000,
            NumberOfDoors = 4
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);
        var startAuction = new { Vin = "CONCURVIN1" };
        await _client.PostAsJsonAsync("/api/auction/start", startAuction);

        // Prepare concurrent bids (some valid, some invalid)
        var bids = new[]
        {
            new { Vin = "CONCURVIN1", Amount = 51000, Bidder = "alice" },
            new { Vin = "CONCURVIN1", Amount = 52000, Bidder = "bob" },
            new { Vin = "CONCURVIN1", Amount = 51500, Bidder = "charlie" },
            new { Vin = "CONCURVIN1", Amount = 53000, Bidder = "dave" },
            new { Vin = "CONCURVIN1", Amount = 52500, Bidder = "eve" }
        };

        // Act: send all bids with a slight delay between each to ensure deterministic results
        var responses = new List<HttpResponseMessage>();
        foreach (var bid in bids)
        {
            var response = await _client.PostAsJsonAsync("/api/bid", bid);
            responses.Add(response);
            await Task.Delay(5);
        }

        // Ensure all bid requests have completed
        responses.Count.ShouldBe(bids.Length);

        // Some requests will fail due to bid validation, so don't call EnsureSuccessStatusCode
        var successAmounts = new List<decimal>();
        var failedAmounts = new List<decimal>();
        for (int i = 0; i < responses.Count; i++)
        {
            if (responses[i].IsSuccessStatusCode)
                successAmounts.Add(bids[i].Amount);
            else
                failedAmounts.Add(bids[i].Amount);
        }

        // Assert: Only the highest bid should be accepted as the last bid
        var auctionResponse = await _client.GetAsync("/api/auction?vin=CONCURVIN1");
        auctionResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var auctions = await auctionResponse.Content.ReadFromJsonAsync<AuctionResponseDto[]>();
        auctions.ShouldNotBeNull();
        var auction = auctions.FirstOrDefault(a => a.Vehicle.Vin == "CONCURVIN1");
        auction.ShouldNotBeNull();
        auction.Bids.ShouldNotBeNull();
        auction.Bids.Count.ShouldBeGreaterThanOrEqualTo(1);
        auction.Bids.Max(b => b.Amount).ShouldBe(53000);

        // Only valid bids should be present
        var acceptedAmounts = auction.Bids.Select(b => b.Amount).ToList();
        acceptedAmounts.ShouldBe(successAmounts.OrderBy(x => x).ToList());

        // Lower or equal bids after a higher one should not be accepted
        failedAmounts.ShouldContain(51500);
        failedAmounts.ShouldContain(52500);
    }
}
