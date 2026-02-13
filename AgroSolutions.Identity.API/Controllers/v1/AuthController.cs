using AgroSolutions.Identity.API.Extensions;
using AgroSolutions.Identity.API.InputModels;
using AgroSolutions.Identity.Application.Commands.CreateUser;
using AgroSolutions.Identity.Application.Commands.DeleteUser;
using AgroSolutions.Identity.Application.Commands.UpdateUser;
using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Application.Queries.GetUserByEmailAndPassword;
using AgroSolutions.Identity.Application.Queries.GetUserById;
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
    public async Task<OkObjectResult> Me()
    {
        Log.Information("Starting Action {ActionName}.", nameof(Me));
        GetUserByIdQuery query = new(User.UserId());
        GetUserByIdResult? getUserByIdResult = await _mediator.Send(query);
        return Ok(getUserByIdResult);
    }

    [HttpPost("login")]
    public async Task<OkObjectResult> Login(GetUserByEmailAndPasswordInputModel inputModel)
    {
        Log.Information("Starting Action {ActionName}.", nameof(Login));
        GetUserByEmailAndPasswordQuery query = new(inputModel.Email, inputModel.Password);
        TokenDto? tokenDto = await _mediator.Send(query);
        return Ok(tokenDto);
    }

    [HttpPost("register")]
    public async Task<CreatedAtActionResult> Register(CreateUserInputModel inputModel)
    {
        Log.Information("Starting Action {ActionName}.", nameof(Register));
        CreateUserCommand command = new(inputModel.Name, inputModel.Email, inputModel.Password);
        TokenDto? tokenDto = await _mediator.Send(command);
        return CreatedAtAction(nameof(Me), tokenDto);
    }

    [Authorize]
    [HttpPatch("me")]
    public async Task<OkObjectResult> Update(UpdateUserInputModel inputModel)
    {
        Log.Information("Starting Action {ActionName}.", nameof(Update));
        UpdateUserCommand command = new(User.UserId(), inputModel.Name, inputModel.Email);
        TokenDto? tokenDto = await _mediator.Send(command);
        return Ok(tokenDto);
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<AcceptedResult> Delete()
    {
        Log.Information("Starting Action {ActionName}.", nameof(Delete));
        DeleteUserCommand command = new(User.UserId());
        await _mediator.Send(command);
        return Accepted();
    }
}
