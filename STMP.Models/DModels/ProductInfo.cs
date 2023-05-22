using Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMP.Models.DModels
{
    /// <summary>
    /// 产品实体类
    /// </summary>
    [Table("ProductInfos")]
    [PrimaryKey("SRegionId", autoIncrement = true)]
    public class ProductInfo
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductNo { get; set; }
        public decimal FitLowTemperature { get; set; }
        public decimal FitHighTemperature { get; set; }
        public int IsDeleted { get; set; }

    }
}
