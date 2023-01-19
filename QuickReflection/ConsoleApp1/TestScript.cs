
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

    void DebugLog(object obj)
    {
        Console.WriteLine(Convert.ToString(obj));
        GC.Collect();
    }


    public unsafe void RunTest2()
    {
        StringBuilder sb = new StringBuilder();
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
        instens.strs = new string[] { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" };
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
            ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

            d = __makeref(array);
            byte** pp = *(byte***)(&d);

            DebugLog("取值后 输出 1, 2, 8, 476, 898, 9  : ");
            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                int value = *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i);
                DebugLog(value);
            }

            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i) = i;
            }
            DebugLog("赋值后 输出  0, 1, 2, 3, 4, 5  : ");
            for (int i = 0; i < array.Length; i++)
            {
                DebugLog(array.GetValue(i));
            }

            DebugLog("不指定类型取值后 输出 0, 1, 2, 3, 4, 5 : ");
            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, i);
                DebugLog(value);
            }

            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, i, 100 * i);
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
            ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

            d = __makeref(array);
            byte** pp = *(byte***)(&d);

            DebugLog("取值后 输出 ass,#$%^&,*SAHASww&(), 兀驦屮鲵傌 : ");
            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                string value = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i));
                DebugLog(value);
            }

            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                GeneralTool.SetObject(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i, "Ac4……*" + i);
            }
            DebugLog("赋值后 输出  Ac4……*0, Ac4……*1, Ac4……*2, Ac4……*3  : ");
            for (int i = 0; i < array.Length; i++)
            {
                DebugLog(array.GetValue(i));
            }

            DebugLog("不指定类型取值后 输出 Ac4……*0, Ac4……*1, Ac4……*2, Ac4……*3  : ");
            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, i);
                DebugLog(value);
            }

            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, i, "Fc%^" + i * 100);
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
            ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

            d = __makeref(array);
            byte** pp = *(byte***)(&d);

            DebugLog("取值后 输出 (3, -4.5, 97.4) (9999, -43, 0.019) (55.3, -0.01, -130) : ");
            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                Vector3 value = *(Vector3*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i);
                DebugLog(value);
            }

            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                var v = new Vector3(i, i, i);
                GeneralTool.MemCpy(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i, GeneralTool.AsPointer(ref v), arrayWrap.elementTypeSize);
            }

            DebugLog("赋值后 输出  (0,0,0) , (1,1,1) , (2,2,2)  : ");
            for (int i = 0; i < array.Length; i++)
            {
                DebugLog(array.GetValue(i));
            }


            DebugLog("不指定类型取值后 输出  (0,0,0) , (1,1,1) , (2,2,2)  : ");
            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, i);
                DebugLog(value);
            }

            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, i, new Vector3(i * 100, i * 10, i * 1000));
            }
            DebugLog("不指定类型赋值后 输出  (0,0,0) , (100,10,1000) , (200,20,2000) : ");
            for (int i = 0; i < array.Length; i++)
            {
                DebugLog(array.GetValue(i));
            }
        }

        instens.oness = new int[,,]
        {
            { { 1, 2 },{  252, 1331 },{ 55, 66 } },
            { { 11, 898},{ 13, -19 },{ -1, -999999 } },
            { { 4576, 8198 },{ 0, 0 },{ 4176, 8958 } }
        };
        instens.strss = new string[,] {
            { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" },
            { "adsadad", "⑤驦屮尼傌", "fun(a*(b+c))", "FSD手动阀手动阀" },
        };
        instens.pointss = new Vector3[,] {
            {
               new Vector3(3, -4.5f, 97.4f),
               new Vector3(9999f, -43f, 0.019f),
               new Vector3(55.3f, -0.01f, -130)
            },
            {
               new Vector3(0, -1.2e+19f, 97.4f),
               new Vector3(9999f, -100000f, 0.019f),
               new Vector3(55.3f, -0.01f, -130)
            },
            {
               new Vector3(3, -4.5f, 5555.5555f),
               new Vector3(12321.4441f, -0.000001f, 0.019f),
               new Vector3(1234, -982.3f, -299)
            },
       };

        fieldName = "oness";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
            var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

            Array array = (Array)addr.GetValue(handleVoid);
            ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

            d = __makeref(array);
            byte** pp = *(byte***)(&d);

            DebugLog("取值后 输出\n1,2,252,1331,55,66,11,898,13,-19,-1,-999999,4576,8198,0,0,4176,8958: ");

            int yzL = arrayWrapOutData.arrayLengths[1] * arrayWrapOutData.arrayLengths[2];
            int zL = arrayWrapOutData.arrayLengths[2];
            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++)
                    {
                        int value = *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize *
                            (x * yzL + y * zL + z)
                            );
                        sb.Append(value + ",");
                    }
                }
            }
            DebugLog(sb.ToString());

            for (int i = 0; i < arrayWrapOutData.length; i++)
            {
                *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i) = i;
            }
            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                    {
                        *(int*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yzL + y * zL + z)) = i;
                    }
                }
            }
            DebugLog("赋值后 输出\n0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17: ");
            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++)
                    {
                        sb.Append(array.GetValue(x, y, z) + ",");
                    }
                }
            }
            DebugLog(sb.ToString());


            DebugLog("不指定类型取值后 输出\n0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17: ");
            sb.Clear();
            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                    {
                        object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yzL + y * zL + z));
                        sb.Append(value + ",");
                    }
                }
            }
            DebugLog(sb.ToString());


            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                    {
                        arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yzL + y * zL + z), -i);
                    }
                }
            }
            DebugLog("不指定类型取值后 输出\n0,-1,-2,-3,-4,-5,-6,-7,-8,-9,-10,-11,-12,-13,-14,-15,-16,-17: ");
            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++)
                    {
                        sb.Append(arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, x * yzL + y * zL + z) + ",");
                    }
                }
            }
            DebugLog(sb.ToString());


        }

        fieldName = "strss";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
            var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

            Array array = (Array)addr.GetValue(handleVoid);
            ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

            d = __makeref(array);
            byte** pp = *(byte***)(&d);

            DebugLog("取值后 输出\nass,#$%^&,*SAHASww&(),兀驦屮鲵傌,adsadad,⑤驦屮尼傌,fun(a*(b+c)),FSD手动阀手动阀: ");
            int yL = arrayWrapOutData.arrayLengths[1];
            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    string value = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y)));
                    sb.Append(value + ",");
                }
            }
            DebugLog(sb.ToString());



            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                {
                    GeneralTool.SetObject(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y), "Ac4……*" + i);
                }
            }
            sb.Clear();
            DebugLog("赋值后 输出\nAc4……*0,Ac4……*1,Ac4……*2,Ac4……*3,Ac4……*4,Ac4……*5,Ac4……*6,Ac4……*7: ");
            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                {
                    sb.Append(array.GetValue(x, y) + ",");
                }
            }
            DebugLog(sb.ToString());

            sb.Clear();
            DebugLog("不指定类型取值后 输出\nAc4……*0,Ac4……*1,Ac4……*2,Ac4……*3,Ac4……*4,Ac4……*5,Ac4……*6,Ac4……*7: ");
            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                {
                    object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yL + y));
                    sb.Append(value + ",");
                }
            }
            DebugLog(sb.ToString());


            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                {
                    arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, i, "Fc%^" + (x * yL + y) * 100);
                }
            }
            DebugLog("不指定类型赋值后 输出\nFc%^0,Fc%^100,Fc%^200,Fc%^300,Fc%^400,Fc%^500,Fc%^600,Fc%^700: ");
            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    sb.Append(array.GetValue(x, y) + ",");
                }
            }
            DebugLog(sb.ToString());

        }

        fieldName = "pointss";
        fixed (char* fieldNamePtr = fieldName)
        {
            TypeAddrFieldAndProperty addr = warp.Find(fieldNamePtr, fieldName.Length);
            var arrayWrap = ArrayWrapManager.GetIArrayWrap(addr.fieldOrPropertyType);

            Array array = (Array)addr.GetValue(handleVoid);
            ArrayWrapData arrayWrapOutData = arrayWrap.GetArrayData(array);

            d = __makeref(array);
            byte** pp = *(byte***)(&d);

            DebugLog("取值后 输出\n(3,-4.5,97.4),(9999,-43,0.019),(55.3,-0.01,-130),(0,-1.2E+19,97.4),(9999,-100000,0.019),(55.3,-0.01,-130),(3,-4.5,5555.5557),(12321.444,-1E-06,0.019),(1234,-982.3,-299): ");
            int yL = arrayWrapOutData.arrayLengths[1];
            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    Vector3 value = *(Vector3*)(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y));
                    sb.Append(value + ",");
                }
            }
            DebugLog(sb.ToString());


            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                {
                    var v = new Vector3(i, i, i);
                    GeneralTool.MemCpy(*pp + arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y), GeneralTool.AsPointer(ref v), arrayWrap.elementTypeSize);
                }
            }
            DebugLog("赋值后 输出\n(0,0,0),(1,1,1),(2,2,2),(3,3,3),(4,4,4),(5,5,5),(6,6,6),(7,7,7),(8,8,8): ");
            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    sb.Append(array.GetValue(x, y) + ",");
                }
            }
            DebugLog(sb.ToString());


            DebugLog("不指定类型取值后 输出\n(0,0,0),(1,1,1),(2,2,2),(3,3,3),(4,4,4),(5,5,5),(6,6,6),(7,7,7),(8,8,8): ");
            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    //object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, i);
                    object value = arrayWrap.GetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yL + y));
                    sb.Append(value + ",");
                }
            }
            DebugLog(sb.ToString());


            for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                {
                    arrayWrap.SetValue(*pp + arrayWrapOutData.startItemOffcet, (x * yL + y), new Vector3(i * 100, i * 10, i * 1000));
                }
            }
            DebugLog("不指定类型赋值后 输出\n(0,0,0),(100,10,1000),(200,20,2000),(300,30,3000),(400,40,4000),(500,50,5000),(600,60,6000),(700,70,7000),(800,80,8000): ");

            sb.Clear();
            for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
            {
                for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                {
                    sb.Append(array.GetValue(x, y) + ",");
                }
            }
            DebugLog(sb.ToString());
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

    string[,] v1ss;
    int[,,] v2ss;
    Vector3[,] v3ss;

    string v1;
    int v2;
    Vector3 v3;
    MyClass a;
    MyClass myClass;

    ///*
    public unsafe void RunTest()
    {
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
            var addr1 = warp.Find(_str, fieldName_strLength);
            var addr2 = warp.Find(_one, fieldName_oneLength);
            var addr3 = warp.Find(_point, fieldName_pointLength);

            DebugLog("");
            
            DebugLog("");


            DebugLog("");
            DebugLog("=========↓↓↓FieldInfo↓↓↓=============");
            DebugLog("Set:");
            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                typeof(MyClass).GetField(fieldName_str).SetValue(myClass, v1);
                typeof(MyClass).GetField(fieldName_one).SetValue(myClass, v2);
                typeof(MyClass).GetField(fieldName_point).SetValue(myClass, v3);
            }
            oTime.Stop();
            DebugLog("原生反射 FieldInfo SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                warp.nameOfField[fieldName_str].SetValue(handleVoid, v1);
                warp.nameOfField[fieldName_one].SetValue(handleVoid, v2);
                warp.nameOfField[fieldName_point].SetValue(handleVoid, v3);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 使用object类型 string查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                warp.Find(_str, fieldName_strLength).SetValue(handleVoid, v1);
                warp.Find(_one, fieldName_oneLength).SetValue(handleVoid, v2);
                warp.Find(_point, fieldName_pointLength).SetValue(handleVoid, v3);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 使用object类型 char*查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                GeneralTool.SetObject(*handleByte + warp.Find(_str, fieldName_strLength).offset, v1);
                *(int*)(*handleByte + warp.Find(_one, fieldName_oneLength).offset) = v2;
                warp.Find(_point, fieldName_pointLength).WriteStruct<Vector3>(handleVoid, v3);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 确定类型的 char*查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                GeneralTool.SetObject(*handleByte + addr1.offset, v1);
                *(int*)(*handleByte + addr2.offset) = v2;
                addr3.WriteStruct<Vector3>(handleVoid, v3);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 忽略字符串查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                myClass.str = v1;
                myClass.one = v2;
                myClass.point = v3;
            }
            oTime.Stop();
            DebugLog("原生 SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            DebugLog("");
            
            DebugLog("Get:");
            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)typeof(MyClass).GetField(fieldName_str).GetValue(myClass);
                v2 = (int)typeof(MyClass).GetField(fieldName_one).GetValue(myClass);
                v3 = (Vector3)typeof(MyClass).GetField(fieldName_point).GetValue(myClass);
            }
            oTime.Stop();
            DebugLog("原生反射 FieldInfo GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)warp.nameOfField[fieldName_str].GetValue(handleVoid);
                v2 = (int)warp.nameOfField[fieldName_one].GetValue(handleVoid);
                v3 = (Vector3)warp.nameOfField[fieldName_point].GetValue(handleVoid);
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
                v2 = *(int*)(*handleByte + warp.Find(_one, fieldName_oneLength).offset);
                v3 = warp.Find(_point, fieldName_pointLength).ReadStruct<Vector3>(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 确定类型的 char*查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)GeneralTool.VoidPtrToObject(*(void**)(*handleByte + addr1.offset));
                v2 = *(int*)(*handleByte + addr2.offset);
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
            DebugLog("原生 GetValue ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            DebugLog("");
            DebugLog("=========↓↓↓Property↓↓↓=============");
            DebugLog("Set:");

            addr1 = warp.Find(_Str, fieldName_StrLength);
            addr2 = warp.Find(_One, fieldName_OneLength);
            addr3 = warp.Find(_Point, fieldName_PointLength);
            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                typeof(MyClass).GetProperty(fieldName_Str).SetValue(myClass, str);
                typeof(MyClass).GetProperty(fieldName_One).SetValue(myClass, 18);
                typeof(MyClass).GetProperty(fieldName_Point).SetValue(myClass, point);
            }
            oTime.Stop();
            DebugLog("原生反射 PropertyInfo SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                warp.nameOfField[fieldName_Str].SetValue(handleVoid, str);
                warp.nameOfField[fieldName_One].SetValue(handleVoid, 18);
                warp.nameOfField[fieldName_Point].SetValue(handleVoid, point);
            }

            oTime.Stop();
            DebugLog("指针方法 SetValue 使用object类型 string查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                warp.Find(_Str, fieldName_StrLength).SetValue(handleVoid, str);
                warp.Find(_One, fieldName_OneLength).SetValue(handleVoid, 18);
                warp.Find(_Point, fieldName_PointLength).SetValue(handleVoid, point);
            }

            oTime.Stop();
            DebugLog("指针方法 SetValue 使用object类型 char*查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                warp.Find(_Str, fieldName_StrLength).propertyDelegateItem.setString(*handleVoid, str);
                warp.Find(_One, fieldName_OneLength).propertyDelegateItem.setInt32(*handleVoid, 18);
                warp.Find(_Point, fieldName_PointLength).propertyDelegateItem.setObject(*handleVoid, point);
            }
            oTime.Stop();
            DebugLog("指针方法 SetValue 确定类型的 char*查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

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
            DebugLog("原生 SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            DebugLog("");
            
            DebugLog("Get:");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)typeof(MyClass).GetProperty(fieldName_Str).GetValue(myClass);
                v2 = (int)typeof(MyClass).GetProperty(fieldName_One).GetValue(myClass);
                v3 = (Vector3)typeof(MyClass).GetProperty(fieldName_Point).GetValue(myClass);
            }
            oTime.Stop();
            DebugLog("原生反射 PropertyInfo GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)warp.nameOfField[fieldName_Str].GetValue(handleVoid);
                v2 = (int)warp.nameOfField[fieldName_One].GetValue(handleVoid);
                v3 = (Vector3)warp.nameOfField[fieldName_Point].GetValue(handleVoid);
            }

            oTime.Stop();
            DebugLog("指针方法 GetValue 使用object类型 string查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = (string)warp.Find(_Str, fieldName_StrLength).GetValue(handleVoid);
                v2 = (int)warp.Find(_One, fieldName_OneLength).GetValue(handleVoid);
                v3 = (Vector3)warp.Find(_Point, fieldName_PointLength).GetValue(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 使用object类型 char*查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1 = warp.Find(_Str, fieldName_StrLength).propertyDelegateItem.getString(handleVoid);
                v2 = warp.Find(_One, fieldName_OneLength).propertyDelegateItem.getInt32(handleVoid);
                v3 = (Vector3)warp.Find(_Point, fieldName_PointLength).propertyDelegateItem.getObject(handleVoid);
            }
            oTime.Stop();
            DebugLog("指针方法 GetValue 确定类型的 char*查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

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
                v1 = myClass.Str;
                v2 = myClass.One;
                v3 = myClass.Point;
            }
            oTime.Stop();
            DebugLog("原生 GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            DebugLog("");
            DebugLog("=========↓↓↓Array↓↓↓=========");
            DebugLog("Get:");
            v1s = new string[] { "Ac", "S@D", "CS@", "CS2ss", "CXaa" };
            v2s = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            v3s = new Vector3[] { new Vector3(1, 2, 3), new Vector3(11, -2, 41.3f), new Vector3(19.999f, 9, 0), new Vector3(123.454f, 647.4f, 11) };
            var arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            var arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            var arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));

            ObjReference v1sVar = new ObjReference(v1s);
            byte** v1sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1sVar);

            ObjReference v2sVar = new ObjReference(v2s);
            byte** v2sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2sVar);

            ObjReference v3sVar = new ObjReference(v3s);
            byte** v3sP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3sVar);

            //var d = __makeref(v1s);
            //byte** v1sP = *(byte***)(&d);
            //d = __makeref(v2s);
            //byte** v2sP = *(byte***)(&d);
            //d = __makeref(v3s);
            //byte** v3sP = *(byte***)(&d);


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
            DebugLog("原生反射 Array GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    v1 = (string)arrayWrapV1.GetValue(*v1sP + data1.startItemOffcet, j);
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    v2 = (int)arrayWrapV2.GetValue(*v2sP + data2.startItemOffcet, j);
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    v3 = (Vector3)arrayWrapV3.GetValue(*v3sP + data3.startItemOffcet, j);
                }
            }
            oTime.Stop();
            DebugLog("指针方法 ArrayWrapManager GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data1.length; j++)
                {
                    v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*v1sP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * j));
                }
                for (int j = 0; j < data2.length; j++)
                {
                    v2 = *(int*)(*v2sP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * j);
                }
                for (int j = 0; j < data3.length; j++)
                {
                    v3 = *(Vector3*)(*v3sP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * j);
                }
            }
            oTime.Stop();
            DebugLog("指针方法 ArrayWrapManager GetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    for (int j = 0; j < data1.length; j++)
                    {
                        v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*v1sP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * j));
                    }
                    for (int j = 0; j < data2.length; j++)
                    {
                        v2 = *(int*)(*v2sP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * j);
                    }
                    for (int j = 0; j < data3.length; j++)
                    {
                        v3 = *(Vector3*)(*v3sP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * j);
                    }
                }
                oTime.Stop();
            }
            DebugLog("指针方法 ArrayWrapManager GetValue 确定类型的 忽略Data查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


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


            DebugLog("");
            
            DebugLog("Set:");
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
            DebugLog("原生反射 Array SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    arrayWrapV1.SetValue(*v1sP + data1.startItemOffcet, j, v1);
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    arrayWrapV2.SetValue(*v2sP + data2.startItemOffcet, j, v2);
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    arrayWrapV3.SetValue(*v3sP + data3.startItemOffcet, j, v3);
                }
            }
            oTime.Stop();
            DebugLog("指针方法 ArrayWrapManager SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                for (int j = 0; j < data1.length; j++)
                {
                    GeneralTool.SetObject(*v1sP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * j, v1);
                }

                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                for (int j = 0; j < data2.length; j++)
                {
                    *(int*)(*v2sP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * j) = v2;
                }

                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                for (int j = 0; j < data3.length; j++)
                {
                    var v = new Vector3(j, j, j);
                    GeneralTool.MemCpy(*v3sP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * j, GeneralTool.AsPointer(ref v), arrayWrapV3.elementTypeSize);
                }
            }
            oTime.Stop();
            DebugLog("指针方法 ArrayWrapManager SetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1s);
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2s);
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3s);
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    for (int j = 0; j < data1.length; j++)
                    {
                        GeneralTool.SetObject(*v1sP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * j, v1);
                    }

                    for (int j = 0; j < data2.length; j++)
                    {
                        *(int*)(*v2sP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * j) = v2;
                    }

                    for (int j = 0; j < data3.length; j++)
                    {
                        var v = new Vector3(i, i, i);
                        GeneralTool.MemCpy(*v3sP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * j, GeneralTool.AsPointer(ref v), arrayWrapV3.elementTypeSize);
                    }
                }
                oTime.Stop();
            }
            DebugLog("指针方法 ArrayWrapManager SetValue 确定类型的 忽略Data查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


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

            //d = __makeref(v1ss);
            //v1sP = *(byte***)(&d);
            //d = __makeref(v2ss);
            //v2sP = *(byte***)(&d);
            //d = __makeref(v3ss);
            //v3sP = *(byte***)(&d);



            DebugLog("");
            DebugLog("=============↓↓↓Multidimensional Array↓↓↓===============");
            DebugLog("Get:");
            v1ss = new string[2, 4] {
                { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" },
                { "adsadad", "⑤驦屮尼傌", "fun(a*(b+c))", "FSD手动阀手动阀" },
            };
            v2ss = new int[3, 3, 2]
            {
                { { 1, 2 },{  252, 1331 },{ 55, 66 } },
                { { 11, 898},{ 13, -19 },{ -1, -999999 } },
                { { 4576, 8198 },{ 0, 0 },{ 4176, 8958 } }
            };
            v3ss = new Vector3[3, 3] {
                {
                   new Vector3(3, -4.5f, 97.4f),
                   new Vector3(9999f, -43f, 0.019f),
                   new Vector3(55.3f, -0.01f, -130)
                },
                {
                   new Vector3(0, -1.2e+19f, 97.4f),
                   new Vector3(9999f, -100000f, 0.019f),
                   new Vector3(55.3f, -0.01f, -130)
                },
                {
                   new Vector3(3, -4.5f, 5555.5555f),
                   new Vector3(12321.4441f, -0.000001f, 0.019f),
                   new Vector3(1234, -982.3f, -299)
                },
            };
            ObjReference v1ssVar = new ObjReference(v1ss);
            byte** v1ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v1ssVar);

            ObjReference v2ssVar = new ObjReference(v2ss);
            byte** v2ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v2ssVar);

            ObjReference v3ssVar = new ObjReference(v3ss);
            byte** v3ssP = (byte**)GeneralTool.AsPointer<ObjReference>(ref v3ssVar);

            arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());



            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                Array arrayV1 = v1ss;
                for (int x = 0; x < arrayV1.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV1.GetLength(1); y++)
                    {
                        v1 = (string)arrayV1.GetValue(x, y);
                    }
                }

                Array arrayV2 = v2ss;
                for (int x = 0; x < arrayV2.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV2.GetLength(1); y++)
                    {
                        for (int z = 0; z < arrayV2.GetLength(2); z++)
                        {
                            v2 = (int)arrayV2.GetValue(x, y, z);
                        }
                    }
                }

                Array arrayV3 = v3ss;
                for (int x = 0; x < arrayV3.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV3.GetLength(1); y++)
                    {
                        v3 = (Vector3)arrayV3.GetValue(x, y);
                    }
                }
            }
            oTime.Stop();
            DebugLog("原生反射 Multidimensional Array GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        //v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                        v1 = (string)arrayWrapV1.GetValue(*v1ssP + data1.startItemOffcet, x * data1.arrayLengths[1] + y);
                    }
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            v2 = (int)arrayWrapV2.GetValue(*v2ssP + data2.startItemOffcet, (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z));
                        }
                    }
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        v3 = (Vector3)arrayWrapV3.GetValue(*v3ssP + data3.startItemOffcet, x * data3.arrayLengths[1] + y);
                    }
                }
            }
            oTime.Stop();
            DebugLog("指针方法 Multidimensional ArrayWrapManager GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*v1ssP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                    }
                }

                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            v2 = *(int*)(*v2ssP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z));
                        }
                    }
                }

                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        v3 = *(Vector3*)(*v3ssP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y));
                    }
                }
            }
            oTime.Stop();
            DebugLog("指针方法 Multidimensional ArrayWrapManager GetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    for (int x = 0; x < data1.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data1.arrayLengths[1]; y++)
                        {
                            v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(*v1ssP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                        }
                    }
                    for (int x = 0; x < data2.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data2.arrayLengths[1]; y++)
                        {
                            for (int z = 0; z < data2.arrayLengths[2]; z++)
                            {
                                v2 = *(int*)(*v2ssP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z));
                            }
                        }
                    }
                    for (int x = 0; x < data3.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data3.arrayLengths[1]; y++)
                        {
                            v3 = *(Vector3*)(*v3ssP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y));
                        }
                    }
                }
            }
            oTime.Stop();
            DebugLog("指针方法 Multidimensional ArrayWrapManager GetValue 忽略Data查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                for (int x = 0; x < v1ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v1ss.GetLength(1); y++)
                    {
                        v1 = v1ss[x, y];
                    }
                }
                for (int x = 0; x < v2ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v2ss.GetLength(1); y++)
                    {
                        for (int z = 0; z < v2ss.GetLength(2); z++)
                        {
                            v2 = v2ss[x, y, z];
                        }
                    }
                }
                for (int x = 0; x < v3ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v3ss.GetLength(1); y++)
                    {
                        v3 = v3ss[x, y];
                    }
                }
            }
            oTime.Stop();
            DebugLog("原生 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            DebugLog("");
            DebugLog("Set:");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                Array arrayV1 = v1ss;
                for (int x = 0; x < arrayV1.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV1.GetLength(1); y++)
                    {
                        arrayV1.SetValue(v1, x, y);
                    }
                }

                Array arrayV2 = v2ss;
                for (int x = 0; x < arrayV2.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV2.GetLength(1); y++)
                    {
                        for (int z = 0; z < arrayV2.GetLength(2); z++)
                        {
                            arrayV2.SetValue(v2, x, y, z);
                        }
                    }
                }

                Array arrayV3 = v3ss;
                for (int x = 0; x < arrayV3.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayV3.GetLength(1); y++)
                    {
                        arrayV3.SetValue(v3, x, y);
                    }
                }
            }
            oTime.Stop();
            DebugLog("原生反射 Multidimensional Array SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        //v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                        arrayWrapV1.SetValue(*v1ssP + data1.startItemOffcet, x * data1.arrayLengths[1] + y, v1);
                    }
                }
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            arrayWrapV2.SetValue(*v2ssP + data2.startItemOffcet, (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z), v2);
                        }
                    }
                }
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        arrayWrapV3.SetValue(*v3ssP + data3.startItemOffcet, x * data3.arrayLengths[1] + y, v3);
                    }
                }
            }
            oTime.Stop();
            DebugLog("指针方法 Multidimensional ArrayWrapManager SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                for (int x = 0; x < data1.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data1.arrayLengths[1]; y++)
                    {
                        GeneralTool.SetObject(
                            (*v1ssP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)),
                            v1);
                    }
                }

                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                for (int x = 0; x < data2.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data2.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < data2.arrayLengths[2]; z++)
                        {
                            *(int*)(*v2ssP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z))
                                = v2;
                        }
                    }
                }

                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                for (int x = 0; x < data3.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < data3.arrayLengths[1]; y++)
                    {
                        // GeneralTool.MemCpy((data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y)), GeneralTool.AsPointer(ref v3), arrayWrapV3.elementTypeSize);
                        *(Vector3*)(*v3ssP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y)) = v3;
                    }
                }

            }
            oTime.Stop();
            DebugLog("指针方法 Multidimensional ArrayWrapManager SetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            {
                ArrayWrapData data1 = arrayWrapV1.GetArrayData(v1ss);
                ArrayWrapData data2 = arrayWrapV2.GetArrayData(v2ss);
                ArrayWrapData data3 = arrayWrapV3.GetArrayData(v3ss);
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    for (int x = 0; x < data1.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data1.arrayLengths[1]; y++)
                        {
                            GeneralTool.SetObject(*v1ssP + data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y), v1);
                        }
                    }
                    for (int x = 0; x < data2.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data2.arrayLengths[1]; y++)
                        {
                            for (int z = 0; z < data2.arrayLengths[2]; z++)
                            {
                                *(int*)(*v2ssP + data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[2] + z)) = v2;
                            }
                        }
                    }
                    for (int x = 0; x < data3.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data3.arrayLengths[1]; y++)
                        {
                            *(Vector3*)(*v3ssP + data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y)) = v3;
                        }
                    }
                }
            }
            oTime.Stop();
            DebugLog("指针方法 Multidimensional ArrayWrapManager SetValue 忽略Data查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                for (int x = 0; x < v1ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v1ss.GetLength(1); y++)
                    {
                        v1ss[x, y] = v1;
                    }
                }
                for (int x = 0; x < v2ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v2ss.GetLength(1); y++)
                    {
                        for (int z = 0; z < v2ss.GetLength(2); z++)
                        {
                            v2ss[x, y, z] = v2;
                        }
                    }
                }
                for (int x = 0; x < v3ss.GetLength(0); x++)
                {
                    for (int y = 0; y < v3ss.GetLength(1); y++)
                    {
                        v3ss[x, y] = v3;
                    }
                }
            }
            oTime.Stop();
            DebugLog("原生 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            DebugLog("");
            DebugLog("=============↓↓↓CreateInstance↓↓↓===============");
            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                a = (MyClass)System.Activator.CreateInstance(typeof(MyClass));
            }
            oTime.Stop();
            DebugLog("原生反射 Activator CreateInstance：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                a = (MyClass)warp.Create();
            }
            oTime.Stop();
            DebugLog("指针方法 Create Class：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                a = new MyClass();
            }
            oTime.Stop();
            DebugLog("原生 new Class ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");
            DebugLog("");


            arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(typeof(string[]));
            arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(typeof(int[]));
            arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(typeof(Vector3[]));
            {
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {

                    ArrayWrapData arrayWrapData1 = new ArrayWrapData
                    {
                        length = 5
                    };
                    v1s = (string[])arrayWrapV1.CreateArray(ref arrayWrapData1);

                    ArrayWrapData arrayWrapData2 = new ArrayWrapData
                    {
                        length = 10
                    };
                    v2s = (int[])arrayWrapV2.CreateArray(ref arrayWrapData2);

                    ArrayWrapData arrayWrapData3 = new ArrayWrapData
                    {
                        length = 4
                    };
                    v3s = (Vector3[])arrayWrapV3.CreateArray(ref arrayWrapData3);

                }
                oTime.Stop();
            }
            DebugLog("指针方法 创建数组：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");
            
            {
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    arrayWrapV1.allLength = 5;
                    v1s = (string[])arrayWrapV1.CreateArray();
                    arrayWrapV2.allLength = 10;
                    v2s = (int[])arrayWrapV2.CreateArray();
                    arrayWrapV3.allLength = 4;
                    v3s = (Vector3[])arrayWrapV3.CreateArray();
                }
                oTime.Stop();
            }
            DebugLog("指针方法 创建数组 (不需要多余返回值)：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1s = (string[])Array.CreateInstance(typeof(string), 5);
                v2s = (int[])Array.CreateInstance(typeof(int), 10);
                v3s = (Vector3[])Array.CreateInstance(typeof(Vector3), 4);
            }
            oTime.Stop();
            DebugLog("原生反射 创建数组 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1s = new string[5];
                v2s = new int[10];
                v3s = new Vector3[4];
            }
            oTime.Stop();
            DebugLog("原生 new 数组 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            DebugLog("");


            arrayWrapV1 = ArrayWrapManager.GetIArrayWrap(v1ss.GetType());
            arrayWrapV2 = ArrayWrapManager.GetIArrayWrap(v2ss.GetType());
            arrayWrapV3 = ArrayWrapManager.GetIArrayWrap(v3ss.GetType());
            {
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    ArrayWrapData arrayWrapData1 = new ArrayWrapData();
                    arrayWrapData1.arrayLengths = new int[] { 2, 4 };
                    v1ss = (string[,])arrayWrapV1.CreateArray(ref arrayWrapData1);

                    ArrayWrapData arrayWrapData2 = new ArrayWrapData();
                    arrayWrapData2.arrayLengths = new int[] { 3, 3, 2 };
                    v2ss = (int[,,])arrayWrapV2.CreateArray(ref arrayWrapData2);

                    ArrayWrapData arrayWrapData3 = new ArrayWrapData();
                    arrayWrapData3.arrayLengths = new int[] { 3, 3 };
                    v3ss = (Vector3[,])arrayWrapV3.CreateArray(ref arrayWrapData3);
                }
                oTime.Stop();
            }
            DebugLog("指针方法 创建多维数组：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            {
                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    arrayWrapV1.lengths[0] = 2;
                    arrayWrapV1.lengths[1] = 4;
                    arrayWrapV1.allLength = 2 * 4;
                    v1ss = (string[,])arrayWrapV1.CreateArray();

                    arrayWrapV2.lengths[0] = 3;
                    arrayWrapV2.lengths[1] = 3;
                    arrayWrapV2.lengths[2] = 2;
                    arrayWrapV2.allLength = 3 * 3 * 2;
                    v2ss = (int[,,])arrayWrapV2.CreateArray();

                    arrayWrapV3.lengths[0] = 3;
                    arrayWrapV3.lengths[1] = 3;
                    arrayWrapV3.allLength = 3 * 3;
                    v3ss = (Vector3[,])arrayWrapV3.CreateArray();
                }
                oTime.Stop();
            }
            DebugLog("指针方法 创建多维数组(不需要多余返回值)：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1ss = (string[,])Array.CreateInstance(typeof(string), new int[] { 2, 4 });
                v2ss = (int[,,])Array.CreateInstance(typeof(int), new int[] { 3, 3, 2 });
                v3ss = (Vector3[,])Array.CreateInstance(typeof(Vector3), new int[] { 3, 3 });
            }
            oTime.Stop();
            DebugLog("原生反射 创建多维数组 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int i = 0; i < testCount; i++)
            {
                v1ss = new string[2, 4];
                v2ss = new int[3, 3, 2];
                v3ss = new Vector3[3, 3];
            }
            oTime.Stop();
            DebugLog("原生 new 多维数组 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            GC.Collect();
        }
    }
    //*/


}
