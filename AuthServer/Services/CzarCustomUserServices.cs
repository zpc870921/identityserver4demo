using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.IRepository;
using AuthServer.IServices;
using AuthServer.Models;

namespace AuthServer.Services
{
    public class CzarCustomUserServices : ICzarCustomUserServices
    {
        private readonly ICzarCustomUserRepository czarCustomUserRepository;
        public CzarCustomUserServices(ICzarCustomUserRepository czarCustomUserRepository)
        {
            this.czarCustomUserRepository = czarCustomUserRepository;
        }

        /// <summary>
        /// 根据账号密码获取用户实体
        /// </summary>
        /// <param name="uaccount">账号</param>
        /// <param name="upassword">密码</param>
        /// <returns></returns>
        public CzarCustomUser FindUserByuAccount(string uaccount, string upassword)
        {
            return czarCustomUserRepository.FindUserByuAccount(uaccount, upassword);
        }
    }
}
