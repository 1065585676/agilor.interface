using System;

namespace Agilor.Interface.Val
{
    class BoolValue:IBase<bool>
    {
        private bool data;

        public BoolValue(bool v) { this.data = v; }

        public object Data()
        {
            return data;
        }

        public int CompareTo(object obj)
        {
            throw new Exception("the bool type can't campare with others");
        }
    }
}
