using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

// public delegate*<object, void*> objectToVoidPtr;
namespace PtrReflection
{
    unsafe class Program
    {
        static unsafe void Main(string[] args)
        {
            TestScript testScript = new TestScript();
            testScript.RunTest();
            Console.ReadLine();
            return;
        }

    }

}

