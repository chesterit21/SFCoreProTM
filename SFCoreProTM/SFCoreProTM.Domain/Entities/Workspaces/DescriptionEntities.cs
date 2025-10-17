using System;
using SFCoreProTM.Domain.Entities;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Domain.Entities.Workspaces;

public sealed class Description : WorkspaceScopedEntity
{
    private Description()
    {
    }

    private Description(Guid id, Guid workspaceId, RichTextContent content)
        : base(id, workspaceId)
    {
        Content = content;
    }

    public RichTextContent Content { get; private set; } = RichTextContent.Create();

    public static Description Create(Guid id, Guid workspaceId, RichTextContent content)
    {
        return new Description(id, workspaceId, content);
    }

    public void UpdateContent(RichTextContent content)
    {
        Content = content;
    }
}

public sealed class DescriptionVersion : WorkspaceScopedEntity
{
    private DescriptionVersion()
    {
    }

    private DescriptionVersion(Guid id, Guid workspaceId, Guid descriptionId, RichTextContent content)
        : base(id, workspaceId)
    {
        DescriptionId = descriptionId;
        Content = content;
    }

    public Guid DescriptionId { get; private set; }

    public RichTextContent Content { get; private set; } = RichTextContent.Create();

    public static DescriptionVersion Create(Guid id, Guid workspaceId, Guid descriptionId, RichTextContent content)
    {
        return new DescriptionVersion(id, workspaceId, descriptionId, content);
    }
}
