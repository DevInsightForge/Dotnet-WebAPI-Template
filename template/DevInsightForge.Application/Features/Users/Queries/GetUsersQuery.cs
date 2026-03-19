using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Contracts.Common;
using DevInsightForge.Application.Contracts.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Users.Queries;

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<GetUsersQuery, Task<Result<PaginatedResponseDto<UserResponseDto>>>>;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}

internal sealed class GetUsersQueryHandler(
    IUnitOfWork unitOfWork,
    IValidator<GetUsersQuery> getUsersValidator) : IRequestHandler<GetUsersQuery, Task<Result<PaginatedResponseDto<UserResponseDto>>>>
{
    public async Task<Result<PaginatedResponseDto<UserResponseDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await getUsersValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<PaginatedResponseDto<UserResponseDto>>.ValidationFailure(validationResult);
        }

        var users = await unitOfWork.Users.GetAllAsync(request.PageNumber, request.PageSize, u => u.Role!);
        var response = new PaginatedResponseDto<UserResponseDto>
        {
            TotalRecords = users.TotalRecords,
            CurrentPageNumber = users.CurrentPageNumber,
            PageSize = users.PageSize,
            Data = users.Data.Adapt<IEnumerable<UserResponseDto>>()
        };

        return Result<PaginatedResponseDto<UserResponseDto>>.Success(response);
    }
}



