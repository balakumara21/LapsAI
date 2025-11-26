using LapsAIDAO.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public interface IDatabaseFactory
    {
        IRepository CreateUserRepository();
    }
}
