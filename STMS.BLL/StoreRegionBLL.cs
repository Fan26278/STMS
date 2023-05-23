using STMS.DAL;
using STMS.Models.DModels;
using STMS.Models.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.BLL
{
    public class StoreRegionBLL
    {
        ViewStoreRegionDAL vsrDAL = new ViewStoreRegionDAL();
        StoreRegionDAL srDAL = new StoreRegionDAL();
        /// <summary>
        /// 条件查询仓库分区列表
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="stateId"></param>
        /// <param name="keywords"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public List<ViewStoreRegionInfo> FindStoreRegionList(int storeId, int stateId, string keywords, bool showDel)
        {
            int isDeleted = showDel ? 1 : 0;
            return vsrDAL.FindStoreRegionList(storeId, stateId, keywords, isDeleted);
        }

        /// <summary>
        /// 获取指定仓库的分区列表
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public List<StoreRegionInfo> GetRegionListByStoreId(int storeId)
        {
            return srDAL.GetRegionListByStoreId(storeId);
        }


        /// <summary>
        /// 判断指定分区是否已添加了产品
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public bool IsRegionAddProducts(int regionId)
        {
            int proCount = srDAL.GetRegionProductCount(regionId);
            if (proCount > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 逻辑删除仓库分区  单个删除
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public int LogicDeleteStoreRegion(int regionId, int storeId)
        {
            int reDel = 0;//失败
            if (IsRegionAddProducts(regionId))
                reDel = 2;//已添加了产品
            else
            {
                bool blDel = srDAL.DeleteStoreRegion(regionId, storeId, 0, 1);
                if (blDel)
                    reDel = 1;//删除成功
                else
                    reDel = 0;
            }
            return reDel;
        }

        /// <summary>
        /// 批量删除分区
        /// </summary>
        /// <param name="regionList"></param>
        /// <returns></returns>
        public string LogicDeleteStoreRegion(List<StoreRegionInfo> regionList)
        {
            string reStr = "";
            foreach (StoreRegionInfo sr in regionList)
            {
                if (reStr.Length > 0)
                    reStr += ",";
                if (IsRegionAddProducts(sr.SRegionId))
                    reStr += sr.SRegionId;
            }
            if (reStr == "")
            {
                reStr = srDAL.DeleteStoreRegionList(regionList, 0, 1) ? "Y" : "0";
            }
            return reStr;
        }

        /// <summary>
        /// 恢复仓库分区信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public bool RecoverStoreRegion(int regionId, int storeId)
        {
            return srDAL.DeleteStoreRegion(regionId, storeId, 0, 0);
        }

        /// <summary>
        /// 真删除仓库分区信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public bool DeleteStoreRegion(int regionId, int storeId)
        {
            return srDAL.DeleteStoreRegion(regionId, storeId, 1, 2);
        }

        /// <summary>
        /// 获取指定的仓库分区信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public StoreRegionInfo GetSRegionInfo(int regionId)
        {
            return srDAL.GetSRegionInfo(regionId);
        }

        /// <summary>
        /// 检查编码或名称是否已存在
        /// </summary>
        /// <param name="sRegionName"></param>
        /// <param name="sRegionNo"></param>
        /// <returns></returns>
        public int ExistsStoreRegion(string sRegionName, string sRegionNo)
        {
            bool[] bls = srDAL.ExistsStoreRegion(sRegionName, sRegionNo);
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
        /// 添加仓库分区
        /// </summary>
        /// <param name="srInfo"></param>
        /// <param name="IsGet"></param>
        /// <returns></returns>
        public int AddStoreRegionWithId(StoreRegionInfo srInfo)
        {
            return srDAL.AddStoreRegion(srInfo, 1);
        }

        public bool AddStoreRegion(StoreRegionInfo srInfo)
        {
            return srDAL.AddStoreRegion(srInfo, 0)>0;
        }

        /// <summary>
        /// 更新仓库分区信息 修改了所属仓库
        /// </summary>
        /// <param name="srInfo"></param>
        /// <param name="oldstoreId"></param>
        /// <returns></returns>
        public bool UpdateStoreRegion(StoreRegionInfo srInfo, int oldstoreId)
        {
            return srDAL.UpdateStoreRegion(srInfo, oldstoreId);
        }

        /// <summary>
        /// 更新仓库分区，没有修改所属仓库
        /// </summary>
        /// <param name="srInfo"></param>
        /// <returns></returns>
        public bool UpdateStoreRegion(StoreRegionInfo srInfo)
        {
            return srDAL.UpdateStoreRegion(srInfo, 0);
        }
    }
}
