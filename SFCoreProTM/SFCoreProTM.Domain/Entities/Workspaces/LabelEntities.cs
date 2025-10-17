using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Workspaces;

public sealed class Label : WorkspaceScopedEntity
{
    private Label()
    {
    }

    private Label(Guid id, Guid workspaceId, string name, ColorCode color, Slug slug)
        : base(id, workspaceId)
    {
        Name = name;
        Color = color;
        Slug = slug;
    }

    public string Name { get; private set; } = string.Empty;

    public ColorCode Color { get; private set; } = ColorCode.FromHex("#000000");

    public Slug Slug { get; private set; } = Slug.Create("label");

    public string? Description { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public static Label Create(Guid id, Guid workspaceId, string name, ColorCode color, Slug slug)
    {
        return new Label(id, workspaceId, name, color, slug);
    }

    public void UpdateDetails(string name, string? description, ColorCode color)
    {
        Name = name;
        Description = description;
        Color = color;
        Slug = Slug.Create(name.ToLowerInvariant().Replace(' ', '-'));
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }
}
