using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Application.Queries.AuthenticateUser;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Domain.Repositories;
using AgroSolutions.Identity.Domain.Service;
using AgroSolutions.Identity.Infrastructure.Persistence;
using FluentAssertions;
using Moq;

namespace AgroSolutions.Identity.Tests.Queries;

public class AuthenticateUserQueryHandlerTests
{
    private readonly Mock<INotificationContext> _notificationContext = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly AuthenticateUserQueryHandler _queryHandler;

    public AuthenticateUserQueryHandlerTests()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_userRepository.Object);
        _queryHandler = new(
            _unitOfWork.Object,
            _authService.Object,
            _notificationContext.Object
        );
    }

    [Fact(DisplayName = "Should return token when email and password matchs")]
    public async Task Should_ReturnToken_WhenEmailAndPasswordMatchs()
    {
        // Arrange
        AuthenticateUserQuery getUserByEmailAndPasswordQuery = new("validEmail@gmail.com", "password1234$$");
        User userDb = new(1, "Valid name user", "validEmail@gmail.com", "$2a$12$2Dj1BaOnV8X0ej7U0KIOjOneac1OOcv9L8rhoIbOgSiafuPPnwQIi");
        _unitOfWork.Setup(u => u.Users.GetByEmailNoTrackingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(userDb);
        _authService.Setup(a => a.GenerateToken(userDb)).Returns("eyJb");

        // Act
        TokenDto tokenDto = (await _queryHandler.Handle(getUserByEmailAndPasswordQuery, CancellationToken.None))!;

        // Assert
        tokenDto.Should().NotBeNull();
        tokenDto.Token.Should().NotBeNullOrEmpty();
        tokenDto.Token.Should().Be("eyJb");
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Never);
        _authService.Verify(a => a.GenerateToken(It.IsAny<User>()), Times.Once);
        _unitOfWork.Verify(u => u.Users.GetByEmailNoTrackingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should notify and return null when email and password don't matches")]
    public async Task Should_ReturnNullAndNotify_WhenEmailAndPasswordDontMatches()
    {
        // Arrange
        AuthenticateUserQuery getUserByEmailAndPasswordQuery = new("validEmail@gmail.com", "wrong password");
        User userDb = new(1, "Valid name user", "validEmail@gmail.com", "$2a$12$2Dj1BaOnV8X0ej7U0KIOjOneac1OOcv9L8rhoIbOgSiafuPPnwQIi");
        _unitOfWork.Setup(u => u.Users.GetByEmailNoTrackingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(userDb);

        // Act
        TokenDto? tokenDto = await _queryHandler.Handle(getUserByEmailAndPasswordQuery, CancellationToken.None);

        // Assert
        tokenDto.Should().BeNull();
        tokenDto?.Token?.Should().BeNullOrEmpty();
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Once);
        _unitOfWork.Verify(u => u.Users.GetByEmailNoTrackingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _authService.Verify(a => a.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}
