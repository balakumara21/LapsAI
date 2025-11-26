using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public interface IDBService
    {
        QueryFactory DB { get; }

    }
}
