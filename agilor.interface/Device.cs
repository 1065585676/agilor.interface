
namespace Agilor.Interface
{
    /// <summary>
    /// 设备数据类
    /// </summary>
    public class Device
    {
        string name;

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        bool isOnline;

        /// <summary>
        /// 是否有数采程序连接
        /// </summary>
        public bool IsOnline
        {
            get { return isOnline; }
            set { isOnline = value; }
        }


    }
}
