using System.ComponentModel.DataAnnotations;

namespace SFCoreProTM.Application.Mapping.Requests.Authentication;

public sealed class SignUpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? DisplayName { get; set; }

    [MaxLength(255)]
    public string? FirstName { get; set; }

    [MaxLength(255)]
    public string? LastName { get; set; }

    [MaxLength(255)]
    public string? WorkspaceName { get; set; }

    [MaxLength(64)]
    public string? TeamSize { get; set; }

    // Optional invite token for invite-only flows
    public string? InviteToken { get; set; }
}
