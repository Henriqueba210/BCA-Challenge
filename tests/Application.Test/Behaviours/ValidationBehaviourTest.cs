using AutoFixture.Xunit2;

using FluentValidation;

using Auction.Application.Behaviours;

using Microsoft.Extensions.Logging;

namespace Application.Test.Behaviours;

public class ValidationBehaviorTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Theory]
    [AutoData]
    public async Task Handle_WithNoValidators_ShouldReturnNextResult(TestQuery request)
    {
        // Arrange
        ILogger<ValidationBehavior<TestQuery, Result<TestResponse>>>? logger =
            _fixture.Freeze<ILogger<ValidationBehavior<TestQuery, Result<TestResponse>>>>();
        List<IValidator<TestQuery>> validators = new();
        ValidationBehavior<TestQuery, Result<TestResponse>> behavior = new(validators, logger);
        TestResponse response = new() { Id = 1 };
        Result<TestResponse>? expectedResult = Result<TestResponse>.Success(response);

        // Act
        Result<TestResponse> result = await behavior.Handle(
            request,
            () => Task.FromResult(expectedResult),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);
        result.Value.ShouldBe(response);
    }

    [Theory]
    [AutoData]
    public async Task Handle_WithValidators_NoErrors_ShouldReturnNextResult(TestQuery request)
    {
        // Arrange
        ILogger<ValidationBehavior<TestQuery, Result<TestResponse>>>? logger =
            _fixture.Freeze<ILogger<ValidationBehavior<TestQuery, Result<TestResponse>>>>();
        TestValidator validator = new();
        List<IValidator<TestQuery>> validators = new() { validator };
        ValidationBehavior<TestQuery, Result<TestResponse>> behavior = new(validators, logger);
        TestResponse response = new() { Id = 1 };
        Result<TestResponse>? expectedResult = Result<TestResponse>.Success(response);

        request.Name = "Valid";

        // Act
        Result<TestResponse> result = await behavior.Handle(
            request,
            () => Task.FromResult(expectedResult),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);
        result.Value.ShouldBe(response);
    }

    [Theory]
    [AutoData]
    public async Task Handle_WithMultipleValidators_NoErrors_ShouldReturnNextResult(TestQuery request)
    {
        // Arrange
        ILogger<ValidationBehavior<TestQuery, Result<TestResponse>>>? logger =
            _fixture.Freeze<ILogger<ValidationBehavior<TestQuery, Result<TestResponse>>>>();
        TestValidator validator1 = new();
        TestValidator validator2 = new();
        List<IValidator<TestQuery>> validators = new() { validator1, validator2 };
        ValidationBehavior<TestQuery, Result<TestResponse>> behavior = new(validators, logger);
        TestResponse response = new() { Id = 1 };
        Result<TestResponse>? expectedResult = Result<TestResponse>.Success(response);

        request.Name = "Valid";

        // Act
        Result<TestResponse> result = await behavior.Handle(
            request,
            () => Task.FromResult(expectedResult),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);
        result.Value.ShouldBe(response);
    }

    [Theory]
    [AutoData]
    public async Task Handle_WithValidationErrors_ShouldReturnInvalidResult(TestQuery request)
    {
        // Arrange
        ILogger<ValidationBehavior<TestQuery, Result<TestResponse>>>? logger =
            _fixture.Freeze<ILogger<ValidationBehavior<TestQuery, Result<TestResponse>>>>();
        TestValidator validator = new();
        List<IValidator<TestQuery>> validators = new() { validator };
        ValidationBehavior<TestQuery, Result<TestResponse>> behavior = new(validators, logger);
        TestResponse response = new() { Id = 1 };
        Result<TestResponse>? expectedResult = Result<TestResponse>.Success(response);

        request.Name = string.Empty;

        // Act
        Result<TestResponse> result = await behavior.Handle(
            request,
            () => Task.FromResult(expectedResult),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.ValidationErrors.Count().ShouldBe(1);
        result.ValidationErrors.First().ErrorMessage.ShouldBe("Name is required");
    }

    public class TestResponse
    {
        public int Id { get; set; }
    }

    public class TestValidator : AbstractValidator<TestQuery>
    {
        public TestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required");
        }
    }
}