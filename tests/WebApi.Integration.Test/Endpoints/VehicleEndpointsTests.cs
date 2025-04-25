using System.Net.Http.Json;
using Auction.Api.Contracts;
using Shouldly;
using Microsoft.AspNetCore.Mvc.Testing;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace WebApi.Integration.Test.Endpoints;

public class VehicleEndpointsTests : IClassFixture<AuctionApiFixture>
{
    private readonly HttpClient _client;

    public VehicleEndpointsTests(AuctionApiFixture factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddVehicle_WithValidData_ReturnsOk_And_DuplicateVin_ReturnsBadRequest()
    {
        // Arrange
        var vehicle = new VehicleRequestDto
        {
            Vin = "VIN12345",
            Type = "Sedan",
            Manufacturer = "Ford",
            Model = "Focus",
            Year = 2020,
            StartingBid = 10000,
            NumberOfDoors = 4
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        // Try to add the same VIN again
        var duplicateResponse = await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);

        // Assert duplicate VIN returns BadRequest
        duplicateResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        if (duplicateResponse.Content.Headers.ContentLength > 0)
        {
            var problem = await duplicateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.ShouldNotBeNull();
            problem.Status.ShouldBe(400);
            problem.Type.ShouldBe("https://tools.ietf.org/html/rfc7231#section-6.5.1");
        }
    }

    [Fact]
    public async Task AddVehicle_WithDuplicateVin_ReturnsBadRequest()
    {
        // Arrange
        var vehicle = new VehicleRequestDto
        {
            Vin = "DUPVIN1",
            Type = "Sedan",
            Manufacturer = "Honda",
            Model = "Civic",
            Year = 2021,
            StartingBid = 12000,
            NumberOfDoors = 4
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);

        // Act
        var duplicateResponse = await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);

        // Assert
        duplicateResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        if (duplicateResponse.Content.Headers.ContentLength > 0)
        {
            var problem = await duplicateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.ShouldNotBeNull();
            problem.Status.ShouldBe(400);
        }
    }

    [Fact]
    public async Task AddVehicle_WithInvalidInput_ReturnsBadRequest()
    {
        // Arrange: missing required fields
        var vehicle = new VehicleRequestDto
        {
            Vin = "", // Invalid VIN
            Type = "Sedan",
            Manufacturer = "Ford",
            Model = "Focus",
            Year = 2020,
            StartingBid = -100, // Invalid bid
            NumberOfDoors = 4
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchVehicle_WithExistingVehicle_ReturnsVehicle()
    {
        // Arrange
        var vehicle = new VehicleRequestDto
        {
            Vin = "VINSEARCH1",
            Type = "SUV",
            Manufacturer = "Toyota",
            Model = "RAV4",
            Year = 2022,
            StartingBid = 20000,
            NumberOfSeats = 5
        };
        await _client.PostAsJsonAsync("/api/auction/vehicle", vehicle);

        // Act
        var response = await _client.GetAsync($"/api/auction/vehicle?type=SUV&manufacturer=Toyota&model=RAV4&year=2022");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var vehicles = await response.Content.ReadFromJsonAsync<VehicleResponseDto[]>();
        vehicles.ShouldNotBeNull();
        vehicles.ShouldContain(v => v.Vin == "VINSEARCH1");
    }

    [Fact]
    public async Task SearchVehicle_WithNoResults_ReturnsEmptyArray()
    {
        // Act
        var response = await _client.GetAsync($"/api/auction/vehicle?type=Truck&manufacturer=NoBrand&model=NoModel&year=1999");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var vehicles = await response.Content.ReadFromJsonAsync<VehicleResponseDto[]>();
        vehicles.ShouldNotBeNull();
        vehicles.ShouldBeEmpty();
    }

    [Fact]
    public async Task SearchVehicle_WithInvalidInput_ReturnsBadRequest()
    {
        // Act: year is not a number
        var response = await _client.GetAsync($"/api/auction/vehicle?year=notanumber");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddVehicle_AllTypes_ReturnsAllFieldsCorrectly()
    {
        var sedan = new VehicleRequestDto
        {
            Vin = "SEDANVIN1",
            Type = "Sedan",
            Manufacturer = "Toyota",
            Model = "Camry",
            Year = 2022,
            StartingBid = 20000,
            NumberOfDoors = 4
        };
        var sedanResponse = await _client.PostAsJsonAsync("/api/auction/vehicle", sedan);
        sedanResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var sedanResult = await sedanResponse.Content.ReadFromJsonAsync<VehicleResponseDto>();
        sedanResult.ShouldNotBeNull();
        sedanResult.Vin.ShouldBe("SEDANVIN1");
        sedanResult.Type.ShouldBe("Sedan");
        sedanResult.NumberOfDoors.ShouldBe(4);
        sedanResult.Manufacturer.ShouldBe("Toyota");
        sedanResult.Model.ShouldBe("Camry");
        sedanResult.Year.ShouldBe(2022);
        sedanResult.StartingBid.ShouldBe(20000);

        var suv = new VehicleRequestDto
        {
            Vin = "SUVVIN1",
            Type = "SUV",
            Manufacturer = "Hyundai",
            Model = "Santa Fe",
            Year = 2021,
            StartingBid = 25000,
            NumberOfSeats = 7
        };
        var suvResponse = await _client.PostAsJsonAsync("/api/auction/vehicle", suv);
        suvResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var suvResult = await suvResponse.Content.ReadFromJsonAsync<VehicleResponseDto>();
        suvResult.ShouldNotBeNull();
        suvResult.Vin.ShouldBe("SUVVIN1");
        suvResult.Type.ShouldBe("SUV");
        suvResult.NumberOfSeats.ShouldBe(7);
        suvResult.Manufacturer.ShouldBe("Hyundai");
        suvResult.Model.ShouldBe("Santa Fe");
        suvResult.Year.ShouldBe(2021);
        suvResult.StartingBid.ShouldBe(25000);

        var hatchback = new VehicleRequestDto
        {
            Vin = "HATCHVIN1",
            Type = "Hatchback",
            Manufacturer = "Volkswagen",
            Model = "Golf",
            Year = 2019,
            StartingBid = 15000,
            NumberOfDoors = 5
        };
        var hatchResponse = await _client.PostAsJsonAsync("/api/auction/vehicle", hatchback);
        hatchResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var hatchResult = await hatchResponse.Content.ReadFromJsonAsync<VehicleResponseDto>();
        hatchResult.ShouldNotBeNull();
        hatchResult.Vin.ShouldBe("HATCHVIN1");
        hatchResult.Type.ShouldBe("Hatchback");
        hatchResult.NumberOfDoors.ShouldBe(5);
        hatchResult.Manufacturer.ShouldBe("Volkswagen");
        hatchResult.Model.ShouldBe("Golf");
        hatchResult.Year.ShouldBe(2019);
        hatchResult.StartingBid.ShouldBe(15000);

        var truck = new VehicleRequestDto
        {
            Vin = "TRUCKVIN1",
            Type = "Truck",
            Manufacturer = "Scania",
            Model = "R500",
            Year = 2020,
            StartingBid = 80000,
            LoadCapacity = 18000
        };
        var truckResponse = await _client.PostAsJsonAsync("/api/auction/vehicle", truck);
        truckResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var truckResult = await truckResponse.Content.ReadFromJsonAsync<VehicleResponseDto>();
        truckResult.ShouldNotBeNull();
        truckResult.Vin.ShouldBe("TRUCKVIN1");
        truckResult.Type.ShouldBe("Truck");
        truckResult.LoadCapacity.ShouldBe(18000);
        truckResult.Manufacturer.ShouldBe("Scania");
        truckResult.Model.ShouldBe("R500");
        truckResult.Year.ShouldBe(2020);
        truckResult.StartingBid.ShouldBe(80000);
    }
}
