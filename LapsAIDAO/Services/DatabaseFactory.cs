using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public static class DatabaseFactory
    {
        public static IDatabaseFactory Create(string type, IServiceProvider sp)
        {
            var config = sp.GetRequiredService<IConfiguration>();

            return type switch
            {
                "MongoServer" => new MongoDatabaseFactory(
                    config,
                    sp.GetRequiredService<ILoggerFactory>(),
                    sp.GetRequiredService<IMongoClient>()
                ),

                "SqlServer" => new SqlServerDatabaseFactory(
                    config.GetConnectionString("SqlServer")
                ),

                "Postgres" => new PostgresDatabaseFactory(
                    config.GetConnectionString("Postgres")
                ),

                _ => throw new Exception("Unknown DB type.")
            };
        }
    }
}
