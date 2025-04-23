using Auction.Application.Abstractions.Messaging;

namespace Application.Test.Behaviours;

// Test classes for the validation behavior
public class TestQuery : IQuery<TestResponse>
{
    public string? Name { get; set; }
}

public class TestResponse
{
    public string? Name { get; set; }
}