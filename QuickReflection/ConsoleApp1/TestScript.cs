using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PtrReflection;

public class TestScript 
{
    public int testCount = 100000;
    System.Diagnostics.Stopwatch oTime = new System.Diagnostics.Stopwatch();

    StringBuilder sb = new StringBuilder();
    void DebugLog(object obj)
    {
        Console.WriteLine(obj);
    }

    private unsafe Vector3 GCC(void* a)
    {
        return *((Vector3*)new IntPtr(a));
    }

    private unsafe void Start()
    {
    }


    [StructLayout(LayoutKind.Explicit)]
    public unsafe class UnsafeTool
    {
        public delegate void* ObjectToVoidPtr(object obj);
        public delegate object VoidPtrToObject(void* obj);
        [FieldOffset(0)]
        public ObjectToVoidPtr ObjectToVoid;

        [FieldOffset(0)]
        Func<object, object> func;

        [FieldOffset(8)]
        public VoidPtrToObject VoidToObject;
        [FieldOffset(8)]
        Func<object, object> func2;

        public UnsafeTool()
        {
            func = Out;
            func2 = Out;
        }
        object Out(object o) { return o; }
    }
    
    public unsafe struct ObjReference
    {
        public object obj;
        public ObjReference(object obj) 
        {
            this.obj = obj;
        }
    }

    public unsafe void RunTest2()
    {
        UnsafeTool tool = new UnsafeTool();
        string fieldName = "One";
        var warp = TypeAddrReflectionWrapper.GetWrapper(typeof(MyClass));

        //bytePtr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(ccc, out gcHandle);
        //tool.GetType().TypeHandle.Value;

        var instens = (MyClass)warp.Create();
         //instens = new MyClass();

        TypedReference d = __makeref(instens);

        ObjReference objReference = new ObjReference(instens);
        void** handleVoid2 = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
        void** handleVoid = *(void***)(&d);

        byte** handleByte = (byte**)handleVoid;


        DebugLog("创建 " + instens.GetType());
        instens.str = "S3ED";
        instens.one = 1;
        instens.point = new Vector3(3, -4.5f, 97.4f);

        fieldName = "one";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
            int value = *(int*)(*handleByte + addr.offset);
            DebugLog("取值后 输出 1 : " + value);

            *(int*)(*handleByte + addr.offset) = 18;
            DebugLog("赋值后 输出 18 : " + instens.one);

            object obj = addr.GetValue(handleVoid);
            DebugLog("不指定类型取值后 输出 18 : " + obj);

            addr.SetValue(handleVoid, 444);
            DebugLog("不指定类型赋值后 输出 444 : " + instens.one);
        }

        fieldName = "str";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

            string value = (string)GeneralTool.VoidPtrToObject(*(void**)(*handleByte + addr.offset));
            DebugLog("取值后 输出 S3ED : " + value);

            GeneralTool.SetObject(*handleByte + addr.offset, "Acfv");
            DebugLog("赋值后 输出 Acfv : " + instens.str);

            object obj = addr.GetValue(handleVoid);
            DebugLog("不指定类型取值后 输出 Acfv : " + obj);

            addr.SetValue(handleVoid, "DDcc#2$%@");
            DebugLog("不指定类型赋值后 输出 DDcc#2$%@ : " + instens.str);
        }

        fieldName = "point";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

            Vector3 value = addr.ReadStruct<Vector3>(handleVoid);
            DebugLog("取值后 输出 3, -4.5, 97.4 : " + value);

            var pt = new Vector3(-99.56f, 50.22f, 9f);
            addr.WriteStruct<Vector3>(handleVoid, pt);

            DebugLog("赋值后 输出 -99.56, 50.22, 9 : " + instens.point);

            object obj = addr.GetValue(handleVoid);
            DebugLog("不指定类型取值后 输出 -99.56, 50.22, 9 : " + obj);

            addr.SetValue(handleVoid, new Vector3(0, -9999f, 12.888f));
            DebugLog("不指定类型赋值后 输出 0, -9999 , 12.888 : " + instens.point);
        }


        DebugLog(" ");
        DebugLog("====↓↓↓↓ 属性 ↓↓↓↓===== ");
        DebugLog(" ");

        instens.Str = "S3ED";
        instens.One = 1;
        instens.Point = new Vector3(3, -4.5f, 97.4f);

        fieldName = "One";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

            int k = addr.propertyDelegateItem.getInt32(*handleVoid);
            DebugLog("取值后 输出 1 : " + k);

            addr.propertyDelegateItem.setInt32(*handleVoid, 18);
            DebugLog("赋值后 输出 18 : " + instens.One);

            object obj = addr.GetValue(handleVoid);
            DebugLog("不指定类型取值后 输出 18 : " + obj);

            addr.SetValue(handleVoid, 444);
            DebugLog("不指定类型赋值后 输出 444 : " + instens.One);
        }

        fieldName = "Str";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

            string value = addr.propertyDelegateItem.getString(*handleVoid);
            DebugLog("取值后 输出 S3ED : " + value);

            addr.propertyDelegateItem.setString(*handleVoid, "ADc34gC@#");
            DebugLog("赋值后 输出 ADc34gC@# : " + instens.Str);

            object obj = addr.GetValue(handleVoid);
            DebugLog("不指定类型取值后 输出 ADc34gC@# : " + obj);

            addr.SetValue(handleVoid, "DDcc#2$%@ ");
            DebugLog("不指定类型赋值后 输出 DDcc#2$%@ : " + instens.Str);
        }

        fieldName = "Point";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);

            Vector3 value = (Vector3)addr.propertyDelegateItem.getObject(*handleVoid);
            DebugLog("取值后 输出 3, -4.5, 97.4 : " + value);

            var pt = new Vector3(-99.56f, 50.22f, 9f);
            addr.propertyDelegateItem.setObject(*handleVoid, pt);
            DebugLog("赋值后 输出 -99.56, 50.22, 9 : " + instens.Point);

            object obj = addr.GetValue(handleVoid);
            DebugLog("不指定类型取值后 输出 -99.56, 50.22, 9 : " + obj);

            addr.SetValue(handleVoid, new Vector3(0, -9999f, 12.888f));
            DebugLog("不指定类型赋值后 输出 0, -9999 , 12.888 : " + instens.Point);
        }

       instens.ones = new int[] { 1, 2, 8, 476, 898, 9 };
       instens.strs = new string[] {"ass","#$%^&","*SAHASww&()", "兀驦屮鲵傌" };
       instens.points = new Vector3[] {
           new Vector3(3, -4.5f, 97.4f),
           new Vector3(9999f, -43f, 0.019f),
           new Vector3(55.3f, -0.01f, -130),
       };


       fieldName = "ones";
       fixed (char* fieldNamePtr = fieldName)
       {
           TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
           var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

           Array array = (Array)addr.GetValue(handleVoid);
           ArrayWrapOutData arrayWrapOutData = arrayWrap.GetArrayData(array);

           DebugLog("取值后 输出 1, 2, 8, 476, 898, 9  : " );
           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               int value = *(int*)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i);
               DebugLog(value);
           }

           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               *(int*)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i) = i;
           }
           DebugLog("赋值后 输出  0, 1, 2, 3, 4, 5  : ");
           for (int i = 0; i < array.Length; i++)
           {
               DebugLog(array.GetValue(i));
           }

           object obj = addr.GetValue(handleVoid);
           DebugLog("不指定类型取值后 输出 0, 1, 2, 3, 4, 5 : ");
           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, i);
               DebugLog(value);
           }

           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               arrayWrap.SetValue(arrayWrapOutData.startItemOffcet, i, 100 * i);
           }
           DebugLog("不指定类型赋值后 输出 0, 100, 200, 300, 400, 500 : ");
           for (int i = 0; i < array.Length; i++)
           {
               DebugLog(array.GetValue(i));
           }
       }

       fieldName = "strs";
       fixed (char* fieldNamePtr = fieldName)
       {
           TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
           var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

           Array array = (Array)addr.GetValue(handleVoid);
           ArrayWrapOutData arrayWrapOutData = arrayWrap.GetArrayData(array);

           DebugLog("取值后 输出 ass,#$%^&,*SAHASww&(), 兀驦屮鲵傌 : ");
           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               string value = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i));
               DebugLog(value);
           }

           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               GeneralTool.SetObject(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i, "Ac4……*" + i);
           }
           DebugLog("赋值后 输出  Ac4……*0, Ac4……*1, Ac4……*2, Ac4……*3  : ");
           for (int i = 0; i < array.Length; i++)
           {
               DebugLog(array.GetValue(i));
           }

           object obj = addr.GetValue(handleVoid);
           DebugLog("不指定类型取值后 输出 Ac4……*0, Ac4……*1, Ac4……*2, Ac4……*3  : ");
           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, i);
               DebugLog(value);
           }

           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               arrayWrap.SetValue(arrayWrapOutData.startItemOffcet, i, "Fc%^" + i * 100);
           }
           DebugLog("不指定类型赋值后 输出 Fc%^0, Fc%^100, Fc%^200, Fc%^300 : ");
           for (int i = 0; i < array.Length; i++)
           {
               DebugLog(array.GetValue(i));
           }
       }

       fieldName = "points";
       fixed (char* fieldNamePtr = fieldName)
       {
           TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
           var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

           Array array = (Array)addr.GetValue(handleVoid);
           ArrayWrapOutData arrayWrapOutData = arrayWrap.GetArrayData(array);

           DebugLog("取值后 输出 (3, -4.5, 97.4) (9999, -43, 0.019) (55.3, -0.01, -130) : ");
           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               Vector3 value = *(Vector3*)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i);
               DebugLog(value);
           }

           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               var v = new Vector3(i, i, i);
               GeneralTool.MemCpy(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i, GeneralTool.AsPointer(ref v), arrayWrap.elementTypeSize);
           }

           DebugLog("赋值后 输出  (0,0,0) , (1,1,1) , (2,2,2)  : ");
           for (int i = 0; i < array.Length; i++)
           {
               DebugLog(array.GetValue(i));
           }


           object obj = addr.GetValue(handleVoid);
           DebugLog("不指定类型取值后 输出  (0,0,0) , (1,1,1) , (2,2,2)  : ");
           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, i);
               DebugLog(value);
           }

           for (int i = 0; i < arrayWrapOutData.length; i++)
           {
               arrayWrap.SetValue(arrayWrapOutData.startItemOffcet, i, new Vector3(i * 100, i * 10, i * 1000));
           }
           DebugLog("不指定类型赋值后 输出  (0,0,0) , (100,10,1000) , (200,20,2000) : ");
           for (int i = 0; i < array.Length; i++)
           {
               DebugLog(array.GetValue(i));
           }

       }




        /*



       //text.text = sb.ToString();

       //UnsafeUtility.ReleaseGCObject(gcHandle);

       //*/
        GC.Collect();
        Console.ReadKey();
        return;
    }

    string[] v1s;
    int[] v2s;
    Vector3[] v3s;

    string v1;
    int v2;
    Vector3 v3;
    MyClass a;
    MyClass myClass;
    public unsafe void RunTest()
    {
        ///*
        var warp = TypeAddrReflectionWrapper.GetWrapper(typeof(MyClass));

        string fieldName = "str";
        int nameSize = fieldName.Length;

        ulong gcHandle;
        myClass = (MyClass)warp.Create();
        var instens = (MyClass)warp.Create();

        ObjReference objReference = new ObjReference(instens);
        void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
        byte** handleByte = (byte**)handleVoid;


        string str = "hello world";
        Vector3 point = new Vector3(99.2f, -98.2f, 3.2f);
        DebugLog("");
        DebugLog("循环" + testCount + "次");
        DebugLog("====================================");
        DebugLog("");

        var fieldName_str = nameof(MyClass.str);
        var fieldName_one = nameof(MyClass.one);
        var fieldName_point = nameof(MyClass.point);

        var fieldName_strLength = fieldName_str.Length;
        var fieldName_oneLength = fieldName_one.Length;
        var fieldName_pointLength = fieldName_point.Length;

        var fieldName_Str = nameof(MyClass.Str);
        var fieldName_One = nameof(MyClass.One);
        var fieldName_Point = nameof(MyClass.Point);

        var fieldName_StrLength = fieldName_Str.Length;
        var fieldName_OneLength = fieldName_One.Length;
        var fieldName_PointLength = fieldName_Point.Length;


        fixed (char* _str = fieldName_str)
        fixed (char* _one = fieldName_one)
        fixed (char* _point = fieldName_point)
        fixed (char* _Str = fieldName_Str)
        fixed (char* _One = fieldName_One)
        fixed (char* _Point = fieldName_Point)
        {
            //fieldName = "one";
            //nameSize = fieldName.Length;

            var addr1 = warp.Find(_str, fieldName_strLength);
            var addr2 = warp.Find(_one, fieldName_oneLength);
            var addr3 = warp.Find(_point, fieldName_pointLength);


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                typeof(MyClass).GetField(nameof(MyClass.str)).SetValue(myClass, str);
                typeof(MyClass).GetField(nameof(MyClass.one)).SetValue(myClass, 18);
                typeof(MyClass).GetField(nameof(MyClass.point)).SetValue(myClass, point);
            }
            oTime.Stop();
            DebugLog("FieldInfo SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                warp.nameOfField[nameof(MyClass.str)].SetValue(handleVoid, str);
                warp.nameOfField[nameof(MyClass.one)].SetValue(handleVoid, 18);
                warp.nameOfField[nameof(MyClass.point)].SetValue(handleVoid, point);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 使用object类型 string查询 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                warp.Find(_str, fieldName_strLength).SetValue(handleVoid, str);
                warp.Find(_one, fieldName_oneLength).SetValue(handleVoid, 18);
                warp.Find(_point, fieldName_pointLength).SetValue(handleVoid, point);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 使用object类型 char*查询 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                GeneralTool.SetObject(*handleByte + warp.Find(_str, fieldName_strLength).offset, str);
                *(int*)(*handleByte + warp.Find(_one, fieldName_oneLength).offset) = 18;
                var addr = warp.Find(_point, fieldName_pointLength);
                GeneralTool.MemCpy(*handleByte + addr.offset, GeneralTool.AsPointer(ref point), addr.stackSize);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                GeneralTool.SetObject(*handleByte + addr1.offset, str);
                *(int*)(handleVoid + addr2.offset) = 18;
                GeneralTool.MemCpy(handleVoid + addr3.offset, GeneralTool.AsPointer(ref point), addr3.stackSize);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 忽略字符串查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                myClass.one = 18;
                myClass.str = str;
                myClass.point = point;
            }
            oTime.Stop();
            DebugLog("原生 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            DebugLog("");
            DebugLog("====================================");
            DebugLog("");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)typeof(MyClass).GetField(nameof(MyClass.str)).GetValue(myClass);
                v2 = (int)typeof(MyClass).GetField(nameof(MyClass.one)).GetValue(myClass);
                v3 = (Vector3)typeof(MyClass).GetField(nameof(MyClass.point)).GetValue(myClass);
            }
            oTime.Stop();
            DebugLog("FieldInfo GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)warp.nameOfField[nameof(MyClass.str)].GetValue(handleVoid);
                v2 = (int)warp.nameOfField[nameof(MyClass.one)].GetValue(handleVoid);
                v3 = (Vector3)warp.nameOfField[nameof(MyClass.point)].GetValue(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 使用object类型 string查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)warp.Find(_str, fieldName_strLength).GetValue(handleVoid);
                v2 = (int)warp.Find(_one, fieldName_oneLength).GetValue(handleVoid);
                v3 = (Vector3)warp.Find(_point, fieldName_pointLength).GetValue(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 使用object类型 char*查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)GeneralTool.VoidPtrToObject(*(void**)(*handleByte + warp.Find(_str, fieldName_strLength).offset));
                v2 = *(int*)(handleVoid + warp.Find(_one, fieldName_oneLength).offset);
                v3 = warp.Find(_point, fieldName_pointLength).ReadStruct<Vector3>(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)GeneralTool.VoidPtrToObject(*(void**)(*handleByte + addr1.offset));
                v2 = *(int*)(handleVoid + addr2.offset);
                v3 = addr3.ReadStruct<Vector3>(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 忽略字符串查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = myClass.str;
                v2 = myClass.one;
                v3 = myClass.point;
            }
            oTime.Stop();
            DebugLog("原生 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            DebugLog("");
            DebugLog("=========↓↓↓Property↓↓↓=============");
            DebugLog("");

            addr1 =  warp.Find(_Str, fieldName_StrLength);
            addr2 = warp.Find(_One, fieldName_OneLength);
            addr3 = warp.Find(_Point, fieldName_PointLength);
            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                typeof(MyClass).GetProperty(nameof(MyClass.Str)).SetValue(myClass, str);
                typeof(MyClass).GetProperty(nameof(MyClass.One)).SetValue(myClass, 18);
                typeof(MyClass).GetProperty(nameof(MyClass.Point)).SetValue(myClass, point);
            }
            oTime.Stop();
            DebugLog("PropertyInfo SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                warp.Find(_Str, fieldName_StrLength).SetValue(handleVoid, str);
                warp.Find(_One, fieldName_OneLength).SetValue(handleVoid, 18);
                warp.Find(_Point, fieldName_PointLength).SetValue(handleVoid, point);
            }
            
            oTime.Stop();
            DebugLog("指针方法 SetValue 使用object类型：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                 warp.Find(_Str, fieldName_StrLength).propertyDelegateItem.setString(*handleVoid, str);
                warp.Find(_One, fieldName_OneLength).propertyDelegateItem.setInt32(*handleVoid, 18);
                warp.Find(_Point, fieldName_PointLength).propertyDelegateItem.setObject(*handleVoid, point);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                addr1.propertyDelegateItem.setString(*handleVoid, str);
                addr2.propertyDelegateItem.setInt32(*handleVoid, 18);
                addr3.propertyDelegateItem.setObject(*handleVoid, point);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 忽略字符串查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = myClass.Str;
                v2 = myClass.One;
                v3 = myClass.Point;
            }
            oTime.Stop();
            DebugLog("原生 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            DebugLog("");
            DebugLog("====================================");
            DebugLog("");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)typeof(MyClass).GetProperty(nameof(MyClass.Str)).GetValue(myClass);
                v2 = (int)typeof(MyClass).GetProperty(nameof(MyClass.One)).GetValue(myClass);
                v3 = (Vector3)typeof(MyClass).GetProperty(nameof(MyClass.Point)).GetValue(myClass);
            }
            oTime.Stop();
            DebugLog("PropertyInfo GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string) warp.Find(_Str, fieldName_StrLength).GetValue(handleVoid);
                v2 = (int)warp.Find(_One, fieldName_OneLength).GetValue(handleVoid);
                v3 = (Vector3)warp.Find(_Point, fieldName_PointLength).GetValue(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 使用object类型 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 =  warp.Find(_Str, fieldName_StrLength).propertyDelegateItem.getString(handleVoid);
                v2 = warp.Find(_One, fieldName_OneLength).propertyDelegateItem.getInt32(handleVoid);
                v3 = (Vector3)warp.Find(_Point, fieldName_PointLength).propertyDelegateItem.getObject(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = addr1.propertyDelegateItem.getString(handleVoid);
                v2 = addr2.propertyDelegateItem.getInt32(handleVoid);
                v3 = (Vector3)addr3.propertyDelegateItem.getObject(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 忽略字符串查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                myClass.Str = str;
                myClass.One = 18;
                myClass.Point = point;
            }
            oTime.Stop();
            DebugLog("原生 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            DebugLog("");
            DebugLog("=========↓↓↓Array↓↓↓=========");
            DebugLog("");
            v1s = new string[] { "Ac", "S@D", "CS@", "CS2ss", "CXaa" };
            v2s = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            v3s = new Vector3[] { new Vector3(1, 2, 3), new Vector3(11, -2, 41.3f), new Vector3(19.999f, 9, 0), new Vector3(123.454f, 647.4f, 11) };
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                Array arrayV1 = v1s;
                for (int j = 0; j < arrayV1.Length; j++)
                {
                    v1 = (string)arrayV1.GetValue(j);
                }
                Array arrayV2 = v2s;
                for (int j = 0; j < arrayV2.Length; j++)
                {
                    v2 = (int)arrayV2.GetValue(j);
                }
                Array arrayV3 = v3s;
                for (int j = 0; j < arrayV3.Length; j++)
                {
                    v3 = (Vector3)arrayV3.GetValue(j);
                }
            }

            oTime.Stop();
            DebugLog("Array GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapOutData data1 = arrayWrapV2.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    v1 = (string)arrayWrapV1.GetValue(data1.startItemOffcet, j);
                }
                ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    v2 = (int)arrayWrapV2.GetValue(data2.startItemOffcet, j);
                }
                ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    v3 = (Vector3)arrayWrapV3.GetValue(data3.startItemOffcet, j);
                }
            }
            oTime.Stop();
            DebugLog("ArrayWrapManager GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * j));
                }
                ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    v2 = *(int*)(data2.startItemOffcet + arrayWrapV2.elementTypeSize * j);
                }
                ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    v3 = *(Vector3*)(data3.startItemOffcet + arrayWrapV3.elementTypeSize * j);
                }
            }
            oTime.Stop();
            DebugLog("ArrayWrapManager GetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            {
                ArrayWrapOutData data1 = arrayWrapV2.GetArrayData(v1s);
                ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2s);
                ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3s);
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    for (int j = 0; j < data1.length; j++)
                    {
                        v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * j));
                    }
                    for (int j = 0; j < data2.length; j++)
                    {
                        v2 = *(int*)(data2.startItemOffcet + arrayWrapV2.elementTypeSize * j);
                    }
                    for (int j = 0; j < data3.length; j++)
                    {
                        v3 = *(Vector3*)(data3.startItemOffcet + arrayWrapV3.elementTypeSize * j);
                    }
                }
            }
            oTime.Stop();
            DebugLog("ArrayWrapManager GetValue 忽略Data查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                for (int j = 0; j < v1s.Length; j++)
                {
                    v1 = v1s[j];
                }
                for (int j = 0; j < v2s.Length; j++)
                {
                    v2 = v2s[j];
                }
                for (int j = 0; j < v3s.Length; j++)
                {
                    v3 = v3s[j];
                }
            }
            oTime.Stop();
            DebugLog("原生 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                Array arrayV1 = v1s;
                for (int j = 0; j < arrayV1.Length; j++)
                {
                    arrayV1.SetValue(v1, j);
                }
                Array arrayV2 = v2s;
                for (int j = 0; j < arrayV2.Length; j++)
                {
                    arrayV2.SetValue(v2, j);
                }
                Array arrayV3 = v3s;
                for (int j = 0; j < arrayV3.Length; j++)
                {
                    arrayV3.SetValue(v3, j);
                }
            }
            oTime.Stop();
            DebugLog("Array SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    arrayWrapV1.SetValue(data1.startItemOffcet, j, v1);
                }
                ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    arrayWrapV2.SetValue(data2.startItemOffcet, j, v2);
                }
                ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    arrayWrapV3.SetValue(data3.startItemOffcet, j, v3);
                }
            }
            oTime.Stop();
            DebugLog("ArrayWrapManager SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    GeneralTool.SetObject(data1.startItemOffcet + arrayWrapV1.elementTypeSize * j, v1);
                }

                ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    *(int*)(data2.startItemOffcet + arrayWrapV2.elementTypeSize * j) = v2;
                }

                ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    var v = new Vector3(j, j, j);
                    GeneralTool.MemCpy(data3.startItemOffcet + arrayWrapV3.elementTypeSize * j, GeneralTool.AsPointer(ref v), arrayWrapV3.elementTypeSize);
                }
            }
            oTime.Stop();
            DebugLog("ArrayWrapManager SetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            {
                ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1s);
                ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2s);
                ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3s);
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    for (int j = 0; j < data1.length; j++)
                    {
                        GeneralTool.SetObject(data1.startItemOffcet + arrayWrapV1.elementTypeSize * j, v1);
                    }

                    for (int j = 0; j < data2.length; j++)
                    {
                        *(int*)(data2.startItemOffcet + arrayWrapV2.elementTypeSize * j) = v2;
                    }

                    for (int j = 0; j < data3.length; j++)
                    {
                        var v = new Vector3(i, i, i);
                        GeneralTool.MemCpy(data3.startItemOffcet + arrayWrapV3.elementTypeSize * j, GeneralTool.AsPointer(ref v), arrayWrapV3.elementTypeSize);
                    }
                }
            }
            oTime.Stop();
            DebugLog("ArrayWrapManager SetValue 忽略Data查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                for (int j = 0; j < v1s.Length; j++)
                {
                    v1s[j] = v1;
                }
                for (int j = 0; j < v2s.Length; j++)
                {
                    v2s[j] = v2;
                }
                for (int j = 0; j < v3s.Length; j++)
                {
                    v3s[j] = v3;
                }
            }
            oTime.Stop();
            DebugLog("原生 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");




            DebugLog("");
            DebugLog("=============↓↓↓CreateInstance↓↓↓===============");
            DebugLog("");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                a = (MyClass)System.Activator.CreateInstance(typeof(MyClass));
            }
            oTime.Stop();
            DebugLog("Activator CreateInstance：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                a = (MyClass)warp.Create();
            }
            oTime.Stop();
            DebugLog("指针方法 Create：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                a = new MyClass();
            }
            oTime.Stop();
            DebugLog(" new ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            GC.Collect();
        }
        //*/
    }



    void Update()
    {

    }
}
