using System;

namespace Agilor.Interface.Val
{
    /// <summary>
    /// Agilor数据类型接口
    /// </summary>
    public interface IBase : IComparable
    {
        /// <summary>
        /// 获取原始数据
        /// </summary>
        /// <returns>Object基础值</returns>
        object Data();
    }

    /// <summary>
    /// Agilor数据类型泛型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBase<T> : IBase
    {
    }
}
