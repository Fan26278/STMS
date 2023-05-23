using Common;
using STMS.DbUtility;
using STMS.Models.DModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DAL
{
    public  class StoreRegionDAL:BaseDAL<StoreRegionInfo>
    {
        /// <summary>
        /// 仓库分区的删除处理（真假删除、恢复处理）
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="storeId"></param>
        /// <param name="delType"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public bool DeleteStoreRegion(int regionId,int storeId,int delType,int isDeleted)
        {
            List<string> sqlList = GetDelRegionSqlList(regionId, storeId, delType, isDeleted);
            return   SqlHelper.ExecuteTrans(sqlList);
        }

        /// <summary>
        /// 批量删除 分区
        /// </summary>
        /// <param name="regionList"></param>
        /// <param name="delType"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public bool DeleteStoreRegionList(List<StoreRegionInfo> regionList, int delType, int isDeleted)
        {
            List<string> sqlList = new List<string>();
            foreach (StoreRegionInfo sr in regionList)
            {
                sqlList.AddRange(GetDelRegionSqlList(sr.SRegionId, sr.StoreId, delType, isDeleted));
            }
            return SqlHelper.ExecuteTrans(sqlList);
        }

        /// <summary>
        /// 生成删除分区批量Sql
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="storeId"></param>
        /// <param name="delType"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        private List<string> GetDelRegionSqlList(int regionId, int storeId, int delType, int isDeleted)
        {
            string strWhere = $"SRegionId={regionId}";
            string sqlDel = "";
            if (delType == 1)
                sqlDel = CreateSql.CreateDeleteSql<StoreRegionInfo>(strWhere);
            else
            {
                sqlDel = CreateSql.CreateLogicDeleteSql<StoreRegionInfo>(strWhere, isDeleted);
            }
            //修改分区数的sql
            string flag = "+";
            if (isDeleted >= 1)
                flag = "-";
            string sqlUpdate = $"update StoreInfos set RegionCount = RegionCount{flag}1 where StoreId=" + storeId;
            List<string> sqlList = new List<string>();
            sqlList.Add(sqlDel);
            sqlList.Add(sqlUpdate);
            return sqlList;
        }

        /// <summary>
        /// 获取指定分区中的产品数量
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public int GetRegionProductCount(int regionId)
        {
            string sql = $"select sum(ProductCount) from ProductStoreInfos where SRegionId={regionId} and IsDeleted=0";
            object oCount= SqlHelper.ExecuteScalar(sql, 1);
            if (oCount != null && oCount.ToString() != "")
                return oCount.GetInt();
            return 0;
        }

        /// <summary>
        /// 获取指定的仓库分区信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public StoreRegionInfo GetSRegionInfo(int regionId)
        {
            string cols = CreateSql.GetColsString<StoreRegionInfo>("IsDeleted");
            return GetById(regionId, cols);
;        }

        /// <summary>
        /// 检查编码或名称是否已存在
        /// </summary>
        /// <param name="sRegionName"></param>
        /// <param name="sRegionNo"></param>
        /// <returns></returns>
        public bool[] ExistsStoreRegion(string sRegionName, string sRegionNo)
        {
            bool blName = false;
            if (!string.IsNullOrEmpty(sRegionName))
                blName = ExistsByName("SRegionName", sRegionName);
            bool blNo = false;
            if (!string.IsNullOrEmpty(sRegionNo))
                blNo = ExistsByName("SRegionNo", sRegionNo);
            return new bool[] { blName, blNo };
        }

        /// <summary>
        /// 获取指定仓库的分区列表
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public List<StoreRegionInfo> GetRegionListByStoreId(int storeId)
        {
            string strWhere = "StoreId=" + storeId + " and IsDeleted=0";
            return GetModelList(strWhere, "SRegionId,SRegionName");
        }

        /// <summary>
        /// 添加仓库分区
        /// </summary>
        /// <param name="srInfo"></param>
        /// <param name="IsGet"></param>
        /// <returns></returns>
        public int AddStoreRegion(StoreRegionInfo srInfo,int IsGet)
        {
            string cols = CreateSql.GetColsString<StoreRegionInfo>("SRegionId,AllowLowTemperature,AllowHighTemperature,IsDeleted");
            //修改仓库的分区数
            return SqlHelper.ExecuteTrans<int>(cmd =>
            {
                try {
                    //入库分区信息
                    SqlModel inModel = CreateSql.GetInsertSqlAndParas<StoreRegionInfo>(srInfo, cols, IsGet);
                    cmd.CommandText = inModel.Sql;
                    SqlHelper.AddParas(cmd, inModel.SqlParaArray);
                    int re = 0;
                    if (IsGet == 1)
                    {
                        object oId = cmd.ExecuteScalar();
                        re = oId.GetInt();
                    }
                    else
                        re = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    if (re > 0)
                    {
                        //修改仓库分区数
                        string upsql = "update StoreInfos set RegionCount=RegionCount+1 where StoreId=" + srInfo.StoreId;
                        cmd.CommandText = upsql;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                    return re;
                }
                catch(SqlException ex)
                {
                    cmd.Transaction.Rollback();
                    throw new Exception("执行添加仓库分区事务异常！", ex);
                }
              
            });
        }

        /// <summary>
        /// 更新仓库分区信息
        /// </summary>
        /// <param name="srInfo"></param>
        /// <param name="oldstoreId"></param>
        /// <returns></returns>
        public bool UpdateStoreRegion(StoreRegionInfo srInfo,int oldstoreId)
        {
            string cols = CreateSql.GetColsString<StoreRegionInfo>("AllowLowTemperature,AllowHighTemperature,IsDeleted");
            if (srInfo.AllowLowTemperature != null)
                cols += ",AllowLowTemperature";
            if(srInfo.AllowHighTemperature != null)
                cols += ",AllowHighTemperature";
            //修改仓库的分区数
            return SqlHelper.ExecuteTrans(cmd =>
            {
                try
                {
                    //更新分区信息
                    SqlModel upModel = CreateSql.GetUpdateSqlAndParas<StoreRegionInfo>(srInfo, cols,"");
                    cmd.CommandText = upModel.Sql;
                    SqlHelper.AddParas(cmd, upModel.SqlParaArray);
                    int re = 0;
                    re = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    if (re > 0&&oldstoreId>0)
                    {
                        //修改仓库分区数
                        string upsql = "update StoreInfos set RegionCount=RegionCount+1 where StoreId=" + srInfo.StoreId;
                        cmd.CommandText = upsql;
                        cmd.ExecuteNonQuery();
                        string upsql1 = "update StoreInfos set RegionCount=RegionCount-1 where StoreId=" + oldstoreId;
                        cmd.CommandText = upsql1;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    cmd.Transaction.Rollback();
                    throw new Exception("执行更新仓库分区事务异常！", ex);
                }
            });
        }

        /// <summary>
        /// 批量更新分区室温
        /// </summary>
        /// <param name="regions"></param>
        /// <returns></returns>
        public bool UpdateSRegionsSRTemperature(List<StoreRegionInfo> regions)
        {
            List<string> sqlList = new List<string>();
            foreach(StoreRegionInfo info in regions)
            {
                string sql = $"update StoreRegionInfos set SRTemperature={info.SRTemperature},TemperState={info.TemperState} where SRegionId={info.SRegionId}";
                sqlList.Add(sql);
            }
            return SqlHelper.ExecuteTrans(sqlList);
        }

        /// <summary>
        /// 更新指定仓库分区的正常室温
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="srTemperature"></param>
        /// <returns></returns>
        public bool UpdateSRTemperatureById(int regionId,decimal srTemperature)
        {
            string sql = $"update StoreRegionInfos set SRTemperature={srTemperature},TemperState=1 where SRegionId={regionId}";
            return SqlHelper.ExecuteNonQuery(sql, 1)>0;
        }
    }
}
