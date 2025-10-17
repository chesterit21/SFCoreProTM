using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Workspaces;

public sealed class Sticky : WorkspaceScopedEntity
{
    private Sticky()
    {
    }

    private Sticky(Guid id, Guid workspaceId, string name, Guid ownerId, double sortOrder, RichTextContent description)
        : base(id, workspaceId)
    {
        Name = name;
        OwnerId = ownerId;
        SortOrder = sortOrder;
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;

    public RichTextContent Description { get; private set; } = RichTextContent.Create();

    public StructuredData LogoProperties { get; private set; } = StructuredData.FromJson(null);

    public ColorCode? Color { get; private set; }

    public ColorCode? BackgroundColor { get; private set; }

    public Guid OwnerId { get; private set; }

    public double SortOrder { get; private set; }

    public static Sticky Create(Guid id, Guid workspaceId, string name, Guid ownerId, double sortOrder, RichTextContent description)
    {
        return new Sticky(id, workspaceId, name, ownerId, sortOrder, description);
    }

    public void UpdateContent(RichTextContent description, StructuredData logoProps)
    {
        Description = description;
        LogoProperties = logoProps;
    }

    public void UpdateVisuals(ColorCode? color, ColorCode? backgroundColor)
    {
        Color = color;
        BackgroundColor = backgroundColor;
    }

    public void Reorder(double sortOrder)
    {
        SortOrder = sortOrder;
    }
}
