
namespace Agilor.Interface.Val
{
    class FloatValue:IBase<float>
    {
        private float data;

        public FloatValue(float v) { this.data = v; }

        public object Data()
        {
            return data;
        }

        public int CompareTo(object obj)
        {
            FloatValue val = obj as FloatValue;
            if (data > val.data) return 1;
            else if (data == val.data) return 0;
            else return -1;
        }
    }
}
