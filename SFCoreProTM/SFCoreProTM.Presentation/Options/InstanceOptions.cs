namespace SFCoreProTM.Presentation.Options;

public sealed class InstanceOptions
{
    public const string SectionName = "Instance";

    // Auth enablement
    public bool SignupsEnabled { get; set; } = true;
    public bool EmailPasswordLoginEnabled { get; set; } = true;

    // Invites
    public bool InvitesEnabled { get; set; } = true;
    public bool InviteRequiredForSignup { get; set; } = false;

    // Email restrictions
    public string[] AllowedEmailDomains { get; set; } = System.Array.Empty<string>();
    public string[] WhitelistEmails { get; set; } = System.Array.Empty<string>();
}
