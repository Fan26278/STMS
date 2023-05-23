using Common.CustomAttributes;
using STMS.Models.DModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.Models.VModels
{
    [Table("ViewStoreRegionInfos")]
    [PrimaryKey("SRegionId")]
    public class ViewStoreRegionInfo:StoreRegionInfo
    {
        public string StoreName { get; set; }
        public string TemperStateText { get; set; }
    }
}
