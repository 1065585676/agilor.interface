using Agilor.Interface.Val;

namespace Agilor.Interface
{
    /// <summary>
    /// watch机制回调接口
    /// </summary>
    public interface IWatcher
    {
        /// <summary>
        /// 接收数据事件
        /// </summary>
        /// <param name="val">订阅点的实时值</param>
        void OnReceive(Value val);
        /// <summary>
        /// 高报警事件
        /// </summary>
        /// <param name="val">订阅点的实时值</param>
        void OnHiLimit(Value val);
        /// <summary>
        /// 高高报警事件
        /// </summary>
        /// <param name="val">订阅点的实时值</param>
        void OnHiHiLimit(Value val);
        /// <summary>
        /// 低报警事件
        /// </summary>
        /// <param name="val">订阅点的实时值</param>
        void OnLoLimit(Value val);
        /// <summary>
        /// 低低报警事件
        /// </summary>
        /// <param name="val">订阅点的实时值</param>
        void OnLoLoLimit(Value val);
    }
}
