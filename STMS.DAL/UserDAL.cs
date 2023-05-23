using STMS.Models.DModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DAL
{
    public class UserDAL:BaseDAL<UserInfo>
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="uName"></param>
        /// <param name="uPwd"></param>
        /// <returns></returns>
        public int LoginSystem(string uName,string uPwd)
        {
            string strWhere = "UserName=@userName and UserPwd=@userPwd and UserState=1 and IsDeleted=0";
            SqlParameter[] paras =
            {
                new SqlParameter("@userName",uName),
                 new SqlParameter("@userPwd",uPwd)
            };
            UserInfo user = GetModel(strWhere, "UserId", paras);
            if (user != null)
                return user.UserId;
            else
                return 0;
        }
    }
}
