using FluentValidation.Results;
using Gameapp.Application.Features.Items.Commands.UpdateItem;

namespace Gameapp.Application.Tests.Items.Commands;

public class UpdateItemCommandValidatorTests
{
    private UpdateItemCommandValidator _validator;

    public UpdateItemCommandValidatorTests()
    {
        _validator = new UpdateItemCommandValidator();
    }

    [Theory]
    [InlineData("", "Description")]
    [InlineData("Name", "")]
    [InlineData(null, "Description")]
    [InlineData("Name", null)]
    [InlineData("", "")]
    [InlineData(null, null)]
    public async Task Should_ReturnFailureResult_WhenUpdateItemRequestInvalid(string name, string description)
    {
        // Arrange
        UpdateItemCommand command = new UpdateItemCommand
        {
            Id = Guid.Empty,
            Name = name,
            Description = description
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(command, default);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Should_ReturnSuccessResult_WhenUpdateItemRequestValid()
    {
        // Arrange
        UpdateItemCommand command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Description = "Description"
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(command, default);

        // Assert
        Assert.True(result.IsValid);
    }
}
