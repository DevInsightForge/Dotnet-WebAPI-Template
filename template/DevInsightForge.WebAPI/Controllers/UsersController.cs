using DevInsightForge.Application.DtoModels.Common;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Application.Features.Users.Commands;
using DevInsightForge.Application.Features.Users.Queries;
using DevInsightForge.WebAPI.Common.Mappings;
using DevInsightForge.WebAPI.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ApiResponse<UserResponseModel>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateUserRequestDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateUserCommand(dto), ct);
        return result.ToApiResponse();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ApiResponse<UserResponseModel>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), ct);
        return result.ToApiResponse();
    }

    [HttpGet]
    [ProducesResponseType<ApiResponse<PaginatedResponseDto<UserResponseModel>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetUsersQuery(pageNumber, pageSize), ct);
        return result.ToApiResponse();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ApiResponse<UserResponseModel>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequestDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateUserCommand(id, dto), ct);
        return result.ToApiResponse();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<ApiResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteUserCommand(id), ct);
        return result.ToApiResponse();
    }
}
