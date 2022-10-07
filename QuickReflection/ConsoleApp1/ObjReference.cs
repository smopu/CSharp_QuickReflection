using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtrReflection
{
    public unsafe struct ObjReference
    {
        public object obj;
        public ObjReference(object obj)
        {
            this.obj = obj;
        }
    }
}
