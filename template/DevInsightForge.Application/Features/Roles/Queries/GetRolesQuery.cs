using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Contracts.Common;
using DevInsightForge.Application.Contracts.Role;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Roles.Queries;

public sealed record GetRolesQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<GetRolesQuery, Task<Result<PaginatedResponseDto<RoleResponseDto>>>>;

public sealed class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
{
    public GetRolesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}

internal sealed class GetRolesQueryHandler(
    IUnitOfWork unitOfWork,
    IValidator<GetRolesQuery> getRolesValidator) : IRequestHandler<GetRolesQuery, Task<Result<PaginatedResponseDto<RoleResponseDto>>>>
{
    public async Task<Result<PaginatedResponseDto<RoleResponseDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await getRolesValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<PaginatedResponseDto<RoleResponseDto>>.ValidationFailure(validationResult);
        }

        var roles = await unitOfWork.Roles.GetAllAsync(request.PageNumber, request.PageSize);
        var response = new PaginatedResponseDto<RoleResponseDto>
        {
            TotalRecords = roles.TotalRecords,
            CurrentPageNumber = roles.CurrentPageNumber,
            PageSize = roles.PageSize,
            Data = roles.Data.Adapt<IEnumerable<RoleResponseDto>>()
        };

        return Result<PaginatedResponseDto<RoleResponseDto>>.Success(response);
    }
}
