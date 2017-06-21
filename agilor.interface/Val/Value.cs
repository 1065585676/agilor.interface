using System;

namespace Agilor.Interface.Val
{
    /// <summary>
    /// 点值
    /// </summary>
    public class Value : IComparable 
    {
        /// <summary>
        /// 点值类型
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// 无类型
            /// </summary>
            NONE=0,
            /// <summary>
            /// 长整形
            /// </summary>
            LONG = (int)'L',
            /// <summary>
            /// 浮点型
            /// </summary>
            FLOAT = (int)'R',
            /// <summary>
            /// 字符串类型
            /// </summary>
            STRING = (int)'S',
            /// <summary>
            /// 布尔型
            /// </summary>
            BOOL = (int)'B'
        }

        
        private DateTime time;


        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        private IBase val;

        /// <summary>
        /// 点值，必须是float bool,string 或 int
        /// </summary>
        public object Val
        {
            get { return this.val.Data(); }
            set
            {

                if (value is float)
                {
                    this.type = Types.FLOAT;
                    this.val = new FloatValue((float)value);
                }
                else if (value is bool)
                {
                    this.type = Types.BOOL;
                    this.val = new BoolValue((bool)value);
                }
                else if (value is int)
                {
                    this.type = Types.LONG;
                    this.val = new IntValue((int)value);
                }
                else if (value == null)
                {
                    this.type = Types.NONE;
                    this.val = null;
                }
                else if (value is string)
                {
                    this.type = Types.STRING;
                    this.val = new StringValue((string)value);
                }
                else
                    throw new Exception("the value type must be float ,bool, long or string");
            }
        }


        private Types type;

        /// <summary>
        /// 点值类型
        /// </summary>
        public Types Type
        {
            get { return type; }
        }




      
        private string name;

        /// <summary>
        /// 要写入的源结点名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        private int state;

        /// <summary>
        /// 点状态
        /// </summary>
        public int State
        {
            get { return state; }
            set { state = value; }
        }

        private Value() { }

        /// <summary>
        ///  初始化一个Value类型
        /// </summary>
        /// <param name="source">源节点名</param>
        /// <param name="val">点值，BOOL，Int,Float类型</param>
        /// <param name="time">时间</param>
        public Value(string source, object val, DateTime time)
        {
            this.name = source;
            this.Val = val;
            this.time = time;
        }

        /// <summary>
        /// 初始化一个Value类型
        /// </summary>
        /// <param name="source">源节点名</param>
        /// <param name="val">点值，BOOL，Int,Float类型</param>
        public Value(string source,object val)
        {
            this.name = source;
            this.Val = val;
            this.time = DateTime.Now;
        }

        /// <summary>
        /// 将当前实例与同一类型的另一个对象进行比较，并返回一个整数，该整数指示当前实例在排序顺序中的位置是位于另一个对象之前、之后还是与其位置相同
        /// </summary>
        /// <param name="obj">与此实例进行比较的对象</param>
        /// <returns>一个值，指示要比较的对象的相对顺序。返回值的含义如下：值含义小于零此实例小于 obj。零此实例等于 obj。大于零此实例大于 obj。</returns>
        public int CompareTo(object obj)
        {
            Value data = obj as Value;
            
            if (val == null|| data.Val==null) throw new Exception("the value is null");
            return val.CompareTo(data.val);
        }

        /// <summary>
        /// 和float进行隐式转换
        /// </summary>
        /// <param name="v">Value值</param>
        /// <returns>float值</returns>
        public static implicit operator float(Value v) {
            return (float)v.Val;
        }

        /// <summary>
        /// 和int型进行隐式转换
        /// </summary>
        /// <param name="v">Value值</param>
        /// <returns>int值</returns>
        public static implicit operator int(Value v)
        {
            return (int)v.Val;
        }
        /// <summary>
        /// 和bool型进行隐式转换
        /// </summary>
        /// <param name="v">Value值</param>
        /// <returns>bool值</returns>
        public static implicit operator bool(Value v)
        {
            return (bool)v.Val;
        }

        /// <summary>
        /// 和string型进行隐式转换
        /// </summary>
        /// <param name="v">Value值</param>
        /// <returns>string值</returns>
        public static implicit operator string(Value v)
        {
            return (string)v.Val;
        }
    }
}
