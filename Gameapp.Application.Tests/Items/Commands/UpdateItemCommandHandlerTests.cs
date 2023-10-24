using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Gameapp.Application.Contracts;
using Gameapp.Application.Features.Items.Commands.UpdateItem;
using Gameapp.Application.Features.Items.Queries.GetItemById;
using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using MediatR;
using Moq;

namespace Gameapp.Application.Tests.Items.Commands;

public class UpdateItemCommandHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IValidator<UpdateItemCommand>> _mockValidator;
    private readonly Mock<IItemRepository> _mockItemRepository;

    public UpdateItemCommandHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockMediator = new Mock<IMediator>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockValidator = new Mock<IValidator<UpdateItemCommand>>();
        _mockItemRepository = new Mock<IItemRepository>();
    }

    [Fact]
    public async Task Update_Handle_Should_ReturnSuccessResult()
    {
        // Arrange
        UpdateItemCommand command = new UpdateItemCommand
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Description = "Description"
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

        _mockMediator.Setup(
            m => m.Send(
                It.IsAny<GetItemByIdQuery>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        UpdateItemCommandHandler handler = new UpdateItemCommandHandler(
            _mockMapper.Object,
            _mockMediator.Object,
            _mockUnitOfWork.Object,
            _mockValidator.Object,
            _mockItemRepository.Object);

        // Act
        await handler.Handle(command, default);

        // Assert
        Assert.Equal(command.Name, item.Name);
        Assert.Equal(command.Description, item.Description);
        Assert.True(item.UpdatedDate > item.CreatedDate);

        _mockItemRepository.Verify(r => r.Update(item), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Update_Handle_Should_ReturnFailureResult()
    {
        // Arrange
        UpdateItemCommand command = new UpdateItemCommand 
        { 
            Id = Guid.Empty, 
            Name = string.Empty, 
            Description = string.Empty 
        };

        _mockValidator.Setup(
            v => v.ValidateAsync(
                command, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Id", "Id não pode ser vazio."),
                new ValidationFailure("Name", "Name não pode ser vazio."),
                new ValidationFailure("Description", "Description não pode ser vazio.")
            }));

        UpdateItemCommandHandler handler = new UpdateItemCommandHandler(
            _mockMapper.Object,
            _mockMediator.Object,
            _mockUnitOfWork.Object,
            _mockValidator.Object,
            _mockItemRepository.Object);

        // Act
        ValidationException ex = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, default));

        // Assert
        Assert.Contains("Id não pode ser vazio.", ex.Message);
        Assert.Contains("Name não pode ser vazio.", ex.Message);
        Assert.Contains("Description não pode ser vazio.", ex.Message);
    }
}
