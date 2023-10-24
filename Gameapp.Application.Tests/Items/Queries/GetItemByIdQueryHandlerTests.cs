using FluentValidation;
using FluentValidation.Results;
using Gameapp.Application.Contracts;
using Gameapp.Application.Features.Items.Commands.UpdateItem;
using Gameapp.Application.Features.Items.Queries.GetItemById;
using Gameapp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Gameapp.Application.Tests.Items.Queries;

public class GetItemByIdQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<IValidator<GetItemByIdQuery>> _mockValidator;

    public GetItemByIdQueryHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockValidator = new Mock<IValidator<GetItemByIdQuery>>();
    }

    [Fact]
    public async Task QueryById_Handle_Should_ReturnSuccessResult()
    {
        // Arrange
        GetItemByIdQuery command = new GetItemByIdQuery 
        { 
            Id = Guid.NewGuid() 
        };

        Item item = new Item 
        { 
            Id = command.Id, 
            Name = "Name", 
            Description = "Description" 
        };

        _mockValidator.Setup(
            v => v.ValidateAsync(
                command, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        Mock<DbSet<Item>> mockSet = new Mock<DbSet<Item>>();
        mockSet.Setup(m => m.FindAsync(command.Id)).ReturnsAsync(item);
      
        _mockContext.Setup(c => c.Items).Returns(mockSet.Object);

        GetItemByIdQueryHandler handler = new GetItemByIdQueryHandler(_mockContext.Object, _mockValidator.Object);

        // Act
        Item result = await handler.Handle(command, default);

        // Assert
        Assert.Equal(command.Id, result.Id);
    }

    [Fact]
    public async Task QueryById_Handle_Should_ReturnFailureResult()
    {
        // Arrange
        GetItemByIdQuery command = new GetItemByIdQuery 
        { 
            Id = Guid.Empty 
        };

        _mockValidator.Setup(
            v => v.ValidateAsync(
                command, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Id", "Id não pode ser vazio.")
            }));

        GetItemByIdQueryHandler handler = new GetItemByIdQueryHandler(_mockContext.Object, _mockValidator.Object);

        // Act
        ValidationException ex = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, default));

        // Assert
        Assert.Contains("Id não pode ser vazio.", ex.Message);
    }
}
