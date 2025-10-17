using System;
using SFCoreProTM.Application.Interfaces.Security;

namespace SFCoreProTM.Persistence.Services.Security;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        ArgumentNullException.ThrowIfNull(providedPassword);
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}
