using STMS.Models.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DAL
{
    public class ViewSRegionTemperDAL:BQuery<ViewSRegionTemperInfo>
    {
        /// <summary>
        /// 查询指定仓库的分区列表
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public List<ViewSRegionTemperInfo> GetSRegionTemperList(int storeId)
        {
            string strWhere = "1=1";
            if(storeId>0)
            {
                strWhere += $" and StoreId={storeId}";
            }
            strWhere += " order by StoreId,SRegionId";
            string cols = CreateSql.GetColsString<ViewSRegionTemperInfo>("TemperRange");
            return GetModelList(strWhere, cols);
        }
    }
}
