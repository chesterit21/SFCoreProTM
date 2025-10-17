using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Projects;

public sealed class State : ProjectScopedEntity
{
    private State()
    {
    }

    private State(Guid id, Guid workspaceId, Guid projectId, string name, ColorCode color, Slug slug, int sequence)
        : base(id, workspaceId, projectId)
    {
        Name = name;
        Color = color;
        Slug = slug;
        Sequence = sequence;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public ColorCode Color { get; private set; } = ColorCode.FromHex("#000000");

    public Slug Slug { get; private set; } = Slug.Create("state");

    public int Sequence { get; private set; }

    public string Group { get; private set; } = "backlog";

    public bool IsTriage { get; private set; }

    public bool IsDefault { get; private set; }

    public ExternalReference? ExternalReference { get; private set; }

    public static State Create(Guid id, Guid workspaceId, Guid projectId, string name, ColorCode color, Slug slug, int sequence)
    {
        return new State(id, workspaceId, projectId, name, color, slug, sequence);
    }

    public void UpdateDetails(string? description, string group, bool isTriage, bool isDefault, int sequence)
    {
        Description = description;
        Group = group;
        IsTriage = isTriage;
        IsDefault = isDefault;
        Sequence = sequence;
    }

    public void SetExternalReference(string? source, string? identifier)
    {
        ExternalReference = source is null && identifier is null ? null : ExternalReference.Create(source, identifier);
    }

    public void Rename(string name, Slug slug)
    {
        Name = name;
        Slug = slug;
    }

    public void SetColor(ColorCode color)
    {
        Color = color;
    }
}
