using Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMP.Models.DModels
{
    /// <summary>
    /// 产品入库信息
    /// </summary>
    [Table("ProductInStoreRecordInfos")]
    [PrimaryKey("RecordId", autoIncrement = true)]
    public class ProductInStoreRecordInfo
    {
        public int RecordId { get; set; }

        public int ProductId { get; set; }
        public int SRegionID { get; set; }
        public int ProductCount { get; set; }
        public DateTime InStoreTime { get; set; }





        public int IsDeleted { get; set;}
    }
}
