using LapsAIDAO.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public class MongoDatabaseFactory : IDatabaseFactory
    {
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMongoClient _mongoClient;

        public MongoDatabaseFactory(
            IConfiguration config,
            ILoggerFactory loggerFactory,
            IMongoClient mongoClient)
        {
            _config = config;
            _loggerFactory = loggerFactory;
            _mongoClient = mongoClient;
        }

        public IRepository CreateUserRepository()
        {
            return new MongoUserRepository(
                _loggerFactory.CreateLogger<MongoUserRepository>(),
                _mongoClient,
                _config
            );
        }
    }
}
