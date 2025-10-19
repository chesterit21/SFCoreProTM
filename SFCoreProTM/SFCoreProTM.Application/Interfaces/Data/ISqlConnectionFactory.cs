using System.Data;

namespace SFCoreProTM.Application.Interfaces.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
