using Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMP.Models.DModels
{
    /// <summary>
    /// 产品库存类
    /// </summary>
    [Table("ProductStoreInfos")]
    [PrimaryKey("ProStoreId", autoIncrement = true)]
    public class ProductStoreInfo
    {
        public int ProStoreId { get; set; }

        public int ProductId { get; set; }

        public int StoreId { get; set; }

        public int SRegionId { get; set; }
        public int ProductCount { get; set; }
        public int IsDeleted { get; set; }



    }
}
