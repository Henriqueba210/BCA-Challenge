using AutoFixture.Xunit2;

using Auction.Application.Behaviours;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Application.Test.Behaviours;

public class LoggingBehaviorTest
{
    private readonly ILogger<LoggingBehavior<TestQuery, Result>> _logger;
    private readonly LoggingBehavior<TestQuery, Result> _behavior;

    public LoggingBehaviorTest()
    {
        IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
        _logger = fixture.Freeze<ILogger<LoggingBehavior<TestQuery, Result>>>();
        _behavior = new LoggingBehavior<TestQuery, Result>(_logger);
    }

    [Theory]
    [AutoData]
    public async Task Handle_SuccessfulRequest_ShouldLogStartAndCompletion(TestQuery request)
    {
        // Arrange
        Result? expectedResult = Result.Success();
        RequestHandlerDelegate<Result> next = () => Task.FromResult(expectedResult);

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);

        Mock.Get(_logger).Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Processing request TestQuery")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        Mock.Get(_logger).Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Completed Request TestQuery")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task Handle_FailedRequest_ShouldLogError(TestQuery request)
    {
        // Arrange
        Result? expectedResult = Result.Error("Test error");
        RequestHandlerDelegate<Result> next = () => Task.FromResult(expectedResult);

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);

        Mock.Get(_logger).Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Completed Request TestQuery with error")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}