using FluentValidation;
using FluentValidation.Results;
using Gameapp.Application.Contracts;
using Gameapp.Application.Features.Items.Commands.DeleteItem;
using Gameapp.Application.Features.Items.Queries.GetItemById;
using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using MediatR;
using Moq;

namespace Gameapp.Application.Tests.Items.Commands;

public class DeleteItemCommandHandlerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IValidator<DeleteItemCommand>> _mockValidator;
    private readonly Mock<IItemRepository> _mockItemRepository;

    public DeleteItemCommandHandlerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockValidator = new Mock<IValidator<DeleteItemCommand>>();
        _mockItemRepository = new Mock<IItemRepository>();
    }

    [Fact]
    public async Task Delete_Handle_Should_ReturnFailureResult()
    {
        // Arrange
        DeleteItemCommand command = new DeleteItemCommand { Id = Guid.Empty };

        _mockValidator.Setup(
            v => v.ValidateAsync(
                command, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Id", "Id não pode ser vazio.")
            }));

        DeleteItemCommandHandler handler = new DeleteItemCommandHandler(
            _mockMediator.Object,
            _mockUnitOfWork.Object,
            _mockValidator.Object,
            _mockItemRepository.Object);

        // Act
        ValidationException ex = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, default));

        // Assert
        Assert.Contains("Id não pode ser vazio.", ex.Message);
    }

    [Fact]
    public async Task Delete_Handle_Should_ReturnSuccessResult()
    {
        // Arrange
        DeleteItemCommand command = new DeleteItemCommand 
        { 
            Id = Guid.NewGuid() 
        };

        Item item = new Item 
        { 
            Id = command.Id 
        };

        _mockValidator.Setup(
            v => v.ValidateAsync(
                command, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockMediator.Setup(
            m => m.Send(
                It.IsAny<GetItemByIdQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        DeleteItemCommandHandler handler = new DeleteItemCommandHandler(
            _mockMediator.Object,
            _mockUnitOfWork.Object,
            _mockValidator.Object,
            _mockItemRepository.Object);

        // Act
        await handler.Handle(command, default);

        // Assert
        _mockItemRepository.Verify(r => r.Delete(item), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
