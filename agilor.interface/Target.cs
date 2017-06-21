using Agilor.Interface.Val;
using System;

namespace Agilor.Interface
{
    /// <summary>
    /// 点数据类
    /// </summary>
    public class Target
    {
        private Target()
        {
            DateCreated = DateTime.Now;
        }

        /// <summary>
        /// 点状态，通常为 IN
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// 禁止
            /// </summary>
            INVALID = 0,
            /// <summary>
            /// 输入
            /// </summary>
            IN = 1,
            /// <summary>
            /// 输出
            /// </summary>
            OUT = 2,
        }

        /// <summary>
        /// 点类型
        /// </summary>
        /// <param name="type">点值类型</param>
        public Target(Value.Types type)
        {

            this.type = type;
            dateCreated = DateTime.Now;
        }


        private int id;

        /// <summary>
        /// 点ID
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
       
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
       
        private string descriptor;

        /// <summary>
        /// 描述
        /// </summary>
        public string Descriptor
        {
            get { return descriptor; }
            set { descriptor = value; }
        }
        
        private Value.Types type;

        /// <summary>
        /// 值类型
        /// </summary>
        public Value.Types Type
        {
            get { return type; }
        }

        
        private string device;

        /// <summary>
        /// 设备名
        /// </summary>
        public string Device
        {
            get { return device; }
            set { device = value; }
        }

        
        private string sourceGroup;

        /// <summary>
        /// 源结点组
        /// </summary>
        public string SourceGroup
        {
            get { return sourceGroup; }
            set { sourceGroup = value; }
        }

        
        private string sourceName;

        /// <summary>
        /// 源结点名
        /// </summary>
        public string SourceName
        {
            get { return sourceName; }
            set { sourceName = value; }
        }

        
        private DateTime dateCreated;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }

       
        private DateTime lastTime;

        /// <summary>
        /// 上次数据存档时间
        /// </summary>
        public DateTime LastTime
        {
            get { return lastTime; }
            set { lastTime = value; }
        }


       
        private Status scan;

        /// <summary>
        /// 结点状态
        /// </summary>
        public Status Scan
        {
            get { return scan; }
            set { scan = value; }
        }


        private float hiLimit;
        /// <summary>
        /// 高报警限值
        /// </summary>
        public float HiLimit
        {
            get { return hiLimit; }
            set { hiLimit = value; }
        }

        private float hihiLimit;

        /// <summary>
        /// 高高报警限值
        /// </summary>
        public float HihiLimit
        {
            get { return hihiLimit; }
            set { hihiLimit = value; }
        }
        private float loLimit;

        /// <summary>
        /// 低报警限值
        /// </summary>
        public float LoLimit
        {
            get { return loLimit; }
            set { loLimit = value; }
        }
        private float loloLimit;

        /// <summary>
        /// 低低报警限值
        /// </summary>
        public float LoloLimit
        {
            get { return loloLimit; }
            set { loloLimit = value; }
        }

        
        private bool archiving;
        /// <summary>
        /// 是否进行历史数据保存
        /// </summary>
        public bool Archiving
        {
            get { return archiving; }
            set { archiving = value; }
        }

     
        private bool compressing;
        /// <summary>
        /// 是否进行历史数据压缩
        /// </summary>
        public bool Compressing
        {
            get { return compressing; }
            set { compressing = value; }
        }



        




    }
}
