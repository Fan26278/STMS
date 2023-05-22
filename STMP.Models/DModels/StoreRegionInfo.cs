using Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMP.Models.DModels
{
    /// <summary>
    /// 仓库分区实体类
    /// </summary>
    [Table("StoreRegionInfos")]
    [PrimaryKey("SRegionId",autoIncrement =true)]
    public class StoreRegionInfo
    {
        public int SRegionId { get; set; }
        public string SRegionName { get; set; }
        public string SRegionNo { get; set; }
        public int StoreId { get; set; }
        public decimal SRTemperature { get; set; }
        public decimal AllowLowTemperature { get; set; }
        public decimal AllowHighTemperature { get; set; }
        public int TmperState { get; set; }
        public string Remark { get; set; }
        public int IsDeleted { get; set; }
    }
}
