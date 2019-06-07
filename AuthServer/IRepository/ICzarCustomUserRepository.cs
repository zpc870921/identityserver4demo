using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models;

namespace AuthServer.IRepository
{
    public interface ICzarCustomUserRepository
    {
        /// <summary>
        /// 根据账号密码获取用户实体
        /// </summary>
        /// <param name="uaccount">账号</param>
        /// <param name="upassword">密码</param>
        /// <returns></returns>
        CzarCustomUser FindUserByuAccount(string uaccount, string upassword);

    }
}
