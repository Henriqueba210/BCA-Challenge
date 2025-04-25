# BCA-Challenge
Problem Statement: Car Auction Management System

# BCA Requirements

You are tasked with implementing a simple Car Auction Management System. The system should handle different types of vehicles: Sedans, SUVs, Hatchbacks and Trucks.
Each of these types has different attributes:
• Hatchback: Number of doors, manufacturer, model, year, and starting bid.
• Sudan: Number of doors, manufacturer, model, year, and starting bid.
• SUV: Number of seats, manufacturer, model, year, and starting bid.
• Truck: Load capacity, manufacturer, model, year, and starting bid.

The system should allow users to:
1. Add vehicles to the auction inventory. Each vehicle has a type (Sedan, SUV, or Truck), a unique identifier, and respective attributes based on its type.
2. Search for vehicles by type, manufacturer, model, or year. The search should return all available vehicles that match the search criteria.
3. Start and close auctions for vehicles. Only one auction can be active for a vehicle at a time. Users should be able to place bids on the vehicle during an active auction.
4. Implement error handling for the following scenarios:

* a) When adding a vehicle, ensure that the unique identifier is not already in use by another vehicle in the inventory. Raise an appropriate error or exception if there's a duplicate identifier.
* b) When starting an auction, verify that the vehicle exists in the inventory and is not already in an active auction. Raise an error if the vehicle does not exist or if it's already in an auction.
* c) When placing a bid, validate that the auction for the given vehicle is active and that the bid amount is greater than the current highest bid. Raise an error if the auction is not active or if the bid amount is invalid.
* d) Handle any other potential error scenarios and edge cases that you identify during your implementation. Consider cases like invalid inputs, out-of-range values, or unexpected behaviour.

Your task is to design a C# solution that uses object-oriented design principles to model this system with the appropriate tests. There is no requirement for any UI or database, with the focus being on the structure of the code and the quality of the tests.

Your solution should include:
* Definition of the classes and their properties and methods.
* Implement the auction management operations (add vehicles, search vehicles, start an auction, place a bid, and close the auction).

Pay special attention to edge cases, such as attempting to bid on a vehicle that doesn't have an active auction.
Please ensure that your code is clean and efficient. You should aim for a solution that is easy to understand and modify.
 
Deliverables
* The source code for the car auction management system includes all necessary classes, interfaces, etc.
* Unit tests for the auction management operations.
* A brief writeup explaining your design decisions and any assumptions you made.

---

# Implementation Overview

## Running the Project

You can run the project in two main ways:

### 1. Using Aspire (Recommended for Development & Integration Testing)

- **Aspire** enables running the application and its dependencies (like PostgreSQL) directly from the IDE (Visual Studio/VS Code) or via the `dotnet aspire` CLI.
- Aspire manages service orchestration, environment variables, and health checks for a smooth developer experience.
- Integration tests are configured to use Aspire resources, ensuring the app is tested end-to-end with a real PostgreSQL database.

### 2. Using Docker Compose

- The project includes a `docker-compose.yaml` file that defines services for the API, PostgreSQL, and the Aspire dashboard.
- To run:
  ```sh
  docker-compose up --build
  ```
- The API will be available at `http://localhost:8080` and PostgreSQL at `localhost:5432`.

## Architecture

- **Clean Architecture**: The solution is structured into Application, Domain, Infrastructure, and WebApi layers.
  - **Domain**: Core business entities and logic.
  - **Application**: Use cases, commands, queries, and validation.
  - **Infrastructure**: Data access (EF Core, PostgreSQL), repositories, and external integrations.
  - **WebApi**: HTTP endpoints, contracts, and API composition.

- **MediatR**: Used for CQRS (Command Query Responsibility Segregation). All business actions (commands/queries) are handled via MediatR request/response pipelines.

- **FluentValidation**: All commands and DTOs are validated using FluentValidation, ensuring robust input validation and clear error messages.

- **Entity Framework Core + Npgsql**: Data persistence is handled by EF Core with the Npgsql driver for PostgreSQL. Migrations are used to manage schema changes.

- **Concurrency Control**: A per-auction `SemaphoreSlim` is used in the repository to ensure that bid placements and auction updates for the same auction are processed sequentially, preventing race conditions.

## Endpoints

### Vehicle Endpoints

- **POST `/api/auction/vehicle/`**  
  Add a new vehicle.  
  **Body:**  
  ```json
  {
    "vin": "string",
    "type": "Sedan|SUV|Hatchback|Truck",
    "manufacturer": "string",
    "model": "string",
    "year": 2020,
    "startingBid": 10000,
    "numberOfDoors": 4,         // Sedan/Hatchback only
    "numberOfSeats": 7,         // SUV only
    "loadCapacity": 12000       // Truck only
  }
  ```

- **GET `/api/auction/vehicle/`**  
  Search vehicles by type, manufacturer, model, or year.  
  **Query params:** `type`, `manufacturer`, `model`, `year`

### Auction Endpoints

- **POST `/api/auction/start`**  
  Start an auction for a vehicle.  
  **Body:** `{ "vin": "string" }`

- **POST `/api/auction/close`**  
  Close an active auction.  
  **Body:** `{ "vin": "string" }`

- **GET `/api/auction/`**  
  List auctions, optionally filtered by status, vehicle type, manufacturer, model, or year.

### Bid Endpoints

- **POST `/api/bid/`**  
  Place a bid on an active auction.  
  **Body:**  
  ```json
  {
    "vin": "string",
    "amount": 10500,
    "bidder": "string"
  }
  ```

## Validation & Error Handling

- Duplicate VINs, invalid vehicle types, out-of-range values, and other invalid inputs are rejected with clear error messages.
- Attempting to start an auction for a non-existent or already-auctioned vehicle, or placing invalid bids, returns appropriate HTTP error responses.

## Testing

### Unit Tests

- All core business logic (commands, validation, domain rules) is covered by unit tests.

### Integration Tests

- Integration tests use **Aspire** to spin up the full application stack, including a real PostgreSQL database.
- Tests use the `WebApplicationFactory` and real HTTP requests to verify endpoint behavior, data persistence, and error handling.
- Tests cover all main scenarios and edge cases, including:
  - Adding vehicles (all types)
  - Duplicate VINs
  - Searching vehicles
  - Starting/closing auctions
  - Placing valid and invalid bids
  - Ensuring auction and bid state transitions are correct

### Concurrency & Auction Locking

- The repository uses a static `ConcurrentDictionary<Guid, SemaphoreSlim>` to lock updates per auction.
- This ensures that concurrent bid placements and auction updates for the same auction are processed sequentially, preventing race conditions and data corruption.

---

## API Examples

- See [`api.http`](./api.http) for sample HTTP requests for all endpoints.
- See [`api.postman_collection.json`](./api.postman_collection.json) for a ready-to-import Postman collection.

---

## Design Decisions & Assumptions

- The system is designed for extensibility and testability, following clean architecture and SOLID principles.
- All business rules are enforced at the application layer, with validation and error handling at every step.
- The use of MediatR and FluentValidation decouples business logic from the API layer.
- Integration tests ensure the system works end-to-end, including with real database state and concurrency scenarios.

---
