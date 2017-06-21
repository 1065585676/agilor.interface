using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agilor.Interface.Val
{
    class StringValue:IBase<String>
    {
        private string data;

        public StringValue(string v) { this.data = v; }

        public object Data()
        {
            return data;
        }

        public int CompareTo(object obj)
        {
            StringValue val = obj as StringValue;
            return data.CompareTo(val.data);
        }
    }
}
