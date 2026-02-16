using AgroSolutions.Identity.Application.Commands.DeleteUser;
using AgroSolutions.Identity.Domain.Common;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Messaging;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace AgroSolutions.Identity.Tests.Commands;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<INotificationContext> _notificationContext = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IEventPublisher> _eventPublisher = new();
    private readonly Mock<IMemoryCache> _memoryCache = new();
    private readonly DeleteUserCommandHandler _commandHandler;

    public DeleteUserCommandHandlerTests()
    {
        _commandHandler = new(
            _notificationContext.Object,
            _unitOfWork.Object,
            _eventPublisher.Object,
            _memoryCache.Object
        );
    }

    [Fact(DisplayName = "Should delete user and return unit when user exists")]
    public async Task Should_DeleteUserAndReturnUnit_WhenUserExits()
    {
        // Arrange
        DeleteUserCommand deleteUserCommand = new(1);
        User userDb = new(1, "Old name user", "oldValidEmail@gmail.com", null);
        _unitOfWork.Setup(u => u.Users.GetByIdTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(userDb);

        // Act
        Unit? unit = (await _commandHandler.Handle(deleteUserCommand, CancellationToken.None))!;

        // Assert
        unit.Should().NotBeNull();
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisher.Verify(u => u.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should notify and return null when user is not found")]
    public async Task Should_ReturnNullAndNotify_WhenUserIsNotFound()
    {
        // Arrange
        DeleteUserCommand deleteUserCommand = new(1);
        _unitOfWork.Setup(u => u.Users.GetByIdTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        Unit? unit = (await _commandHandler.Handle(deleteUserCommand, CancellationToken.None))!;

        // Assert
        unit.Should().BeNull();
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _eventPublisher.Verify(u => u.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
