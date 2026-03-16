using DevInsightForge.Application.DtoModels.Common;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Application.Features.Users.Commands;
using DevInsightForge.Application.Features.Users.Queries;
using DevInsightForge.WebAPI.Contracts.Attributes;
using DevInsightForge.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [CreatedResponse<UserResponseModel>]
    public async Task<ActionResult<UserResponseModel>> CreateUser(CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateUserCommand(request), cancellationToken);
        return result.ToCreatedActionResult();
    }

    [HttpGet("{id:guid}")]
    [SuccessResponse<UserResponseModel>]
    public async Task<ActionResult<UserResponseModel>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        return result.ToOkActionResult();
    }

    [HttpGet]
    [SuccessResponse<PaginatedResponseDto<UserResponseModel>>]
    public async Task<ActionResult<PaginatedResponseDto<UserResponseModel>>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetUsersQuery(pageNumber, pageSize), cancellationToken);
        return result.ToOkActionResult();
    }

    [HttpPut("{id:guid}")]
    [SuccessResponse<UserResponseModel>]
    public async Task<ActionResult<UserResponseModel>> UpdateUser(Guid id, UpdateUserRequestDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateUserCommand(id, request), cancellationToken);
        return result.ToOkActionResult();
    }

    [HttpDelete("{id:guid}")]
    [NoContentResponse]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return result.ToNoContentActionResult();
    }
}
