using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Contracts.Attributes;

public sealed class NoContentResponseAttribute : ProducesResponseTypeAttribute
{
    public NoContentResponseAttribute() : base(StatusCodes.Status204NoContent)
    {
    }
}
