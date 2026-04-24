using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Infrastructure.Persistence.Services;

public class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connectionString = configuration.GetConnectionString("SentiaDatabase");
        return new SqlConnection(connectionString!);
    }
}