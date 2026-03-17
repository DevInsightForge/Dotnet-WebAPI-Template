using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Contracts.Attributes;

public sealed class CreatedResponseAttribute<T> : ProducesResponseTypeAttribute
{
    public CreatedResponseAttribute() : base(typeof(T), StatusCodes.Status201Created)
    {
    }
}

