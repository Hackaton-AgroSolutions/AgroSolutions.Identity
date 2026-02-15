using AgroSolutions.Identity.Application.Queries.ValidateToken;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Domain.Repositories;
using AgroSolutions.Identity.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace AgroSolutions.Identity.Tests.Queries;

public class ValidateTokenQueryHandlerTests
{
    private readonly Mock<INotificationContext> _notificationContext = new();
    //private readonly INotificationContext _notificationContext = new NotificationContext();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly ValidateTokenQueryHandler _queryHandler;

    public ValidateTokenQueryHandlerTests()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_userRepository.Object);
        _queryHandler = new(_unitOfWork.Object, _notificationContext.Object, _memoryCache);
    }

    [Fact(DisplayName = "Should return unit when user exists in cache")]
    public async Task Should_ReturnUnit_WhenUserExistsInCache()
    {
        // Arrange
        ValidateTokenQuery validateTokenQuery = new(1);
        _memoryCache.Set(validateTokenQuery.UserId, validateTokenQuery.UserId);

        // Act
        Unit? unit = (await _queryHandler.Handle(validateTokenQuery, CancellationToken.None))!;

        // Assert
        unit.Should().NotBeNull();
        unit.Should().Be(Unit.Value);
        _memoryCache.Get<int?>(validateTokenQuery.UserId).Should().Be(validateTokenQuery.UserId);
        _unitOfWork.Verify(u => u.Users.ExistsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Never);
        //_notificationContext.HasNotifications.Should().Be(false);
    }

    [Fact(DisplayName = "Should return unit when user exists only in database")]
    public async Task Should_ReturnUnit_WhenUserExistsOnlyInDatabase()
    {
        // Arrange
        ValidateTokenQuery validateTokenQuery = new(1);
        _unitOfWork.Setup(u => u.Users.ExistsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        Unit? unit = (await _queryHandler.Handle(validateTokenQuery, CancellationToken.None))!;

        // Assert
        unit.Should().NotBeNull();
        unit.Should().Be(Unit.Value);
        _memoryCache.Get<int?>(validateTokenQuery.UserId).Should().Be(validateTokenQuery.UserId);
        _unitOfWork.Verify(u => u.Users.ExistsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Never);
        //_notificationContext.Notifications.Any(n => n.Type == NotificationType.UserNoLongerExists).Should().Be(false);
    }

    [Fact(DisplayName = "Should return null and Notify when user not exists in database")]
    public async Task Should_ReturnNullAndNotify_WhenUserNotExistsInDatabase()
    {
        // Arrange
        ValidateTokenQuery validateTokenQuery = new(1);
        _unitOfWork.Setup(u => u.Users.ExistsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        Unit? unit = await _queryHandler.Handle(validateTokenQuery, CancellationToken.None);

        // Assert
        unit.Should().BeNull();
        unit.Should().NotBe(Unit.Value);
        _memoryCache.Get<int?>(validateTokenQuery.UserId).Should().BeNull();
        _unitOfWork.Verify(u => u.Users.ExistsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _notificationContext.Verify(n => n.AddNotification(It.IsAny<NotificationType>()), Times.Once);
        //_notificationContext.HasNotifications.Should().Be(true);
    }
}
