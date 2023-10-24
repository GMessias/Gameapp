using FluentValidation.Results;
using Gameapp.Application.Features.Items.Commands.CreateItem;

namespace Gameapp.Application.Tests.Items.Commands;

public class CreateItemCommandValidatorTests
{
    private CreateItemCommandValidator _validator;

    public CreateItemCommandValidatorTests()
    {
        _validator = new CreateItemCommandValidator();
    }

    [Theory]
    [InlineData("", "Description")]
    [InlineData("Name", "")]
    [InlineData(null, "Description")]
    [InlineData("Name", null)]
    [InlineData("", "")]
    [InlineData(null, null)]
    public async Task Should_ReturnFailureResult_WhenCreateItemRequestInvalid(string name, string description)
    {
        // Arrange
        CreateItemCommand command = new CreateItemCommand
        {
            Name = name,
            Description = description
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(command, default);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Should_ReturnSuccessResult_WhenCreateItemRequestValid()
    {
        // Arrange
        CreateItemCommand command = new CreateItemCommand
        {
            Name = "Name",
            Description = "Description"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(command, default);

        // Assert
        Assert.True(result.IsValid);
    }
}
