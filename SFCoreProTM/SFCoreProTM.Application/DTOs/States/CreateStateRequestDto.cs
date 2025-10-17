using System;

namespace SFCoreProTM.Application.DTOs.States;

public sealed class CreateStateRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string ColorHex { get; set; } = "#000000";

    public string? Description { get; set; }

    public string Group { get; set; } = "backlog";

    public bool IsDefault { get; set; }

    public bool IsTriage { get; set; }

    public int Sequence { get; set; } = 0;

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }
}
