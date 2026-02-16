using AgroSolutions.Identity.Application.Commands.UpdateUser;
using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Domain.Repositories;
using AgroSolutions.Identity.Domain.Service;
using AgroSolutions.Identity.Infrastructure.Persistence;
using FluentAssertions;
using Moq;

namespace AgroSolutions.Identity.Tests.Commands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<INotificationContext> _notificationContext = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly UpdateUserCommandHandler _commandHandler;

    public UpdateUserCommandHandlerTests()
    {
        _commandHandler = new(
            _notificationContext.Object,
            _unitOfWork.Object,
            _authService.Object
        );
    }

    [Fact(DisplayName = "Should update user and return token when data is valid")]
    public async Task Should_UpdateUserAndReturnToken_WhenDataIsValid()
    {
        // Arrange
        UpdateUserCommand updateUserCommand = new(1, "New name user", "newValidEmail@gmail.com");
        User userDb = new(1, "Old name user", "oldValidEmail@gmail.com");
        _unitOfWork.Setup(u => u.Users.GetByIdTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(userDb);
        _unitOfWork.Setup(u => u.Users.ExistsByEmailExceptByUserIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _authService.Setup(a => a.GenerateToken(It.IsAny<User>())).Returns("eyJb");

        // Act
        TokenDto tokenDto = (await _commandHandler.Handle(updateUserCommand, CancellationToken.None))!;

        // Assert
        tokenDto.Should().NotBeNull();
        tokenDto.Token.Should().NotBeNullOrEmpty();
        tokenDto.Token.Should().Be("eyJb");
        userDb.Name.Should().Be(updateUserCommand.Name);
        userDb.Email.Should().Be(updateUserCommand.Email);
        _notificationContext.Verify(a => a.AddNotification(It.IsAny<NotificationType>()), Times.Never);
        _authService.Verify(a => a.GenerateToken(It.IsAny<User>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should notify and return null when user is not found")]
    public async Task Should_ReturnNullAndNotify_WhenUserIsNotFound()
    {
        // Arrange
        UpdateUserCommand updateUserCommand = new(1, "New name user", "newValidEmail@gmail.com");
        _unitOfWork.Setup(u => u.Users.GetByIdTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        TokenDto? tokenDto = await _commandHandler.Handle(updateUserCommand, CancellationToken.None);

        // Act - Assert
        tokenDto.Should().BeNull();
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Once);
        _authService.Verify(a => a.GenerateToken(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should notify and return null when email is in use")]
    public async Task Should_ReturnNullAndNotify_WhenEmailIsInUse()
    {
        // Arrange
        UpdateUserCommand updateUserCommand = new(1, "New name user", "newValidEmail@gmail.com");
        _unitOfWork.Setup(u => u.Users.GetByIdTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<User>());
        _unitOfWork.Setup(u => u.Users.ExistsByEmailExceptByUserIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        TokenDto? tokenDto = await _commandHandler.Handle(updateUserCommand, CancellationToken.None);

        // Act - Assert
        tokenDto.Should().BeNull();
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Once);
        _authService.Verify(a => a.GenerateToken(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
