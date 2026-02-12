using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Application.Queries.GetUserByEmailAndPassword;
using AgroSolutions.Identity.Application.Queries.GetUserById;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Domain.Repositories;
using AgroSolutions.Identity.Domain.Service;
using AgroSolutions.Identity.Infrastructure.Persistence;
using FluentAssertions;
using Moq;

namespace AgroSolutions.Identity.Tests.Queries;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<INotificationContext> _notificationContext = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly GetUserByIdQueryHandler _queryHandler;

    public GetUserByIdQueryHandlerTests()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_userRepository.Object);
        _queryHandler = new(_unitOfWork.Object, _notificationContext.Object);
    }

    [Fact(DisplayName = "Should return token when user exists")]
    public async Task Should_ReturnToken_WhenUserExists()
    {
        // Arrange
        GetUserByIdQuery getUserByIdQuery = new(1);
        User userDb = new(1, "Valid name user", "validEmail@gmail.com");
        _unitOfWork.Setup(u => u.Users.GetByIdNoTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(userDb);

        // Act
        GetUserByIdResult getUserByIdResult = (await _queryHandler.Handle(getUserByIdQuery, CancellationToken.None))!;

        // Assert
        getUserByIdResult.Should().NotBeNull();
        getUserByIdResult.UserId.Should().Be(userDb.UserId);
        getUserByIdResult.Email.Should().Be(userDb.Email);
        getUserByIdResult.Name.Should().Be(userDb.Name);
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Never);
        _unitOfWork.Verify(u => u.Users.GetByIdNoTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should notify and return null when user not found")]
    public async Task Should_ReturnNullAndNotify_WhenUserNotFound()
    {
        // Arrange
        GetUserByIdQuery getUserByIdQuery = new(1);
        _unitOfWork.Setup(u => u.Users.GetByIdNoTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        GetUserByIdResult getUserByIdResult = (await _queryHandler.Handle(getUserByIdQuery, CancellationToken.None))!;

        // Assert
        getUserByIdResult?.Should().BeNull();
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Once);
        _unitOfWork.Verify(u => u.Users.GetByIdNoTrackingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
