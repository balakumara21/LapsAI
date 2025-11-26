using LapsAIDAO.Repository;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public class PostgresDatabaseFactory : IDatabaseFactory
    {
        private readonly string _connection;

        public PostgresDatabaseFactory(string connection)
        {
            _connection = connection;
        }

        public IRepository CreateUserRepository()
        {
            return new SqlUserRepository(
                _connection,
                new PostgresCompiler()
            );
        }
    }
}
