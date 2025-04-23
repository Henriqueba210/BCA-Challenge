using AutoFixture.Xunit2;

using Auction.Application.Behaviours;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Application.Test.Behaviours;

public class ExceptionHandlingBehaviorTests
{
    private readonly IFixture _fixture;
    private readonly ILogger<ExceptionHandlingBehavior<TestQuery, Result>> _logger;
    private readonly ExceptionHandlingBehavior<TestQuery, Result> _behavior;

    public ExceptionHandlingBehaviorTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _logger = _fixture.Freeze<ILogger<ExceptionHandlingBehavior<TestQuery, Result>>>();
        _behavior = new ExceptionHandlingBehavior<TestQuery, Result>(_logger);
    }

    [Theory]
    [AutoData]
    public async Task Handle_SuccessfulRequest_ShouldPassThrough(TestQuery request)
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
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.Contains("Unhandled Exception occurred for Request TestQuery")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task Handle_WhenExceptionThrown_ShouldReturnCriticalError(TestQuery request)
    {
        // Arrange
        RequestHandlerDelegate<Result> next = () =>
            throw new Exception("Test exception");

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.CriticalError);
        result.Errors.First().ShouldContain("An error occurred processing request TestQuery");

        Mock.Get(_logger).Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.Contains("Unhandled Exception occurred for Request TestQuery")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}