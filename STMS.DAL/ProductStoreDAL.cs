using STMS.DbUtility;
using STMS.Models.DModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.DAL
{
    public  class ProductStoreDAL:BaseDAL<ProductStoreInfo>
    {
        /// <summary>
        /// 检查指定产品在指定分区中是否有库存记录
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="sRegionId"></param>
        /// <returns></returns>
        public bool ExistProductStoreRecord(int productId,int sRegionId)
        {
            string strWhere = $"ProductId={productId} and SRegionId={sRegionId}";
            return Exists(strWhere);
        }

        /// <summary>
        /// 添加产品库存记录
        /// </summary>
        /// <param name="psInfo"></param>
        /// <returns></returns>
        public bool AddProductStore(ProductStoreInfo psInfo)
        {
            //添加产品库存记录
            //添加本次入库的入库记录
            string cols = CreateSql.GetColsString<ProductStoreInfo>("IsDeleted");
            string reCols = CreateSql.GetColsString<ProductInStoreRecordInfo>("InStoreTime,IsDeleted");
            SqlModel instoreModel = CreateSql.GetInsertSqlAndParas<ProductStoreInfo>(psInfo, cols, 0);
            List<CommandInfo> comList = new List<CommandInfo>();
            comList.Add(new CommandInfo()
            {
                CommandText = instoreModel.Sql,
                IsProc = false,
                Paras = instoreModel.SqlParaArray
            });
            //入库记录信息
            ProductInStoreRecordInfo recordInfo = new ProductInStoreRecordInfo()
            {
                ProductId = psInfo.ProductId,
                SRegionId = psInfo.SRegionId,
                ProductCount = psInfo.ProductCount
            };
            SqlModel inRecordModel = CreateSql.GetInsertSqlAndParas<ProductInStoreRecordInfo>(recordInfo, reCols, 0);
            comList.Add(new CommandInfo()
            {
                CommandText = inRecordModel.Sql,
                IsProc = false,
                Paras = inRecordModel.SqlParaArray
            });
            return SqlHelper.ExecuteTrans(comList);
        }

        /// <summary>
        /// 修改产品库存数量
        /// </summary>
        /// <param name="psInfo"></param>
        /// <returns></returns>
        public bool UpdateProductStore(ProductStoreInfo psInfo)
        {
            //修改产品数量
            //添加本次入库的入库记录
            string reCols = CreateSql.GetColsString<ProductInStoreRecordInfo>("InStoreTime,IsDeleted");
            string upSql = $"update ProductStoreInfos set ProductCount=ProductCount+{psInfo.ProductCount} where ProductId={psInfo.ProductId} and SRegionId={psInfo.SRegionId}";
            List<CommandInfo> comList = new List<CommandInfo>();
            comList.Add(new CommandInfo()
            {
                CommandText =upSql,
                IsProc = false
            });
            //入库记录信息
            ProductInStoreRecordInfo recordInfo = new ProductInStoreRecordInfo()
            {
                ProductId = psInfo.ProductId,
                SRegionId = psInfo.SRegionId,
                ProductCount = psInfo.ProductCount
            };
            SqlModel inRecordModel = CreateSql.GetInsertSqlAndParas<ProductInStoreRecordInfo>(recordInfo, reCols, 0);
            comList.Add(new CommandInfo()
            {
                CommandText = inRecordModel.Sql,
                IsProc = false,
                Paras = inRecordModel.SqlParaArray
            });
            return SqlHelper.ExecuteTrans(comList);
        }
    }
}
