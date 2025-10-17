using System;

namespace SFCoreProTM.Application.DTOs.Labels;

public sealed class UpdateLabelRequestDto
{
    public string? Name { get; set; }

    public string? ColorHex { get; set; }

    public string? Description { get; set; }

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }
}
