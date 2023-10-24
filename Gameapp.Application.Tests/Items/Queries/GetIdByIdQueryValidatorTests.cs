using FluentValidation.Results;
using Gameapp.Application.Features.Items.Queries.GetItemById;

namespace Gameapp.Application.Tests.Items.Queries;

public class GetIdByIdQueryValidatorTests
{
    private GetItemByIdQueryValidator _validator;

    public GetIdByIdQueryValidatorTests()
    {
        _validator = new GetItemByIdQueryValidator();
    }

    [Fact]
    public async Task Should_ReturnFailureResult_WhenQueryItemRequestInvalid()
    {
        // Arrange
        GetItemByIdQuery command = new GetItemByIdQuery
        {
            Id = Guid.Empty
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(command, default);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Should_ReturnSuccessResult_WhenQueryItemRequestValid()
    {
        // Arrange
        GetItemByIdQuery command = new GetItemByIdQuery
        {
            Id = Guid.NewGuid()
        };

        // Act
        ValidationResult result = await _validator.ValidateAsync(command, default);

        // Assert
        Assert.True(result.IsValid);
    }
}
