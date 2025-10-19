using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SFCoreProTM.Application.Interfaces.Data;

namespace SFCoreProTM.Persistence.Data;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        return new NpgsqlConnection(connectionString);
    }
}
