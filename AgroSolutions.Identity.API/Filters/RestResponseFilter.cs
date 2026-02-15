using AgroSolutions.Identity.API.Extensions;
using AgroSolutions.Identity.API.Responses;
using AgroSolutions.Identity.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

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

        if ((context.Result is NoContentResult || context.Result is AcceptedResult) && _notification.HasNotifications)
        {
            context.Result = new ObjectResult(new RestResponse { Notifications = _notification.AsListString })
            {
                StatusCode = MapStatusCode(_notification.Notifications)
            };
            await next();
            return;
        }

        if (context.Result is ObjectResult objectResult && objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
        {
            if (objectResult.Value is not null)
            {
                RestResponse restResponse = new(objectResult.Value);

                if (_notification.HasNotifications)
                {
                    objectResult.StatusCode = (int)HttpStatusCode.MultiStatus;
                    objectResult.Value = restResponse with { Notifications = _notification.AsListString };
                }
                else
                    objectResult.Value = restResponse;
            }
            else
            {
                context.Result = new ObjectResult(new RestResponse { Notifications = _notification.AsListString })
                {
                    StatusCode = MapStatusCode(_notification.Notifications)
                };
            }
        }

        await next();
    }

    private static int MapStatusCode(IReadOnlyCollection<Notification> notifications)
    {
        if (notifications.Any(n => n.Type == NotificationType.InvalidCredentialsError))
            return StatusCodes.Status401Unauthorized;

        if (notifications.Any(n => n.Type == NotificationType.UserNotFound))
            return StatusCodes.Status401Unauthorized;

        if (notifications.Any(n => n.Type == NotificationType.UserNoLongerExists))
            return StatusCodes.Status401Unauthorized;

        if (notifications.Any(n => n.Type == NotificationType.EmailAlreadyInUse))
            return StatusCodes.Status409Conflict;

        return StatusCodes.Status400BadRequest;
    }
}
