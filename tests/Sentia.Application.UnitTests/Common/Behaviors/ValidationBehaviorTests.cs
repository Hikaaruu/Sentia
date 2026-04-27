using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Sentia.Application.Common.Behaviors;
using ValidationException = Sentia.Application.Common.Exceptions.ValidationException;

namespace Sentia.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    public record TestRequest(string Value) : IRequest<string>;

    private static RequestHandlerDelegate<string> NextReturning(string value)
        => () => Task.FromResult(value);

    [Fact]
    public async Task Handle_NoValidatorsRegistered_CallsNextAndReturnsResult()
    {
        var sut = new ValidationBehavior<TestRequest, string>([]);
        var next = NextReturning("ok");

        var result = await sut.Handle(new TestRequest("x"), next, CancellationToken.None);

        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_ValidationPasses_CallsNextAndReturnsResult()
    {
        var validator = new Mock<IValidator<TestRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        var sut = new ValidationBehavior<TestRequest, string>([validator.Object]);
        var next = NextReturning("ok");

        var result = await sut.Handle(new TestRequest("x"), next, CancellationToken.None);

        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_ValidationFails_ThrowsValidationExceptionWithoutCallingNext()
    {
        var failures = new List<ValidationFailure>
        {
            new("Value", "Value is required.")
        };
        var validator = new Mock<IValidator<TestRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var sut = new ValidationBehavior<TestRequest, string>([validator.Object]);

        var nextCalled = false;
        RequestHandlerDelegate<string> next = () => { nextCalled = true; return Task.FromResult("ok"); };

        var act = () => sut.Handle(new TestRequest(""), next, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_MultipleValidationErrors_AggregatesAllFailures()
    {
        var failures = new List<ValidationFailure>
        {
            new("Value", "Value is required."),
            new("Value", "Value must be unique."),
            new("OtherField", "OtherField is invalid.")
        };
        var validator = new Mock<IValidator<TestRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var sut = new ValidationBehavior<TestRequest, string>([validator.Object]);

        var act = () => sut.Handle(new TestRequest(""), NextReturning("ok"), CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().ContainKey("Value")
            .WhoseValue.Should().HaveCount(2);
        ex.Which.Errors.Should().ContainKey("OtherField");
    }
}
