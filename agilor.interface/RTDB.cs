using Agilor.Interface.Val;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Agilor.Interface
{
    /// <summary>
    /// 数采写值接口
    /// </summary>
    public class RTDB:IDisposable
    {
        private string name;
        private string server;
        private int port;
        private bool isOpen = false;

        private static Dictionary<string, RTDB> connections = new Dictionary<string, RTDB>();
        private static readonly object me = new object();

        public delegate void VALUEHANDLER(Value value);

        public event VALUEHANDLER ValueReceived;


        private _Convert.TARGETHANDLER My_On_Target_Arrived;
        private _Convert.TARGETHANDLER My_On_Target_Removed;
        private _Convert.TARGETVALHANDLER My_On_Value_Seted;
        private _Convert.TARGETHANDLER My_On_Value_Geted;


        /// <summary>
        /// RTDB连接状态
        /// </summary>
        public enum States
        {

            /// <summary>
            /// 连接正常
            /// </summary>
            CONNECTED = 1,
            /// <summary>
            /// 正在主动和服务器建立连接
            /// </summary>
            CONNECTING = 2,
            /// <summary>
            /// 正在自动重连
            /// </summary>
            RECONNECTING = 3,
            /// <summary>
            /// 设备处于断连状态
            /// </summary>
            DISCONNECTED = 4
        }

        private RTDB() { }
        private RTDB(string name,string server,int port=700)
        {
            this.name = name;
            this.server = server;
            this.port = port;
        }

        private void Open0() {
            isOpen = true;
        }
        private void close0()
        {
            isOpen = false;
        }

        private string Key
        {
            get { return server + ";" + port + ";" + name; }
        }

        /// <summary>
        /// 建立一个数据连接
        /// </summary>
        /// <param name="name">设备站名称</param>
        /// <param name="server">服务器地址</param>
        /// <param name="port">端口，默认700</param>
        /// <returns>返回一个RTDB连接单例</returns>
        public static RTDB Instance(string name, string server, int port=700)
        {
            lock (me)
            {
                RTDB rtdb = null;

                if (connections.ContainsKey(name))
                    rtdb = connections[name];

                else {
                    //int code = 0;
                    int code = _Convert.DRTDB_RegisterDevice(server, port, name);
                    if (code != 0)
                        throw new Exception("register device error:" + code);
                    rtdb = new RTDB(name, server, port);

                    rtdb.My_On_Target_Arrived = rtdb.On_Target_Arrived;
                    rtdb.My_On_Target_Removed = rtdb.On_Target_Removed;
                    rtdb.My_On_Value_Seted = rtdb.On_Value_Seted;
                    rtdb.My_On_Value_Geted = rtdb.On_Value_Geted;


                    _Convert.DRTDB_MD_SetCallBackFunction(name,rtdb.My_On_Target_Arrived, rtdb.My_On_Target_Removed, rtdb.My_On_Value_Seted, rtdb.My_On_Value_Geted);
                    //_Convert.DRTDB_SetCallBackFunction(new _Convert.TARGETHANDLER(rtdb.On_Target_Arrived), new _Convert.TARGETHANDLER(rtdb.On_Target_Removed), new _Convert.TARGETVALHANDLER(rtdb.On_Value_Seted), new _Convert.TARGETHANDLER(rtdb.On_Value_Geted));
                    connections.Add(name, rtdb);
                }
                rtdb.Open0();
                return rtdb;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            close0();
            lock (me)
            {
                if (isOpen) return;
                if (connections.ContainsKey(this.name))
                {
                    _Convert.DRTDB_MD_UnregisterDevice(this.name, null);
                    connections.Remove(this.name);
                }
            }
        }

        /// <summary>
        /// 写入点值
        /// </summary>
        /// <param name="val">点值</param>
        /// <param name="isImmediate">是否立即发送，true为立即发送，false为缓存</param>
        /// <returns>返回写值操作结果</returns>

        public bool WriteValue(Value val, bool isImmediate = true)
        {
            _Convert.TAG_VALUE_LOCAL data = new _Convert.TAG_VALUE_LOCAL();
            Convert(val, ref data);
            return _Convert.DRTDB_MD_SendNewValue(this.name, ref data, isImmediate) == 0;
        }

        

        /// <summary>
        /// 清空缓存区
        /// </summary>
        public void Flush()
        {
            _Convert.DRTDB_MD_Flush(this.name);
        }

        /// <summary>
        /// 获取当前连接的设备的点数量
        /// </summary>
        /// <returns>返回当前连接的设备的点个数</returns>
        public long GetTagCount()
        {
            int count = 0;
            return _Convert.DRTDB_MD_GetTagCount(this.name, ref count) == 0 ? count : 0;
        }

        /// <summary>
        /// 当前连接状态
        /// </summary>
        /// <returns>返回当前连接状态</returns>
        public States State()
        {
            if (!this.isOpen) return States.DISCONNECTED;
            return (States)_Convert.DRTDB_MD_GetCurrentState(this.name);
        }

        private void On_Target_Arrived(ref _Convert.TAG_NODE node) { }

        private void On_Target_Removed(ref _Convert.TAG_NODE node) { }

        private void On_Value_Seted(ref _Convert.TAG_VALUE_LOCAL data) {
            


            if (this.ValueReceived != null)
            {
                Value dest = null;
                Convert(data, out dest);
                this.ValueReceived(dest);
            }
        
        }

        private void On_Value_Geted(ref _Convert.TAG_NODE val) {
        }





        private static void Convert(Value src, ref _Convert.TAG_VALUE_LOCAL dest)
        {
            dest.lTimeStamp = Utils.ConvertDateTimeInt(src.Time);
            dest.szTagSource = src.Name;
            dest.cTagType = (byte)src.Type;
            dest.nTagState = src.State;
            
            byte[] data = null;
            if (src.Type == Value.Types.BOOL)
                data = BitConverter.GetBytes((bool)src.Val);
            else if (src.Type == Value.Types.FLOAT)
                data= BitConverter.GetBytes((float)src.Val);
            else if (src.Type == Value.Types.LONG)
                data = BitConverter.GetBytes((Int32)src.Val);
            else if (src.Type == Value.Types.STRING)
                data = Encoding.Default.GetBytes((string)src.Val);
            dest.value = new byte[128];
            Array.Copy(data, dest.value, data.Length);
        }

        private static void Convert(_Convert.TAG_VALUE_LOCAL src, out Value dest) {
            dest = new Value(src.szTagSource, null);
            switch (src.cTagType)
            {
                case (int)'L': dest.Val = BitConverter.ToInt32(src.value, 0); break;
                case (int)'B': dest.Val = BitConverter.ToBoolean(src.value, 0); break;
                case (int)'R': dest.Val = BitConverter.ToSingle(src.value, 0); break;
                case (int)'S': dest.Val = Encoding.Default.GetString(src.value); break;
            }
            dest.State = src.nTagState;
            dest.Time = Utils.GetTime(src.lTimeStamp);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (isOpen)
                Close();
        }
    }
}
