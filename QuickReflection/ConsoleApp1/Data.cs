using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace ConsoleApp1
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

    public struct JLQ
    {
        public long a;
        public long x;
    }
    public struct JLQ2
    {
        public long a;
        public long x;
        public long y;
        public long z;
        public long z1;
        public long z2;
        public long z3;
        public long z4;
        public long z5;
        public long z6;
        public long z7;
    }

    public class DataF
    {
        public JLQ a { get; set; }
    }

    public class DataF2
    {
        public JLQ FV (Func<DataF, JLQ> func , DataF f)
        {
            return func(f);
        }
    }



    [StructLayout(LayoutKind.Explicit)]
    public unsafe class DDD
    {
        [FieldOffset(0)]
        public Delegate delegate1;

        [FieldOffset(0)]
        public Func<JLQ2> delegate2;
    }


    class Program
    {
        public static JLQ Get()
        {
            JLQ jLQ = new JLQ();
            jLQ.a = 111;
            jLQ.x = 1344;
            return jLQ;
        }
        static unsafe void Main(string[] args)
        {
            TestScript testScript = new TestScript();
            testScript.RunTest();

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

            DDD ddd = new DDD();
            ddd.delegate1 = (Func<JLQ>)Get;
            var v = ddd.delegate2();

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

            Console.WriteLine("IL2CPP不能用关键字__makeref，可以用下面方法取【对象句柄】：");
            objs[0] = data;
            void** handle2 = (void**)tool.objectToVoidPtr(objs[0]);
            Console.WriteLine(" 句柄地址：" + ((long)handle));

            Console.WriteLine("IL2CPP不能用关键字__makeref，可以用下面方法取【类型句柄】：");
             d = __makeref(data);
            Console.WriteLine(*((IntPtr*)(&d) + 1));
            Console.WriteLine(data.GetType().TypeHandle.Value);

            Console.Read();
        }
    }
}
