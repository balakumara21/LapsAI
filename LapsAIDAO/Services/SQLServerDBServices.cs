using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public class SQLServerDBServices : IDBService
    {
        public QueryFactory DB { get; }

        public SQLServerDBServices(string connection)
        {
            var con = new SqlConnection(connection);
            var compiler = new SqlServerCompiler();
            DB = new QueryFactory(con, compiler);
        }
    }
}
