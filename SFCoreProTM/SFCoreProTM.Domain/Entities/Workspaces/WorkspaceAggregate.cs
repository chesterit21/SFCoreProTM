using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Workspaces;

public sealed class Workspace : AggregateRoot
{

    private Workspace()
    {
    }

    private Workspace(Guid id, string name, Guid ownerId, Slug slug, string timezone)
        : base(id)
    {
        Name = name;
        OwnerId = ownerId;
        Slug = slug;
        Timezone = timezone;
    }

    public string Name { get; private set; } = string.Empty;

    public Url? Logo { get; private set; }

    public Guid? LogoAssetId { get; private set; }

    public Guid OwnerId { get; private set; }

    public Slug Slug { get; private set; } = Slug.Create("default-workspace");

    public string? OrganizationSize { get; private set; }

    public string Timezone { get; private set; } = "UTC";

    public ColorCode? BackgroundColor { get; private set; }



    public static Workspace Create(Guid id, string name, Guid ownerId, Slug slug, string timezone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(timezone);

        return new Workspace(id, name, ownerId, slug, timezone);
    }

    public void UpdateBranding(Url? logo, Guid? logoAssetId, ColorCode? backgroundColor)
    {
        Logo = logo;
        LogoAssetId = logoAssetId;
        BackgroundColor = backgroundColor;
    }

    public void UpdateOrganizationProfile(string? organizationSize, string timezone)
    {
        OrganizationSize = organizationSize;
        Timezone = timezone;
    }

}
