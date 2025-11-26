using LapsAIDAO.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAI.Shared.Services
{
    public interface IAPIGateway
    {
        Task<bool> postloginData(UserInfo userInfo);

        Task<string> GetToken(string ApiKey);

        Task<string> GetAServiceValue();

    }
}
