using LapsAIDAO.Models;
using LapsAIDAO.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAIDAO.Services
{
    public class LoginService : ILoginService
    {
        private readonly IRepository _repository;


        public LoginService(IRepository repository)
        {
            this._repository = repository;
        }

        public async Task<UserInfo> GetUser(UserInfo userInfo)
        {
            return await _repository.GetUser(userInfo);
        }
    }
}
