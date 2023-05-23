using STMS.Models.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DAL
{
    public class ViewProductStoreDAL:BQuery<ViewProductStoreInfo>
    {
        /// <summary>
        /// 获取所有的产品库存数据
        /// </summary>
        /// <returns></returns>
          public List<ViewProductStoreInfo> GetAllProductStoreList()
        {
            List<ViewProductStoreInfo> list = new List<ViewProductStoreInfo>();
            string cols = "ProStoreId,ProductName,StoreName,SRegionName,ProductCount";
            list = GetModelList(cols, 0);
            return list;
        }
    }
}
