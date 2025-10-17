using System;

namespace SFCoreProTM.Application.DTOs.Labels;

public sealed class CreateLabelRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string ColorHex { get; set; } = "#000000";

    public string? Description { get; set; }

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }
}
