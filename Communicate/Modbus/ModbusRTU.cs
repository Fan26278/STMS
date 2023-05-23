using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate.Modbus
{
    public  class ModbusRTU:SerialPortBase
    {
        //创建通信对象 Create
        //提供打开通信对象  Open
        //读取数据并返回相应数据

        /// <summary>
        /// 创建串口对象
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="parity"></param>
        public ModbusRTU(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity):base(portName,baudRate,dataBits,stopBits,parity)
        {

        }
        /// <summary>
        /// 读取无符号单精度数据
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="deviceAddr"></param>
        /// <param name="funcCode"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public override async Task<List<ushort>> ReadUInt16(byte startAddr, byte deviceAddr = 1, byte funcCode = 3, ushort len = 1)
        {
            //拼装请求报文  01 03 00 00 00 0A
           List<byte> readBytes =  this.GetReadCommandBytes(deviceAddr, funcCode, startAddr, len);
            //进行RTU的封装----添加一个CRC16校验
            readBytes = CRC16(readBytes);
            //发送报文并返回响应报文
            //serialPort.Write(readBytes.ToArray(), 0, readBytes.Count);
            List<byte> respBytes= await this.Send(readBytes, len);
            //响应报文的校验
           if(respBytes.Count >0)
            {
                //解析数据部分
                respBytes.RemoveRange(0, 3);
                respBytes.RemoveRange(respBytes.Count - 2, 2);
                //进行统一的方法解析
                return this.ReadUInt16(respBytes);
            }
            return null;

        }

        /// <summary>
        /// 写单寄存器
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="val"></param>
        /// <param name="deviceAddr"></param>
        /// <param name="funcCode"></param>
        /// <returns></returns>
        public override async Task<bool> WriteUInt16(int startAddr, ushort val, byte deviceAddr = 1, byte funcCode = 6)
        {
            List<byte> commandBytes = this.GetWriteCommandBytes(deviceAddr, funcCode, startAddr, val);
            commandBytes = CRC16(commandBytes);
            List<byte> respBytes = await this.Send(commandBytes, 0);
            if (respBytes.Count==commandBytes.Count)
                return true;
            else
                return false;
        }

        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="value"></param>
        /// <param name="poly"></param>
        /// <param name="crcInit"></param>
        /// <returns></returns>
        private List<byte> CRC16(List<byte> value, ushort poly = 0xA001, ushort crcInit = 0xFFFF)
        {
            if (value == null || !value.Any())
                throw new ArgumentException("");

            //运算
            ushort crc = crcInit;
            for (int i = 0; i < value.Count; i++)
            {
                crc = (ushort)(crc ^ (value[i]));
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ poly) : (ushort)(crc >> 1);
                }
            }
            byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
            byte lo = (byte)(crc & 0x00FF);         //低位置

            List<byte> buffer = new List<byte>();
            buffer.AddRange(value);
            buffer.Add(lo);
            buffer.Add(hi);
            return buffer;
        }
    }
}
