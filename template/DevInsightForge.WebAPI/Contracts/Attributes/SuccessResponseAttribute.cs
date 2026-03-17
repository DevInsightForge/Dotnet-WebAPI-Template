using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Contracts.Attributes;

public sealed class SuccessResponseAttribute<T> : ProducesResponseTypeAttribute
{
    public SuccessResponseAttribute() : base(typeof(T), StatusCodes.Status200OK)
    {
    }
}

