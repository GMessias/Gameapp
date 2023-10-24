using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Gameapp.Application.Contracts;
using Gameapp.Application.Features.Items.Commands.CreateItem;
using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using Moq;

namespace Gameapp.Application.Tests.Items.Commands;

public class CreateItemCommandHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IValidator<CreateItemCommand>> _mockValidator;
    private readonly Mock<IItemRepository> _mockItemRepository;

    public CreateItemCommandHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockValidator = new Mock<IValidator<CreateItemCommand>>();
        _mockItemRepository = new Mock<IItemRepository>();
    }

    [Fact]
    public async Task Create_Handle_Should_ReturnFailureResult()
    {
        // Arrange
        CreateItemCommand command = new CreateItemCommand
        {
            Name = string.Empty,
            Description = string.Empty
        };

        _mockValidator.Setup(
            x => x.ValidateAsync(
                It.Is<CreateItemCommand>(c => c.Name == string.Empty), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name não pode ser vazio."),
                new ValidationFailure("Description", "Description não pode ser vazio.")
            }));

        _mockValidator.Setup(
            x => x.ValidateAsync(
                It.Is<CreateItemCommand>(c => !string.IsNullOrEmpty(c.Name)), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        CreateItemCommandHandler handler = new CreateItemCommandHandler(
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockValidator.Object,
            _mockItemRepository.Object);

        // Act
        ValidationException ex = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, default));

        // Assert
        Assert.Contains("Name não pode ser vazio.", ex.Message);
        Assert.Contains("Description não pode ser vazio.", ex.Message);
    }

    [Fact]
    public async Task Create_Handle_Should_ReturnSuccessResult()
    {
        // Arrange
        CreateItemCommand command = new CreateItemCommand
        {
            Name = "Name",
            Description = "Description"
        };

        Item item = new Item 
        { 
            Name = command.Name, 
            Description = command.Description 
        };

        _mockValidator.Setup(
            x => x.ValidateAsync(
                command, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockMapper.Setup(
            m => m.Map<Item>(command))
            .Returns(item);

        _mockItemRepository.Setup(
            r => r.CreateAsync(item))
            .ReturnsAsync(item);

        CreateItemCommandHandler handler = new CreateItemCommandHandler(
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockValidator.Object,
            _mockItemRepository.Object);

        // Act
        Item result = await handler.Handle(command, default);

        // Assert
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Description, result.Description);
        Assert.True(result.CreatedDate != DateTime.MinValue && result.CreatedDate != DateTime.MaxValue);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
