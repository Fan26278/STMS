using Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.Models.DModels
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    //应用特性的方式
   [Table("UserInfos")]
   [PrimaryKey("UserId",autoIncrement =true)]
    public  class UserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public int UserState { get; set; }
        public int IsDeleted { get; set; }
    }
}
