using LapsAIDAO.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public interface ILoginService
    {
        Task<UserInfo> GetUser(UserInfo userInfo);
    }
}
