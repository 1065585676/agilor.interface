
namespace Agilor.Interface.Val
{
    class IntValue:IBase<int>
    {
        private int data;

        public IntValue(int v) { this.data = v; }

        public object Data()
        {
            return data;
        }

        public int CompareTo(object obj)
        {
            IntValue val = obj as IntValue;
            
            if (data > val.data) return 1;
            else if (data == val.data) return 0;
            else return -1;
        }
    }
}
