using Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.Models.DModels
{
    /// <summary>
    /// 产品入库记录信息实体
    /// </summary>
    [Table("ProductInStoreRecordInfos")]
    [PrimaryKey("RecordId")]
    public  class ProductInStoreRecordInfo
    {
        public int RecordId { get; set; }
        public int ProductId { get; set; }
        public int SRegionId { get; set; }
        public int ProductCount { get; set; }
        public DateTime InStoreTime { get; set; }
        public int IsDeleted { get; set; }
    }
}
