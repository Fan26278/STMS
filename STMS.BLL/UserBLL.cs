using STMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.BLL
{
    public class UserBLL
    {
        UserDAL userDAL = new UserDAL();
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="uName"></param>
        /// <param name="uPwd"></param>
        /// <returns></returns>
        public bool LoginSystem(string uName, string uPwd)
        {
            int userId = userDAL.LoginSystem(uName, uPwd);
            if (userId > 0)
                return true;
            else
                return false;
        }
    }
}
