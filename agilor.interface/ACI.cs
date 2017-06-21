using Agilor.Interface.Val;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

/*
 * 
 */
namespace Agilor.Interface
{
    
    
    /// <summary>
    /// Agilor ACI操作接口
    /// </summary>
    public class ACI:IDisposable
    {
        private string name;

        private string ip;
        private int port;

        private  bool isOpen = false;
        private static Dictionary<string, IWatcher> watcher_collections = new Dictionary<string, IWatcher>();
        private Thread watcher = null;
        private static readonly object watch_obj = new object();

        private static Dictionary<string, ACI> connections = new Dictionary<string, ACI>();
        private static readonly object me = new object();

        

        /// <summary>
        /// 获取连接节点名称
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        /// <summary>
        /// 获取数据库连接地址
        /// </summary>
        public string IP
        {
            get { return ip; }
        }

        /// <summary>
        /// 获取数据库的连接端口
        /// </summary>
        public int Port
        {
            get { return port; }
        }

        private ACI() { }
        private ACI(string name, string ip, int port)
        {
            this.name = name;
            this.ip = ip;
            this.port = port;
            this.watcher = new Thread(WatcherHandler);
            this.watcher.Start(this);
        }

        private int threadId = -1;
        private int ThreadId
        {
            get { while (threadId <= 0) { Thread.Sleep(1); };return threadId; }
        }
        /// <summary>
        /// 建立ACI连接
        /// </summary>
        /// <param name="name">连接名称</param>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        /// <returns>返回一个ACI连接的单例</returns>
        public static ACI Instance(string name, string ip, int port = 900)
        {
            lock (me)
            {
                if(connections.Count()==0)
                    _Convert.Agcn_Startup();

                name = name.Replace('.', '-');

                string key = ip + ";" + port + ";" + name;
                //存在key
                ACI aci = null;
                if (connections.ContainsKey(key))
                    aci = connections[key];
                else
                {
                    if (aci != null)
                        return aci;
                    aci = new ACI(name, ip, port);
                    if (!_Convert.Agcn_CreateNode(name, IntPtr.Zero, aci.ThreadId))
                        throw new Exception("create connect node error");

                    int error = _Convert.Agcn_Connect(name, ip, name, name, (uint)port);
                    if (error > 0)
                        throw new Exception("connect " + ip + ":" + port + "error:" + error);
                    connections.Add(key, aci);
                }
                aci.Open0();
                return aci;
            }
        }

        private void Open0()
        {
            isOpen = true;
        }
        private void Close0()
        {
            isOpen = false;
        }

        

        ///<summary>
        /// 关闭连接
        ///</summary>
        public void Close()
        {
            Close0();
            lock (me)
            {
                if (isOpen) return;
                if (connections.ContainsKey(getKey()))
                {
                    _Convert.PostThreadMessage(threadId,_Convert.WM_QUIT,0,0);
                    _Convert.Agcn_Disconnect(this.name);
                    connections.Remove(getKey());
                    watcher.Join();
                    if (connections.Count == 0)
                    {
                        _Convert.Agcn_Cleanup();
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有设备信息
        /// </summary>
        /// <returns>设备列表</returns>
        public List<Device> getDevices()
        {
            List<Device> result = new List<Device>();
            int handle = _Convert.Agpt_QueryDeviceInfo(this.name);

            while (handle > 0)
            {
                int deviceId = 0;
                _Convert.struDevice_Info data = new _Convert.struDevice_Info();
                if (_Convert.Agpt_EnumDeviceInfo(handle, out deviceId, ref data))
                {
                    Device it = new Device();
                    it.Name = data.szDeviceName;
                    it.IsOnline = data.bIsOnline;
                    result.Add(it);
                }
                else
                    break;
            }
            return result;
        }

        /// <summary>
        /// 根据指定设备下的所有点名
        /// </summary>
        /// <param name="deviceName">设备名</param>
        /// <returns>包含基础信息的点的列表</returns>
        public List<TargetSimple> getTargetsByDevice(string deviceName)
        {
            int handle = _Convert.Agpt_QueryTagsbyDevice(this.name, deviceName);
            return enumTargetSimpleList(handle);
        }


        /// <summary>
        /// 根据ID获取点名
        /// </summary>
        /// <param name="targetId">点名</param>
        /// <returns>点名</returns>
        public string getTargetNameById(int targetId)
        {
            StringBuilder str = new StringBuilder(128);
            _Convert.Agpt_GetTagNamebyID(targetId, str);
            return str.ToString();
        }

        /// <summary>
        /// 模糊查询点
        /// </summary>
        /// <param name="targetNameMask">点名，支持?(代替小于等于1个任意字符),*(代替任意个字符)</param>
        /// <returns>包含基础信息的点的列表</returns>
        public List<TargetSimple> getTargetsbyNameMask(string targetNameMask)
        {
            List<TargetSimple> result = new List<TargetSimple>();

            int handle = _Convert.Agpt_QueryTagsbyNameMask(this.name, targetNameMask);
            return enumTargetSimpleList(handle);

        }

        /// <summary>
        /// 删除点
        /// </summary>
        /// <param name="targetId">点ID</param>
        /// <returns>返回删除操作结果</returns>
        public bool removeTarget(int targetId)
        {
            return _Convert.Agpt_RemoveTag(this.name, targetId) == 0;
        }


        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="node">点信息</param>
        /// <param name="isOverride">如果存在,是否覆盖</param>
        /// <returns>返回添加点操作结果</returns>
        public bool addTarget(Target node, bool isOverride = true)
        {
            _Convert.struTagNode data = new _Convert.struTagNode();
            Convert(node, ref data);
            return _Convert.Agpt_AddNewTag(this.name, ref data, isOverride) == 0;
        }


        /// <summary>
        /// 获取当前点集合的值
        /// </summary>
        /// <param name="targetNames">点集合</param>
        /// <returns>值列表</returns>
        public List<Value> QuerySnapshots(string[] targetNames)
        {
            int handle = _Convert.Agda_QuerySnapshots(this.name, buildtargertNameStr(targetNames), (uint)targetNames.Length);
            return enumTargetValues(handle);
        }

        /// <summary>
        /// 获取指定点的当前值
        /// </summary>
        /// <param name="targetName">指定的点名</param>
        /// <returns>指定点的当前值</returns>
        public Value QuerySnapshots(string targetName)
        {
            List<Value> result = QuerySnapshots(new string[] { targetName });
            return result.Count > 0 ? result[0] : null;
        }


        /// <summary>
        /// 获取某个点的历史值
        /// </summary>
        /// <param name="targetName">点名</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="step">步长(即两个值的间隔，单位:秒)</param>
        /// <returns>返回指定点的历史值列表</returns>
        public List<Value> QueryTagHistory(string targetName, DateTime startTime, DateTime endTime, int step = 0)
        {
            int handle = _Convert.Agda_QueryTagHistory(this.name + "." + targetName, Utils.ConvertDateTimeInt(startTime.AddSeconds(1)), Utils.ConvertDateTimeInt(endTime), step);
            return enumTargetValues(handle);
        }


        //public void SetTargetValue(string name, Value val, bool isManual = false)
        //{
        //    _Convert.struValue data = new _Convert.struValue();

        //    data.Type = (int)val.Type;
        //    switch (val.Type)
        //    {
        //        case Value.Types.BOOL: data.dataValue.bval = (bool)val.Val; break;
        //        case Value.Types.FLOAT: data.dataValue.rval = (float)val.Val; break;
        //        case Value.Types.LONG: data.dataValue.lval = (int)val.Val; break;
        //        //case Value.Types.STRING: data.dataValue.sval = (string)val.Val; break;
        //    }
        //    data.lTimestamp = ConvertDateTimeInt(val.Time);
        //    _Convert.Agda_SetTagValue(this.name + "." + name, ref data, isManual);
        //}

        /// <summary>
        /// 获取点的详细信息
        /// </summary>
        /// <param name="name">点名</param>
        /// <returns>点详细信息</returns>
        public Target GetTarget(string name)
        {
            _Convert.struTagNode node = new _Convert.struTagNode();
            _Convert.Agpt_GetTagInfo(this.name + "." + name, ref node);
            Target data = null;
            Convert(node, out data);
            return data;
        }

        /// <summary>
        /// 设置点值
        /// </summary>
        /// <param name="val"></param>
        public void SetValue(Value val) {
            _Convert.TAGVALUE data = new _Convert.TAGVALUE();
            Convert(val,ref data);
            long res = _Convert.Agda_SetTagValue(this.name + "." + val.Name,ref data, true, null);
            if (res != 0)
                throw new Exception("set value error:" + res);
        }

        ///// <summary>
        ///// 给点添加注释
        ///// </summary>
        ///// <param name="target">点名</param>
        ///// <param name="time">时间</param>
        ///// <param name="comment">注释</param>
        ///// <returns></returns>
        ////public bool AddComment(string target, DateTime time, string comment)
        ////{
        ////    return _Convert.Agda_AddTagComment(this.name + "." + target, Utils.ConvertDateTimeInt(time), 16384, comment) == 0;
        ////}

        private static void WatcherHandler(object obj)
        {
            
            ACI conn = (ACI)obj;
            conn.threadId = _Convert.GetCurrentThreadId();
            _Convert.MSG msg = new _Convert.MSG();
            while (_Convert.GetMessage(ref msg, 0, 0, 0) != 0)
            {
                if (msg.message == _Convert.WM_QUIT) break;
                if (conn.isOpen)
                {
                    if (msg.message == _Convert.WM_SUBDATAARRIVAL)
                    {


                        _Convert.struTagValue s_val = new _Convert.struTagValue();
                        _Convert.Agda_GetSubTagValue(ref s_val);
                        Value val = new Value(s_val.TagName.Substring(conn.Name.Length+1), null);
                        ACI.Convert(s_val, ref val);

                        try
                        {
                            IWatcher handler = watcher_collections[val.Name];
                            handler.OnReceive(val);
                            switch (val.State >> 16)
                            {
                                case _Convert.ALARM_TYPE_HIHILIMIT_MASK: handler.OnHiHiLimit(val); break;
                                case _Convert.ALARM_TYPE_HILIMIT_MASK: handler.OnHiLimit(val); break;
                                case _Convert.ALARM_TYPE_LOLOLIMIT_MASK: handler.OnLoLoLimit(val); break;
                                case _Convert.ALARM_TYPE_LOLIMIT_MASK: handler.OnLoLimit(val); break;
                            }
                        }
                        catch (KeyNotFoundException)
                        {

                        }


                    }
                }
            }
        }

        /// <summary>
        /// 给点添加一个watch
        /// </summary>
        /// <param name="target">点名</param>
        /// <param name="handler">回调函数</param>
        public void Watch(string target, IWatcher handler)
        {
            lock (watch_obj)
            {
                if (watcher_collections.ContainsKey(target))
                    watcher_collections[target] = handler;
                else
                {
                    if (_Convert.Agda_SubscribeTags(this.Name, buildtargertNameStr(new string[] { target }), 1) > 0)
                    {
                        watcher_collections[target] = handler;
                    }
                    else throw new Exception("subscribe faild");
                }
            }
        }

        /// <summary>
        /// 解除点的watch
        /// </summary>
        /// <param name="target">点名</param>
        public void UnWatch(string target)
        {
            lock(watch_obj)
            {
                if(watcher_collections.ContainsKey(target))
                {
                    if (_Convert.Agda_UnSubscribeTags(this.Name, buildtargertNameStr(new string[] { target }), 1) > 0)
                        watcher_collections.Remove(target);
                }
            }
        }

        private List<TargetSimple> enumTargetSimpleList(int handle)
        {
            List<TargetSimple> result = new List<TargetSimple>();

            while (handle > 0)
            {
                int targetId = 0;
                StringBuilder tagName = new StringBuilder(64);
                if (_Convert.Agpt_EnumTagName(handle, ref targetId, tagName))
                {
                    if (tagName.ToString().StartsWith("%#_DeviceState_")) continue;
                    result.Add(new TargetSimple()
                    {
                        Id = targetId,
                        Name = tagName.ToString()
                    });
                }
                else break;
            }
            return result;
        }

        private List<Value> enumTargetValues(int handle)
        {
            List<Value> result = new List<Value>();
            while (handle > 0)
            {
                _Convert.struTagValue data = new _Convert.struTagValue();
                if (_Convert.Agda_GetNextTagValue(handle, ref data, true))
                {
                    Value it = new Value(data.TagName, null);
                    Convert(data, ref it);
                    result.Add(it);
                }
                else break;
            }
            return result;
        }



        private string getKey()
        {
            return ip + ";" + port + ";" + name;
        }

        private static string buildtargertNameStr(string[] targetNames)
        {
            StringBuilder builder = new StringBuilder(targetNames.Length * 64);
            foreach (string it in targetNames)
            {
                byte[] dest = new byte[64];
                Encoding.UTF8.GetBytes(it).CopyTo(dest, 0);
                dest[dest.Length - 1] = 0;
                builder.Append(Encoding.UTF8.GetString(dest));
            }
            return builder.ToString();
        }

        private static void Convert(_Convert.struTagValue src,ref Value dest)
        {
          
            switch (src.TagType)
            {
                case (int)'L': dest.Val = BitConverter.ToInt32(src.value, 0); break;
                case (int)'B': dest.Val = BitConverter.ToBoolean(src.value, 0); break;
                case (int)'R': dest.Val = BitConverter.ToSingle(src.value, 0); break;
                case (int)'S': dest.Val = Encoding.Default.GetString(src.value); break;
        
            }
            dest.State = src.TagState;
            dest.Time = Utils.GetTime(src.Timestamp);
            
        }

        private static void Convert(Target src, ref  _Convert.struTagNode dest)
        {
            dest.name = src.Name;
            dest.pointid = src.Id;
            dest.pointtype = (byte)src.Type;
            dest.descriptor = src.Descriptor;
            dest.scan = (byte)src.Scan;
            dest.creationdate = Utils.ConvertDateTimeInt(src.DateCreated);
            dest.sourcegroup = src.SourceGroup;
            dest.sourceserver = src.Device;
            dest.sourcetag = src.SourceName;
            dest.alarmmax = src.HiLimit;
            dest.alarmhihi = src.HihiLimit;
            dest.alarmmin = src.LoLimit;
            dest.alarmlolo = src.LoloLimit;


            dest.archiving = (byte)(src.Archiving?1:0);//进行历史数据保存
            dest.compressing = (byte)(src.Compressing?1:0);//历史数据压缩

        }

        private static void Convert(_Convert.struTagNode src, out Target dest)
        {
            dest = new Target((Value.Types)src.pointtype);
            dest.Name = src.name;
            dest.Id = src.pointid;
            dest.Descriptor = src.descriptor;
            dest.Scan = (Target.Status)src.scan;
            dest.DateCreated = Utils.GetTime(src.timedate);
            dest.SourceName = src.sourcetag;
            dest.SourceGroup = src.sourcegroup;
            dest.Device = src.sourceserver.ToString();
            dest.HihiLimit = src.alarmhihi;
            dest.HiLimit = src.alarmmax;
            dest.LoloLimit = src.alarmlolo;
            dest.LoLimit = src.alarmmin;
            dest.Compressing = src.compressing == 1 ? true : false;
            dest.Archiving = src.archiving == 1 ? true : false;
        }


        private static void Convert(Value src, ref _Convert.TAGVALUE dest) {
            dest.lTimestamp = Utils.ConvertDateTimeInt(src.Time);
    
            dest.type = (byte)src.Type;
            byte[] data = null;
            if (src.Type == Value.Types.BOOL)
                data = BitConverter.GetBytes((bool)src.Val);
            else if (src.Type == Value.Types.FLOAT)
                data = BitConverter.GetBytes((float)src.Val);
            else if (src.Type == Value.Types.LONG)
                data = BitConverter.GetBytes((Int32)src.Val);
            else if (src.Type == Value.Types.STRING)
                data = Encoding.Default.GetBytes((string)src.Val);
            dest.value = new byte[128];
            Array.Copy(data, dest.value, data.Length);
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
