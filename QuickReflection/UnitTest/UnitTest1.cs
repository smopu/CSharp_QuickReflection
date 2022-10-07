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
            //������
            UnsafeTool tool = new UnsafeTool();

            object[] objs = new object[1];
            {
                //����һ����ڴ� ����GC
                List<Data> programs = new List<Data>();
                for (int i = 0; i < 10000; i++)
                {
                    programs.Add(new Data());
                }
                programs.Clear();
                programs = null;
            }

            //���ǰ�ȡ��ַ�Ķ��� ���������ڴ���м�
            Data data = new Data();
            data.a = 100;

            TypedReference d = __makeref(data);
            IntPtr p = *(IntPtr*)*(IntPtr*)(&d);
            Console.WriteLine("__makeref ȡ�����ַ��" + p);

            //���
            void** handle = *(void***)(&d);

            Console.WriteLine("objectToVoidPtr ȡ�����ַ��" + (long)tool.objectToVoidPtr(data));
            Console.WriteLine("�������ַ���ȡ���ĵ�ַӦ����һ����\n");


            Console.WriteLine("�����ַ��" + ((long)handle));
            Console.WriteLine("��� ȡ�����ȡ�����ַ��" + ((long)*handle));
            Console.WriteLine("���ȡ������ĵ�ַӦ���Ǻ����ַ���ȡ����һ����\n");
            Console.WriteLine("��� ȡ�����ֵ��" + ((Data)tool.voidPtrToObject(*handle)).a);

            {
                //����һ����ڴ� ����GC������������ȡ��data�ĵ�ַ��һ������10000�Ӵ�
                List<Data> programs = new List<Data>();
                for (int i = 0; i < 10000; i++)
                {
                    programs.Add(new Data());
                }
                programs.Clear();
                programs = null;
            }
            System.GC.Collect();

            Console.WriteLine("\n����GC�Ժ󣬿��Կ�������ĵ�ַ�Ǹı��˵�\n");

            p = *(IntPtr*)*(IntPtr*)(&d);
            Console.WriteLine("__makeref ȡ�����ַ��" + p);
            Console.WriteLine("objectToVoidPtr ȡ�����ַ��" + (long)tool.objectToVoidPtr(data));
            Console.WriteLine("�������ַ���ȡ���ĵ�ַӦ����һ���ģ����͵�һ��ȡ�Ĳ�һ��\n");

            Console.WriteLine("�����ַ��" + ((long)handle));
            Console.WriteLine("��� ȡ�����ȡ�����ַ��" + ((long)*handle));
            Console.WriteLine("���ȡ������ĵ�ַӦ���Ǻ����ַ���ȡ����һ����\n");
            Console.WriteLine("��� ȡ�����ֵ��" + ((Data)tool.voidPtrToObject(*handle)).a);

            Console.WriteLine("\n�����ֵ����GC�䶯���仯\n");

            Console.WriteLine("���ùؼ���__makeref�����������淽��ȡ������������");
            ObjReference objReference = new ObjReference();
            objReference.obj = data;

            void** handleVoid2 = (void**)Unsafe.AsPointer(ref objReference);

            void** handle2 = (void**)tool.objectToVoidPtr(objs[0]);
            Console.WriteLine(" �����ַ��" + ((long)handle));
            Assert.Pass();
        }
    }
}