using AgroSolutions.Identity.Application.Commands.CreateUser;
using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Domain.Repositories;
using AgroSolutions.Identity.Domain.Service;
using AgroSolutions.Identity.Infrastructure.Persistence;
using FluentAssertions;
using Moq;

namespace AgroSolutions.Identity.Tests.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<INotificationContext> _notificationContext = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly CreateUserCommandHandler _commandHandler;

    public CreateUserCommandHandlerTests()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_userRepository.Object);
        _commandHandler = new(
            _notificationContext.Object,
            _unitOfWork.Object,
            _authService.Object
        );
    }

    [Fact(DisplayName = "Should register user and return token when email is available")]
    public async Task Should_RegisterUserAndReturnToken_WhenEmailIsAvailable()
    {
        // Arrange
        CreateUserCommand createUserCommand = new("Valid Name User", "validEmail@gmail.com", "F4hHW#7G40,!");
        _unitOfWork.Setup(u => u.Users.IsEmailInUseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _authService.Setup(a => a.GenerateToken(It.IsAny<User>())).Returns("eyJb");

        // Act
        TokenDto tokenDto = (await _commandHandler.Handle(createUserCommand, CancellationToken.None))!;

        // Assert
        tokenDto.Should().NotBeNull();
        tokenDto.Token.Should().NotBeNullOrEmpty();
        tokenDto.Token.Should().Be("eyJb");
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Never);
        _authService.Verify(a => a.GenerateToken(It.IsAny<User>()), Times.Once);
        _unitOfWork.Verify(u => u.Users.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should return null and notify when email is already in use")]
    public async Task Should_ReturnNullAndNotify_WhenEmailIsAlreadyInUse()
    {
        // Arrange
        CreateUserCommand createUserCommand = new("Valid Name User", "validEmail@gmail.com", "F4hHW#7G40,!");
        _unitOfWork.Setup(u => u.Users.IsEmailInUseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        TokenDto? tokenDto = await _commandHandler.Handle(createUserCommand, CancellationToken.None);

        // Act - Assert
        tokenDto.Should().BeNull();
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Once);
        _authService.Verify(a => a.GenerateToken(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(u => u.Users.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
