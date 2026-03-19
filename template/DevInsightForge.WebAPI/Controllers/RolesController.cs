using DevInsightForge.Application.Contracts.Common;
using DevInsightForge.Application.Contracts.Role;
using DevInsightForge.Application.Features.Roles.Commands;
using DevInsightForge.Application.Features.Roles.Queries;
using DevInsightForge.WebAPI.Contracts.Attributes;
using DevInsightForge.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [CreatedResponse<RoleResponseDto>]
    public async Task<ActionResult<RoleResponseDto>> CreateRole(CreateRoleRequestDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateRoleCommand(request), cancellationToken);
        return result.ToCreatedActionResult();
    }

    [HttpGet("{id:guid}")]
    [SuccessResponse<RoleResponseDto>]
    public async Task<ActionResult<RoleResponseDto>> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return result.ToOkActionResult();
    }

    [HttpGet]
    [SuccessResponse<PaginatedResponseDto<RoleResponseDto>>]
    public async Task<ActionResult<PaginatedResponseDto<RoleResponseDto>>> GetRoles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetRolesQuery(pageNumber, pageSize), cancellationToken);
        return result.ToOkActionResult();
    }

    [HttpPut("{id:guid}")]
    [SuccessResponse<RoleResponseDto>]
    public async Task<ActionResult<RoleResponseDto>> UpdateRole(Guid id, UpdateRoleRequestDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateRoleCommand(id, request), cancellationToken);
        return result.ToOkActionResult();
    }

    [HttpDelete("{id:guid}")]
    [NoContentResponse]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteRoleCommand(id), cancellationToken);
        return result.ToNoContentActionResult();
    }
}
