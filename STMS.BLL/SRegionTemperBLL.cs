using Communicate.Modbus;
using STMS.DAL;
using STMS.Models.DModels;
using STMS.Models.UIModels;
using STMS.Models.VModels;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STMS.BLL
{
    public  class SRegionTemperBLL
    {
        private StoreRegionDAL srDAL = new StoreRegionDAL();
        private ViewSRegionTemperDAL vsrtDAL = new ViewSRegionTemperDAL();

        /// <summary>
        /// 查询指定仓库的分区列表（分区盒子列表）
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public List<StoreRegionBoxModel> FindRegionBoxList(int storeId)
        {
            List<StoreRegionBoxModel> boxList = new List<StoreRegionBoxModel>();
            List<ViewSRegionTemperInfo> regionList = vsrtDAL.GetSRegionTemperList(storeId);
            //转换为目标类型
            boxList = GetRegionBoxList(regionList);
            return boxList;
        }

        /// <summary>
        /// 将List<ViewSRegionTemperInfo>转换为List<StoreRegionBoxModel> 
        /// </summary>
        /// <param name="regionList"></param>
        /// <returns></returns>
        private List<StoreRegionBoxModel> GetRegionBoxList(List<ViewSRegionTemperInfo> regionList)
        {
            List<StoreRegionBoxModel> boxList = new List<StoreRegionBoxModel>();
            if(regionList!=null)
            {
                int i = 0;
                int id = 0;
                foreach(ViewSRegionTemperInfo info in regionList)
                {
                    string range = "";
                    if (info.AllowHighTemperature == info.AllowLowTemperature && info.AllowLowTemperature == 0)
                    {
                        range = "无";
                    }
                    else
                        range = info.AllowLowTemperature + " ~ " + info.AllowHighTemperature;
                    if(info.SRTemperature>0)
                    {
                        if (info.AllowLowTemperature > info.SRTemperature)
                            info.TemperState = 0;//低温异常
                        else if (info.AllowHighTemperature < info.SRTemperature)
                            info.TemperState = 2;//高温异常
                        else
                            info.TemperState = 1;//正常
                    }
                    //自编号
                    i += 1;
                    if (i > 1)
                        id += 1;
                    boxList.Add(new StoreRegionBoxModel()
                    {
                        SId = id,
                        SRgionId=info.SRegionId,
                        SRegionName=info.SRegionName,
                        SRTemper=info.SRTemperature,
                        MaxTemper=info.AllowHighTemperature,
                        MinTemper=info.AllowLowTemperature,
                        TemperRange=range,
                        TemperState=info.TemperState,
                        ProductCount=info.TotalCount
                    }) ;
                }
            }

            return boxList;
        }

        /// <summary>
        /// 读取分区温度数据（从设备中读取）
        /// </summary>
        /// <returns></returns>
        public List<StoreRegionBoxModel> ReadRegionTemperData()
        {
            List<ViewSRegionTemperInfo> regionTemperList = vsrtDAL.GetSRegionTemperList(0);
            //建立连接，读取数据
            ModbusRTU mbRtu = new ModbusRTU("COM1", 9600, 8, StopBits.One, Parity.None);
            ushort length = (ushort)regionTemperList.Count;
            if(mbRtu.Open())
            {
                regionTemperList   Task.Run(async () =>
                {
                List<ushort> values = await mbRtu.ReadUInt16(0, len: length);
        
                    if (values != null)
                    {
                        if (values.Count > 0)
                        {
                            List<StoreRegionInfo> regions = new List<StoreRegionInfo>();
                            for (int i = 0; i < values.Count; i++)
                            {
                                //当前读取到第i个分区的室温
                                decimal temperVal = decimal.Parse(((decimal)values[i] / 10).ToString("0.00"));
                                StoreRegionInfo regionInfo = new StoreRegionInfo();
                                regionInfo.SRegionId = regionTemperList[i].SRegionId;
                                regionInfo.SRTemperature = temperVal;
                                if (temperVal > 0)
                                {
                                    if (regionTemperList[i].AllowHighTemperature < temperVal)
                                        regionInfo.TemperState = 2;
                                    else if (regionTemperList[i].AllowLowTemperature > temperVal)
                                        regionInfo.TemperState = 0;
                                    else
                                        regionInfo.TemperState = 1;
                                    regions.Add(regionInfo);
                                    regionTemperList[i].TemperState = regionInfo.TemperState;
                                    regionTemperList[i].SRTemperature = temperVal;
                                }
                            }
                            //将读取到的分区室温数据更新到数据库
                            bool blUpdate = srDAL.UpdateSRegionsSRTemperature(regions);
                            if (!blUpdate)
                                return null;
                        }
                        return regionTemperList;
                    }
                    else return null;
                }).Result;
                mbRtu.Close();
            }
            return GetRegionBoxList(regionTemperList);

        }

        /// <summary>
        /// 将调整后的温度写入到设备
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="srTemperature"></param>
        /// <returns></returns>
        public bool SetSRTemperature(int sId,decimal srTemperature)
        {
            bool reBl = false;
            ModbusRTU mbRtu = new ModbusRTU("COM1", 9600, 8, StopBits.One, Parity.None);
            if(mbRtu.Open())
            {
                 reBl= Task.Run(async () => {
                    ushort val = (ushort)(srTemperature * 10);//要写入的值
                    bool bl = await mbRtu.WriteUInt16(sId, val);
                    return bl;
                }).Result;
                mbRtu.Close();
            }
            return reBl;
        }

        /// <summary>
        /// 更新指定仓库分区的正常室温
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="srTemperature"></param>
        /// <returns></returns>
        public bool UpdateSRTemperatureById(int regionId, decimal srTemperature)
        {
            return srDAL.UpdateSRTemperatureById(regionId, srTemperature);
        }

    }
}
