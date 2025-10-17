using System;

namespace SFCoreProTM.Application.DTOs.States;

public sealed class UpdateStateRequestDto
{
    public string? Name { get; set; }

    public string? ColorHex { get; set; }

    public string? Description { get; set; }

    public string? Group { get; set; }

    public bool? IsDefault { get; set; }

    public bool? IsTriage { get; set; }

    public int? Sequence { get; set; }

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }
}
