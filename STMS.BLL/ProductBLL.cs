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
    public  class ProductBLL
    {
        ProductDAL proDAL = new ProductDAL();
        ViewProductStoreDAL vpsDAL = new ViewProductStoreDAL();
        ProductStoreDAL psDAL = new ProductStoreDAL();
        #region 产品信息管理
        /// <summary>
        /// 查询产品列表
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public List<ProductInfo> GetProductList(string keywords, bool  showDel)
        {
            int isDeleted = showDel ? 1 : 0;
            return proDAL.GetProductList(keywords, isDeleted);
        }

        /// <summary>
        /// 获取指定的产品信息
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ProductInfo GetProductInfo(int productId)
        {
            return proDAL.GetProductInfo(productId);
        }


        /// <summary>
        /// 检查指定产品是否已有库存
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool IsProductHasStore(int productId)
        {
            int count = proDAL.GetProductCount(productId);
            if (count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 逻辑删除产品
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public int LogicDeleteProduct(int productId)
        {
            int reDel = 0;//失败
            if (IsProductHasStore(productId))
                reDel = 2;//已有库存
            else
            {
                bool blDel = proDAL.Delete(productId, 0, 1);
                if (blDel)
                    reDel = 1;//删除成功
                else
                    reDel = 0;
            }
            return reDel;
        }

        /// <summary>
        /// 批量逻辑删除产品信息
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns></returns>
        public string LogicDeleteProductList(List<int> productIds)
        {
            string reStr = "";
            foreach (int id in productIds)
            {
                if (reStr.Length > 0)
                    reStr += ",";
                if (IsProductHasStore(id))
                    reStr += id;
            }
            if (reStr == "")
            {
                reStr = proDAL.DeleteList(productIds, 0, 1) ? "Y" : "0";
            }
            return reStr;
        }

        /// <summary>
        /// 恢复产品信息
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool RecoverProduct(int productId)
        {
            return proDAL.Delete(productId, 0, 0);
        }

        /// <summary>
        /// 真删除产品信息
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool DeleteProduct(int productId)
        {
            return proDAL.Delete(productId, 1, 2);
        }

        /// <summary>
        /// 检查编码或名称是否已存在
        /// </summary>
        /// <param name="proName"></param>
        /// <param name="proNo"></param>
        /// <returns></returns>
        public int ExistsProduct(string proName, string proNo)
        {
            bool[] bls = proDAL.ExistsProduct(proName, proNo);
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
        /// 添加产品信息
        /// </summary>
        /// <param name="proInfo"></param>
        /// <param name="isGetId"></param>
        /// <returns></returns>
        public bool AddProductInfo(ProductInfo proInfo)
        {
            return proDAL.AddProductInfo(proInfo, 0)>0;
        }

        public int AddProductInfoWithId(ProductInfo proInfo)
        {
            return proDAL.AddProductInfo(proInfo, 1) ;
        }

        /// <summary>
        /// 修改产品信息
        /// </summary>
        /// <param name="proInfo"></param>
        /// <returns></returns>
        public bool UpdateProductInfo(ProductInfo proInfo)
        {
            return proDAL.UpdateProductInfo(proInfo);
        }
        #endregion

        #region 产品入库


        /// <summary>
        /// 获取所有产品列表  绑定下拉框
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetAllProducts()
        {
            return proDAL.GetAllProducts();
        }

        /// <summary>
        /// 获取所有的产品库存数据
        /// </summary>
        /// <returns></returns>
        public List<ViewProductStoreInfo> GetAllProductStoreList()
        {
            return vpsDAL.GetAllProductStoreList();
        }

        /// <summary>
        /// 产品入库方法
        /// </summary>
        /// <param name="psInfo"></param>
        /// <returns></returns>
        public bool InStoreProducts(ProductStoreInfo psInfo)
        {
            if (psInfo == null)
                throw new Exception("入库信息不能为null");
            if(psDAL.ExistProductStoreRecord(psInfo.ProductId,psInfo.SRegionId))
            {
                //修改产品数量
                return psDAL. UpdateProductStore(psInfo);
            }
            else
            {
                //添加产品库存记录
                return psDAL. AddProductStore(psInfo);
            }
        }
        #endregion

    }
}
