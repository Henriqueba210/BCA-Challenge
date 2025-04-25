using System.Net.Http.Json;

using Auction.Api.Contracts;

using Shouldly;

using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApi.Integration.Test.Endpoints;

public class AuctionEndpointsTests(AuctionApiFixture factory) : IClassFixture<AuctionApiFixture>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task StartAuction_WithValidVehicle_ReturnsCreated()
    {
        // Arrange
        var vehicle = new VehicleRequestDto
        {
            Vin = "STARTAUC1",
            Type = "SUV",
            Manufacturer = "Jeep",
            Model = "Compass",
            Year = 2023,
            StartingBid = 25000,
            NumberOfSeats = 5
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);

        var startAuction = new { Vin = "STARTAUC1" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auction/start", startAuction);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task StartAuction_WithNonexistentVehicle_ReturnsNotFound()
    {
        var startAuction = new { Vin = "NOTFOUNDVIN" };

        var response = await _client.PostAsJsonAsync("/api/auction/start", startAuction);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task StartAuction_WhenAlreadyActive_ReturnsBadRequest()
    {
        // Arrange
        var vehicle = new VehicleRequestDto
        {
            Vin = "ACTIVEVIN1",
            Type = "Truck",
            Manufacturer = "Volvo",
            Model = "FH",
            Year = 2022,
            StartingBid = 50000,
            LoadCapacity = 10000
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);
        var startAuction = new { Vin = "ACTIVEVIN1" };
        await _client.PostAsJsonAsync("/api/auction/start", startAuction);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auction/start", startAuction);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CloseAuction_WithActiveAuction_ReturnsOk()
    {
        // Arrange
        var vehicle = new VehicleRequestDto
        {
            Vin = "CLOSEAUC1",
            Type = "Sedan",
            Manufacturer = "Audi",
            Model = "A4",
            Year = 2022,
            StartingBid = 35000,
            NumberOfDoors = 4
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);
        var startAuction = new { Vin = "CLOSEAUC1" };
        var auctionStart = await _client.PostAsJsonAsync("/api/auction/start", startAuction);
        
        auctionStart.StatusCode.ShouldBe(HttpStatusCode.Created);

        var closeAuction = new { Vin = "CLOSEAUC1" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auction/close", closeAuction);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CloseAuction_WhenAuctionNotActive_ReturnsBadRequest()
    {
        var closeAuction = new { Vin = "NOACTIVEAUC" };

        var response = await _client.PostAsJsonAsync("/api/auction/close", closeAuction);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CloseAuction_WithNonexistentVehicle_ReturnsNotFound()
    {
        var closeAuction = new { Vin = "NOTFOUNDVIN2" };

        var response = await _client.PostAsJsonAsync("/api/auction/close", closeAuction);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task StartAuction_AllTypes_ReturnsAuctionWithVehicleFields()
    {
        // Sedan
        var sedan = new VehicleRequestDto
        {
            Vin = "AUCSEDAN1",
            Type = "Sedan",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2022,
            StartingBid = 18000,
            NumberOfDoors = 4
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", sedan);
        var sedanAuction = new { Vin = "AUCSEDAN1" };
        var sedanAuctionResponse = await _client.PostAsJsonAsync("/api/auction/start", sedanAuction);
        sedanAuctionResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var sedanAuctionResult = await sedanAuctionResponse.Content.ReadFromJsonAsync<AuctionResponseDto>();
        sedanAuctionResult.ShouldNotBeNull();
        sedanAuctionResult.Vehicle.Vin.ShouldBe("AUCSEDAN1");
        sedanAuctionResult.Vehicle.Type.ShouldBe("Sedan");
        sedanAuctionResult.Vehicle.NumberOfDoors.ShouldBe(4);
        sedanAuctionResult.Vehicle.Manufacturer.ShouldBe("Toyota");
        sedanAuctionResult.Vehicle.Model.ShouldBe("Corolla");
        sedanAuctionResult.Vehicle.Year.ShouldBe(2022);
        sedanAuctionResult.Vehicle.StartingBid.ShouldBe(18000);

        // SUV
        var suv = new VehicleRequestDto
        {
            Vin = "AUCSUV1",
            Type = "SUV",
            Manufacturer = "Ford",
            Model = "Explorer",
            Year = 2021,
            StartingBid = 32000,
            NumberOfSeats = 7
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", suv);
        var suvAuction = new { Vin = "AUCSUV1" };
        var suvAuctionResponse = await _client.PostAsJsonAsync("/api/auction/start", suvAuction);
        suvAuctionResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var suvAuctionResult = await suvAuctionResponse.Content.ReadFromJsonAsync<AuctionResponseDto>();
        suvAuctionResult.ShouldNotBeNull();
        suvAuctionResult.Vehicle.Vin.ShouldBe("AUCSUV1");
        suvAuctionResult.Vehicle.Type.ToLowerInvariant().ShouldBe("suv");
        suvAuctionResult.Vehicle.NumberOfSeats.ShouldBe(7);
        suvAuctionResult.Vehicle.Manufacturer.ShouldBe("Ford");
        suvAuctionResult.Vehicle.Model.ShouldBe("Explorer");
        suvAuctionResult.Vehicle.Year.ShouldBe(2021);
        suvAuctionResult.Vehicle.StartingBid.ShouldBe(32000);

        // Hatchback
        var hatch = new VehicleRequestDto
        {
            Vin = "AUCHATCH1",
            Type = "Hatchback",
            Manufacturer = "Fiat",
            Model = "Argo",
            Year = 2020,
            StartingBid = 12000,
            NumberOfDoors = 5
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", hatch);
        var hatchAuction = new { Vin = "AUCHATCH1" };
        var hatchAuctionResponse = await _client.PostAsJsonAsync("/api/auction/start", hatchAuction);
        hatchAuctionResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var hatchAuctionResult = await hatchAuctionResponse.Content.ReadFromJsonAsync<AuctionResponseDto>();
        hatchAuctionResult.ShouldNotBeNull();
        hatchAuctionResult.Vehicle.Vin.ShouldBe("AUCHATCH1");
        hatchAuctionResult.Vehicle.Type.ShouldBe("Hatchback");
        hatchAuctionResult.Vehicle.NumberOfDoors.ShouldBe(5);
        hatchAuctionResult.Vehicle.Manufacturer.ShouldBe("Fiat");
        hatchAuctionResult.Vehicle.Model.ShouldBe("Argo");
        hatchAuctionResult.Vehicle.Year.ShouldBe(2020);
        hatchAuctionResult.Vehicle.StartingBid.ShouldBe(12000);

        // Truck
        var truck = new VehicleRequestDto
        {
            Vin = "AUCTRUCK1",
            Type = "Truck",
            Manufacturer = "Mercedes",
            Model = "Actros",
            Year = 2023,
            StartingBid = 95000,
            LoadCapacity = 20000
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", truck);
        var truckAuction = new { Vin = "AUCTRUCK1" };
        var truckAuctionResponse = await _client.PostAsJsonAsync("/api/auction/start", truckAuction);
        truckAuctionResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var truckAuctionResult = await truckAuctionResponse.Content.ReadFromJsonAsync<AuctionResponseDto>();
        truckAuctionResult.ShouldNotBeNull();
        truckAuctionResult.Vehicle.Vin.ShouldBe("AUCTRUCK1");
        truckAuctionResult.Vehicle.Type.ShouldBe("Truck");
        truckAuctionResult.Vehicle.LoadCapacity.ShouldBe(20000);
        truckAuctionResult.Vehicle.Manufacturer.ShouldBe("Mercedes");
        truckAuctionResult.Vehicle.Model.ShouldBe("Actros");
        truckAuctionResult.Vehicle.Year.ShouldBe(2023);
        truckAuctionResult.Vehicle.StartingBid.ShouldBe(95000);
    }
}
