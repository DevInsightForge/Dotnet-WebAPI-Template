using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.DtoModels.Common;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Users.Queries;

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<GetUsersQuery, Task<Result<PaginatedResponseDto<UserResponseModel>>>>;

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
    IValidator<GetUsersQuery> getUsersValidator) : IRequestHandler<GetUsersQuery, Task<Result<PaginatedResponseDto<UserResponseModel>>>>
{
    public async Task<Result<PaginatedResponseDto<UserResponseModel>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var validationResult = await getUsersValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Result<PaginatedResponseDto<UserResponseModel>>.ValidationFailure(validationResult);
        }

        var users = await unitOfWork.Users.GetAllAsync(request.PageNumber, request.PageSize);
        var response = new PaginatedResponseDto<UserResponseModel>
        {
            TotalRecords = users.TotalRecords,
            CurrentPageNumber = users.CurrentPageNumber,
            PageSize = users.PageSize,
            Data = users.Data.Adapt<IEnumerable<UserResponseModel>>()
        };

        return Result<PaginatedResponseDto<UserResponseModel>>.Success(response);
    }
}
