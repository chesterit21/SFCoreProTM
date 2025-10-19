
using System;

namespace SFCoreProTM.Application.Interfaces.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? WorkspaceId { get; }
}
