using AgroSolutions.Identity.API.Extensions;
using AgroSolutions.Identity.API.InputModels;
using AgroSolutions.Identity.API.Responses;
using AgroSolutions.Identity.Application.Commands.CreateUser;
using AgroSolutions.Identity.Application.Commands.DeleteUser;
using AgroSolutions.Identity.Application.Commands.UpdateUser;
using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Application.Queries.AuthenticateUser;
using AgroSolutions.Identity.Application.Queries.GetUser;
using AgroSolutions.Identity.Application.Queries.ValidateToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AgroSolutions.Identity.API.Controllers.v1;

[ApiController]
[Route("api/v1/auth")]
public partial class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [Authorize]
    [HttpGet("me")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(RestResponse))]
    public async Task<OkObjectResult> Me()
    {
        Log.Information("Starting Action {ActionName}.", nameof(Me));
        GetUserQuery query = new(User.UserId);
        GetUserQueryResult? getUserQueryResult = await _mediator.Send(query);
        return Ok(getUserQueryResult);
    }

    [Authorize]
    [HttpGet("validate-token")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(RestResponse))]
    public async Task<NoContentResult> ValidateToken()
    {
        Log.Information("Starting Action {ActionName}.", nameof(ValidateToken));
        ValidateTokenQuery query = new(User.UserId);
        await _mediator.Send(query);
        return NoContent();
    }

    [HttpPost("login")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RestResponseWithInvalidFields))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(RestResponse))]
    public async Task<OkObjectResult> Login(GetUserByEmailAndPasswordInputModel inputModel)
    {
        Log.Information("Starting Action {ActionName}.", nameof(Login));
        AuthenticateUserQuery query = new(inputModel.Email, inputModel.Password);
        TokenDto? tokenDto = await _mediator.Send(query);
        return Ok(tokenDto);
    }

    [HttpPost("register")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RestResponseWithInvalidFields))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(RestResponse))]
    public async Task<CreatedAtActionResult> Register(CreateUserInputModel inputModel)
    {
        Log.Information("Starting Action {ActionName}.", nameof(Register));
        CreateUserCommand command = new(inputModel.Name, inputModel.Email, inputModel.Password);
        TokenDto? tokenDto = await _mediator.Send(command);
        return CreatedAtAction(nameof(Me), tokenDto);
    }

    [Authorize]
    [HttpPatch("me")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RestResponseWithInvalidFields))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(RestResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(RestResponse))]
    public async Task<OkObjectResult> Update(UpdateUserInputModel inputModel)
    {
        Log.Information("Starting Action {ActionName}.", nameof(Update));
        UpdateUserCommand command = new(User.UserId, inputModel.Name, inputModel.Email);
        TokenDto? tokenDto = await _mediator.Send(command);
        return Ok(tokenDto);
    }

    [Authorize]
    [HttpDelete("me")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(RestResponse))]
    public async Task<AcceptedResult> Delete()
    {
        Log.Information("Starting Action {ActionName}.", nameof(Delete));
        DeleteUserCommand command = new(User.UserId);
        await _mediator.Send(command);
        return Accepted();
    }
}
