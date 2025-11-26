using LapsAIDAO.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Repository
{
    public interface IRepository
    {
        Task AddUser(UserInfo userInfo);

        Task<UserInfo> GetUser(UserInfo userInfo);
    }
}
