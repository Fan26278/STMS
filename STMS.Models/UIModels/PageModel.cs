using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.Models.UIModels
{
        /// <summary>
        /// 分页模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
    public class PageModel<T>
    {
        public int TotalCount { get; set; }
        public List<T> ReList { get; set; }

    }
}
