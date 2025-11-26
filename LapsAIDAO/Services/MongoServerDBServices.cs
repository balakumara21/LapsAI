using MongoDB.Bson;
using MongoDB.Driver;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public class MongoServerDBServices : IDBService
    {

        private readonly IMongoDatabase _mongoDatabase;

        private readonly IMongoClient _mongoClient;

        private readonly bool _supportTransactions;

        public MongoServerDBServices(IMongoDatabase database, IMongoClient client, QueryFactory dB)
        {
            _mongoDatabase = database;
            _mongoClient = client;
            DB = dB;

            var retryCount = 0;
            const int maxRetries = 5;
            const int delayMilliseconds = 5;
            while (retryCount < maxRetries)
            {
                var server = _mongoClient.Cluster.Description.Servers.FirstOrDefault();
                if (server != null && server.Type != MongoDB.Driver.Core.Servers.ServerType.Standalone)
                {
                    _supportTransactions = server.Type != MongoDB.Driver.Core.Servers.ServerType.Unknown;
                    break;
                }
                retryCount++;
                Task.Delay(delayMilliseconds).Wait();
            }
            //if(retryCount == maxRetries)
            //{
            //    throw new InvalidOperationException("Could not determine server type for MongoDB client.");
            //}
            CheckConnection();
        }

        private void CheckConnection()
        {
            try
            {
                var command = new BsonDocument("ping", 1);
                _mongoDatabase.RunCommand<BsonDocument>(command);
                // _logger.LogInformation("MongoDB connection is healthy.");
            }
            catch (Exception)
            {
                //_logger.LogError("MongoDB connection failed.");
                throw;
            }
        }

        private async Task<IClientSessionHandle?> BeginTransaction()
        {
            if (!_supportTransactions)
            { return null; }
            IClientSessionHandle? session = null;
            try
            {
                session = await _mongoClient.StartSessionAsync();
                session.StartTransaction();
                return session;
            }
            catch (Exception)
            {
                session?.Dispose();
                throw;
            }
        }

        private static async Task CommitTransaction(IClientSessionHandle? session)
        {
            if (session != null)
                await session.CommitTransactionAsync();
        }
        private static async Task AbortTransaction(IClientSessionHandle? session)
        {
            if (session != null)
                await session.AbortTransactionAsync();
        }

        public QueryFactory DB { get; }

        public MongoServerDBServices(string connection)
        {

        }
    }
}
