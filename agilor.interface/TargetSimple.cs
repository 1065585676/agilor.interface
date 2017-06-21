
namespace Agilor.Interface
{
    /// <summary>
    /// 点数据类（仅包含少量属性）
    /// </summary>
    public class TargetSimple
    {
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
        /// 点名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
