using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace UnitTest
{
    public unsafe class UnitTest1
    {
        [StructLayout(LayoutKind.Explicit)]
        public unsafe class UnsafeTool
        {
            public delegate void* ObjectToVoidPtr(object obj);
            public delegate object VoidPtrToObject(void* obj);

            [FieldOffset(0)]
            public ObjectToVoidPtr objectToVoidPtr;
            [FieldOffset(0)]
            Func<object, object> func;

            [FieldOffset(8)]
            public VoidPtrToObject voidPtrToObject;
            [FieldOffset(8)]
            Func<object, object> func2;
            public UnsafeTool()
            {
                func = Out;
                func2 = Out;
            }
            object Out(object o) { return o; }
        }

        [StructLayout(LayoutKind.Explicit)]
        class Data
        {
            [FieldOffset(0)]
            public long a = 0;
            [FieldOffset(67789)]
            long a2 = 0;
        }

        public unsafe struct ObjReference
        {
            public object obj;
            public ObjReference(object obj)
            {
                this.obj = obj;
            }
        }

        [Test]
        public void Test1()
        {
            //GCHandle.Alloc(testScript, GCHandleType.Pinned);
            //工具类
            UnsafeTool tool = new UnsafeTool();

            object[] objs = new object[1];
            {
                //申请一大堆内存 用于GC
                List<Data> programs = new List<Data>();
                for (int i = 0; i < 10000; i++)
                {
                    programs.Add(new Data());
                }
                programs.Clear();
                programs = null;
            }

            //我们把取地址的对象 防在申请内存的中间
            Data data = new Data();
            data.a = 100;

            TypedReference d = __makeref(data);
            IntPtr p = *(IntPtr*)*(IntPtr*)(&d);
            Console.WriteLine("__makeref 取对象地址：" + p);

            //句柄
            void** handle = *(void***)(&d);

            Console.WriteLine("objectToVoidPtr 取对象地址：" + (long)tool.objectToVoidPtr(data));
            Console.WriteLine("上面两种方法取到的地址应该是一样的\n");


            Console.WriteLine("句柄地址：" + ((long)handle));
            Console.WriteLine("句柄 取对象的取对象地址：" + ((long)*handle));
            Console.WriteLine("句柄取到对象的地址应该是和两种方法取到的一样的\n");
            Console.WriteLine("句柄 取对象的值：" + ((Data)tool.voidPtrToObject(*handle)).a);

            {
                //申请一大堆内存 用于GC，如果后面代码取到data的地址都一样，把10000加大
                List<Data> programs = new List<Data>();
                for (int i = 0; i < 10000; i++)
                {
                    programs.Add(new Data());
                }
                programs.Clear();
                programs = null;
            }
            System.GC.Collect();

            Console.WriteLine("\n经过GC以后，可以看到对象的地址是改变了的\n");

            p = *(IntPtr*)*(IntPtr*)(&d);
            Console.WriteLine("__makeref 取对象地址：" + p);
            Console.WriteLine("objectToVoidPtr 取对象地址：" + (long)tool.objectToVoidPtr(data));
            Console.WriteLine("上面两种方法取到的地址应该是一样的，但和第一次取的不一样\n");

            Console.WriteLine("句柄地址：" + ((long)handle));
            Console.WriteLine("句柄 取对象的取对象地址：" + ((long)*handle));
            Console.WriteLine("句柄取到对象的地址应该是和两种方法取到的一样的\n");
            Console.WriteLine("句柄 取对象的值：" + ((Data)tool.voidPtrToObject(*handle)).a);

            Console.WriteLine("\n句柄的值不随GC变动而变化\n");

            Console.WriteLine("不用关键字__makeref，可以用下面方法取【对象句柄】：");
            ObjReference objReference = new ObjReference();
            objReference.obj = data;

            void** handleVoid2 = (void**)Unsafe.AsPointer(ref objReference);

            void** handle2 = (void**)tool.objectToVoidPtr(objs[0]);
            Console.WriteLine(" 句柄地址：" + ((long)handle));
            Assert.Pass();
        }
    }
}