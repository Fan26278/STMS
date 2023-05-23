using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate.Modbus
{
    public abstract class ModbusBase
    {
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public virtual bool Open()
        {
            return true;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Close()
        {

        }
        /// <summary>
        /// 读取无符号单精度数据
        /// </summary>
        /// <param name="startAddr">起始地址</param>
        /// <param name="deviceAddr">从站地址</param>
        /// <param name="funcCode">功能码</param>
        /// <param name="len">读取长度</param>
        /// <returns></returns>
        public virtual Task<List<ushort>> ReadUInt16(byte startAddr, byte deviceAddr = 1, byte funcCode = 3, ushort len = 1)
        {
            return null;
        }
        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="command">报文内容</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public virtual   Task<List<byte>> Send(List<byte> command, int len) { return null; }


        /// <summary>
        /// 组装请求报文
        /// </summary>
        /// <param name="deviceAddr">从站地址</param>
        /// <param name="funcCode">功能码</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="length">读取长度</param>
        /// <returns></returns>
        protected List<byte> GetReadCommandBytes(byte deviceAddr, byte funcCode, byte startAddr, ushort length)
        {
            List<byte> buffer = new List<byte>();
            buffer.Add(deviceAddr); 
            buffer.Add(funcCode);
            buffer.Add(BitConverter.GetBytes(startAddr)[1]);
            buffer.Add(BitConverter.GetBytes(startAddr)[0]);
            buffer.Add(BitConverter.GetBytes(length)[1]);
            buffer.Add(BitConverter.GetBytes(length)[0]);
            return buffer;
        }

        /// <summary>
        /// 组装写入单寄存器报文
        /// </summary>
        /// <param name="deviceAddr">1</param>
        /// <param name="funcCode">5</param>
        /// <param name="startAddr"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        protected List<byte> GetWriteCommandBytes(byte deviceAddr, byte funcCode, int startAddr, ushort val)
        {
            List<byte> buffer = new List<byte>();
            buffer.Add(deviceAddr);
            buffer.Add(funcCode);
            buffer.Add(BitConverter.GetBytes(startAddr)[1]);
            buffer.Add(BitConverter.GetBytes(startAddr)[0]);
            buffer.Add(BitConverter.GetBytes(val)[1]);
            buffer.Add(BitConverter.GetBytes(val)[0]);
            return buffer;
        }

        /// <summary>
        /// 解析响应数据 读取无符号整数
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected  List<ushort> ReadUInt16(List<byte> result)
        {
            List<ushort> values = new List<ushort>();
            for (int i = 0; i < result.Count; i++)
            {
                List<byte> valueByte = new List<byte>();
                valueByte.Add(result[i]);
                valueByte.Add(result[++i]);
                valueByte.Reverse();
                ushort value = BitConverter.ToUInt16(valueByte.ToArray(), 0);
                values.Add(value);
            }
            return values;
        }

        /// <summary>
        /// 写入寄存器
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="val"></param>
        /// <param name="deviceAddr"></param>
        /// <param name="funcCode"></param>
        /// <returns></returns>
        public virtual Task<bool> WriteUInt16(int startAddr, ushort val,byte deviceAddr = 1, byte funcCode = 6)
        {
            return null;
        }

    }
}
