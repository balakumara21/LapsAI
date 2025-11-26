using LapsAIDAO.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Repository
{
    public class MongoUserRepository : IRepository
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly ILogger<MongoUserRepository> _logger;
        private readonly IMongoClient _mongoClient;
        private readonly bool _supportTransactions;

        public MongoUserRepository(ILogger<MongoUserRepository> logger, IMongoClient mongoClient, IConfiguration configuration)
        {

            _logger = logger;
            _mongoClient = mongoClient;

            var dbName = configuration.GetSection("UserDB_MongoDb:DatabaseName").Value;
            _mongoDatabase = _mongoClient.GetDatabase(dbName);

            _supportTransactions = DetectTransactionSupport();
            CheckConnection();

        }

        private bool DetectTransactionSupport()
        {
            var retryCount = 0;
            const int maxRetries = 5;
            const int delayMs = 5;

            while (retryCount < maxRetries)
            {
                var server = _mongoClient.Cluster.Description.Servers.FirstOrDefault();
                if (server != null && server.Type != MongoDB.Driver.Core.Servers.ServerType.Standalone)
                {
                    return server.Type != MongoDB.Driver.Core.Servers.ServerType.Unknown;
                }

                retryCount++;
                Task.Delay(delayMs).Wait();
            }

            return false;
        }

        private void CheckConnection()
        {
            try
            {
                var command = new BsonDocument("ping", 1);
                _mongoDatabase.RunCommand<BsonDocument>(command);
                _logger.LogInformation("MongoDB connection OK.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MongoDB connection failed.");
                throw;
            }
        }



        private async Task<IClientSessionHandle?> BeginTransactionAsync()
        {
            if (!_supportTransactions)
                return null;

            try
            {
                var session = await _mongoClient.StartSessionAsync();
                session.StartTransaction();
                return session;
            }
            catch
            {
                return null;
            }
        }

        public async Task AddUser(UserInfo userInfo)
        {
            if (userInfo == null)
                throw new ArgumentNullException(nameof(userInfo), "User data cannot be null.");

            using var session = await BeginTransactionAsync();

            try
            {

                var collection = _mongoDatabase.GetCollection<UserInfo>("UserInfo");

                if (session != null)
                    await collection.InsertOneAsync(session, userInfo);
                else
                    await collection.InsertOneAsync(userInfo);



                if (session != null)
                    await session.CommitTransactionAsync();


            }
            catch (Exception ex)
            {
                if (session != null)
                    await session.AbortTransactionAsync();

                _logger.LogError(ex, "InsertUsersAsync failed.");
                throw;
            }
        }

        public async Task<UserInfo> GetUser(UserInfo userInfo)
        {
            var collection = _mongoDatabase.GetCollection<UserInfo>("UserInfo");
            return await collection.Find(_ => true).FirstOrDefaultAsync();
        }
    }
}
