using STMS.DAL;
using STMS.Models.DModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.BLL
{
    public class StoreBLL
    {
        private StoreDAL storeDAL = new StoreDAL();
        /// <summary>
        /// 条件查询仓库信息列表
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="isDel"></param>
        /// <returns></returns>
        public List<StoreInfo> GetStoreInfos(string keywords, bool isDel)
        {
            int isDeleted = isDel ? 1 : 0;
            return storeDAL.GetStoreInfos(keywords, isDeleted);
        }

        public List<StoreInfo> GetAllStores()
        {
            return storeDAL.GetAllStores();
        }

        /// <summary>
        /// 判断指定仓库是否添加了分区
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public bool IsAddRegions(int storeId)
        {
            int regionCount = storeDAL.GetStoreRegionCount(storeId);
            if (regionCount > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 逻辑删除仓库
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public int LogicDeleteStore(int storeId)
        {
            int reDel = 0;//失败
            if (IsAddRegions(storeId))
                reDel = 2;//已添加了分区
            else
            {
                bool blDel= storeDAL.Delete(storeId, 0, 1);
                if (blDel)
                    reDel = 1;//删除成功
                else
                    reDel = 0;
            }
            return reDel;
        }

        /// <summary>
        /// 批量逻辑删除仓库信息
        /// </summary>
        /// <param name="storeIds"></param>
        /// <returns></returns>
        public string LogicDeleteStore(List<int> storeIds)
        {
            string reStr = "";
            foreach(int id in storeIds)
            {
                if (reStr.Length > 0)
                    reStr += ",";
                if (IsAddRegions(id))
                    reStr += id;
            }
           if(reStr=="")
            {
                reStr = storeDAL.DeleteList(storeIds, 0, 1)?"Y":"0";
            }
            return reStr;
        }

        /// <summary>
        /// 恢复仓库信息
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public bool RecoverStore(int storeId)
        {
            return storeDAL.Delete(storeId, 0, 0);
        }

        /// <summary>
        /// 真删除仓库信息
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public bool DeleteStore(int storeId)
        {
            return storeDAL.Delete(storeId, 1, 2);
        }


        /// <summary>
        /// 获取指定的仓库信息
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public StoreInfo GetStore(int storeId)
        {
            return storeDAL.GetStore(storeId);
        }

        /// <summary>
        /// 检查编码或名称是否已存在
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeNo"></param>
        /// <returns></returns>
        public int ExistsStore(string storeName, string storeNo)
        {
            bool[] bls = storeDAL.ExistsStore(storeName, storeNo);
            bool blName = bls[0];
            bool blNo = bls[1];
            if (blName && blNo)
                return 3;//都存在
            else if (blName && (!blNo))
                return 1;//Name存在
            else if (!blName && blNo)
                return 2;//No存在
            else
                return 0;//都不存在
        }

        /// <summary>
        /// 添加仓库信息
        /// </summary>
        /// <param name="storeInfo"></param>
        /// <returns></returns>
        public bool AddStore(StoreInfo storeInfo)
        {
            return storeDAL.AddStore(storeInfo, 0)>0;
        }
        /// <summary>
        /// 添加仓库并返回编号
        /// </summary>
        /// <param name="storeInfo"></param>
        /// <returns></returns>
        public int AddStoreGetId(StoreInfo storeInfo)
        {
            return storeDAL.AddStore(storeInfo, 1);
        }
        /// <summary>
        /// 更新仓库信息
        /// </summary>
        /// <param name="storeInfo"></param>
        /// <returns></returns>
        public bool UpdateStore(StoreInfo storeInfo)
        {
            return storeDAL.UpdateStore(storeInfo);
        }

    }
}
