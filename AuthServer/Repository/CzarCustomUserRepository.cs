using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.IRepository;
using AuthServer.Models;
using Dapper;
using Microsoft.Extensions.Options;

namespace AuthServer.Repository
{
    public class CzarCustomUserRepository : ICzarCustomUserRepository
    {
        private readonly string DbConn = "";
        public CzarCustomUserRepository(IOptions<CzarConfig> czarConfig)
        {
            DbConn = czarConfig.Value.DbConnectionStrings;
        }

        /// <summary>
        /// 根据账号密码获取用户实体
        /// </summary>
        /// <param name="uaccount">账号</param>
        /// <param name="upassword">密码</param>
        /// <returns></returns>
        public CzarCustomUser FindUserByuAccount(string uaccount, string upassword)
        {
            using (var connection = new SqlConnection(DbConn))
            {
                string sql = @"SELECT * from CzarCustomUser where username=@uaccount and userpwd=upassword ";
                var result = connection.QueryFirstOrDefault<CzarCustomUser>(sql, new { uaccount, upassword });
                return result;
            }
        }
    }
}
