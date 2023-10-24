using FluentValidation.Results;
using Gameapp.Application.Features.Items.Commands.DeleteItem;

namespace Gameapp.Application.Tests.Items.Commands;

public class DeleteItemCommandValidatorTests
{
    private DeleteItemCommandValidator _validator;

    public DeleteItemCommandValidatorTests()
    {
        _validator = new DeleteItemCommandValidator();
    }

    [Fact]
    public async Task Should_ReturnFailureResult_WhenDeleteItemRequestInvalid()
    {
        // Arrange
        DeleteItemCommand command = new DeleteItemCommand
        {
            Id = Guid.Empty
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(command, default);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Should_ReturnSuccessResult_WhenDeleteItemRequestValid()
    {
        // Arrange
        DeleteItemCommand command = new DeleteItemCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(command, default);

        // Assert
        Assert.True(result.IsValid);
    }
}
