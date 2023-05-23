using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.Models.UIModels
{
    public class StoreRegionBoxModel
    {
        /// <summary>
        /// 分区编号
        /// </summary>
        public int SRgionId { get; set; }

        /// <summary>
        /// 自编号
        /// </summary>
        public int SId { get; set; }
        /// <summary>
        /// 仓库分区名称
        /// </summary>
        public string SRegionName { get; set; }

        /// <summary>
        /// 设置按钮的显示
        /// </summary>
        public bool BtnSetVisible { get; set; }

        /// <summary>
        /// 设置按钮的文本
        /// </summary>
        public string BtnSetText { get; set; }

        /// <summary>
        /// 该仓库分区的温度状态
        /// </summary>
        public int TemperState { get; set; }


        /// <summary>
        /// 指示灯颜色
        /// </summary>
        public Color StateColor { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// 室内温度
        /// </summary>
        public decimal SRTemper { get; set; }

        /// <summary>
        /// 高温线值
        /// </summary>
        public decimal MaxTemper { get; set; }

        /// <summary>
        /// 低温线值
        /// </summary>
        public decimal MinTemper { get; set; }

        /// <summary>
        /// 室内温度范围
        /// </summary>
        public string TemperRange { get; set; }
    }
}
