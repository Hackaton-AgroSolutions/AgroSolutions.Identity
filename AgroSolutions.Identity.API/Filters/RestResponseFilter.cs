using AgroSolutions.Identity.API.Responses;
using AgroSolutions.Identity.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AgroSolutions.Identity.API.Extensions;
using AgroSolutions.Identity.Infrastructure.Extensions;

namespace AgroSolutions.Identity.API.Filters;

public class RestResponseFilter(INotificationContext notification) : IAsyncResultFilter
{
    private readonly INotificationContext _notification = notification;

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (_notification.HasValidations)
        {
            context.Result = new BadRequestObjectResult(new RestResponseWithInvalidFields { InvalidFields = _notification.Validations });
            await next();
            return;
        }

        if (_notification.HasNotifications)
        {
            context.Result = new ObjectResult(new RestResponse { Notifications = _notification.Notifications.Select(n => n.Type!.GetDescription()) })
            {
                StatusCode = MapStatusCode(_notification.Notifications)
            };
            await next();
            return;
        }

        if (context.Result is NoContentResult)
        {
            await next();
            return;
        }

        switch (context.Result)
        {
            case CreatedAtActionResult created:
                created.Value = new RestResponse { Data = created.Value };
                break;

            case AcceptedResult accepted:
                accepted.Value = default;
                break;

            case OkObjectResult ok:
                ok.Value = new RestResponse { Data = ok.Value! };
                break;

            case ObjectResult obj when obj.StatusCode is >= 200 and < 300:
                obj.Value = new RestResponse { Data = obj.Value! };
                break;
        }

        await next();
    }

    private static int MapStatusCode(IReadOnlyCollection<Notification> notifications)
    {
        if (notifications.Any(n => n.Type == NotificationType.InvalidCredentialsError))
            return StatusCodes.Status401Unauthorized;

        if (notifications.Any(n => n.Type == NotificationType.EmailAlreadyInUse))
            return StatusCodes.Status409Conflict;

        return StatusCodes.Status400BadRequest;
    }
}
