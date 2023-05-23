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
    public class ProductDAL:BaseDAL<ProductInfo>
    {
        /// <summary>
        /// 查询产品列表
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public List<ProductInfo> GetProductList(string keywords,int isDeleted)
        {
            string cols = CreateSql.GetColsString<ProductInfo>("IsDeleted");
            string strWhere = $"IsDeleted={isDeleted}";
            SqlParameter para = null;
            if(!string.IsNullOrEmpty(keywords))
            {
                strWhere += " and (ProductName like @keywords or ProductNo like @keywords)";
                para = new SqlParameter("@keywords", $"%{keywords}%");
                return GetModelList(strWhere, cols, para);
            }
            return GetModelList(strWhere, cols);
        }

        /// <summary>
        /// 获取指定的产品信息
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ProductInfo GetProductInfo(int productId)
        {
            string cols= CreateSql.GetColsString<ProductInfo>("IsDeleted");
            return GetById(productId, cols);
        }

        /// <summary>
        /// 获取所有产品列表  绑定下拉框
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetAllProducts()
        {
            return  GetModelList("ProductId,ProductName", 0);
        }

        /// <summary>
        /// 获取指定产品的库存数
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public int GetProductCount(int productId)
        {
            string sql = "select sum(ProductCount) from ProductStoreInfos where ProductId=" + productId;
            object oCount= SqlHelper.ExecuteScalar(sql, 1);
            if (oCount != null && oCount.ToString() != "")
                return oCount.GetInt();
            return 0;
        }

        /// <summary>
        /// 检查编码或名称是否已存在
        /// </summary>
        /// <param name="proName"></param>
        /// <param name="proNo"></param>
        /// <returns></returns>
        public bool[] ExistsProduct(string proName, string proNo)
        {
            bool blName = false;
            if (!string.IsNullOrEmpty(proName))
                blName = ExistsByName("ProductName", proName);
            bool blNo = false;
            if (!string.IsNullOrEmpty(proNo))
                blNo = ExistsByName("ProductNo", proNo);
            return new bool[] { blName, blNo };
        }

        /// <summary>
        /// 添加产品信息
        /// </summary>
        /// <param name="proInfo"></param>
        /// <param name="isGetId"></param>
        /// <returns></returns>
        public int AddProductInfo(ProductInfo proInfo,int isGetId)
        {
            string cols = CreateSql.GetColsString<ProductInfo>("ProductId,IsDeleted");
            return Add(proInfo, cols, isGetId);
        }

        /// <summary>
        /// 修改产品信息
        /// </summary>
        /// <param name="proInfo"></param>
        /// <returns></returns>
        public bool UpdateProductInfo(ProductInfo proInfo)
        {
            string cols = CreateSql.GetColsString<ProductInfo>("IsDeleted");
            return Update(proInfo, cols);
        }
    }
}
