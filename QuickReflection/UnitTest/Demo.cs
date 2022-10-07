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
using PtrReflection;

namespace UnitTest
{
    public unsafe class Demo
    {
        public int testCount = 100000;
        System.Diagnostics.Stopwatch oTime = new System.Diagnostics.Stopwatch();

        StringBuilder sb = new StringBuilder();
        void DebugLog(object obj)
        {
            Console.WriteLine(Convert.ToString(obj));
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

        [Test]
        public unsafe void RunTest2()
        {
            StringBuilder sb = new StringBuilder();
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
                ArrayWrapOutData arrayWrapOutData = arrayWrap.GetArrayData(array);

                DebugLog("取值后 输出 1, 2, 8, 476, 898, 9  : ");
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
                ArrayWrapOutData arrayWrapOutData = arrayWrap.GetArrayData(array);

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
                            int value = *(int*)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize *
                                (x * yzL + y * zL + z)
                                );
                            sb.Append(value + ",");
                        }
                    }
                }
                DebugLog(sb.ToString());

                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    *(int*)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * i) = i;
                }
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                        {
                            *(int*)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yzL + y * zL + z)) = i;
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
                for (int i = 0; i < arrayWrapOutData.length; i++)
                {
                    object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, i);
                    DebugLog(value);
                }
                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        for (int z = 0; z < arrayWrapOutData.arrayLengths[2]; z++, i++)
                        {
                            object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, (x * yzL + y * zL + z));
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
                            arrayWrap.SetValue(arrayWrapOutData.startItemOffcet, (x * yzL + y * zL + z), -i);
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
                            sb.Append(arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, x * yzL + y * zL + z) + ",");
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
                ArrayWrapOutData arrayWrapOutData = arrayWrap.GetArrayData(array);

                DebugLog("取值后 输出\nass,#$%^&,*SAHASww&(),兀驦屮鲵傌,adsadad,⑤驦屮尼傌,fun(a*(b+c)),FSD手动阀手动阀: ");
                int yL = arrayWrapOutData.arrayLengths[1];
                sb.Clear();
                for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        string value = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y)));
                        sb.Append(value + ",");
                    }
                }
                DebugLog(sb.ToString());


                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        GeneralTool.SetObject(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y), "Ac4……*" + i);
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
                        object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, (x * yL + y));
                        sb.Append(value + ",");
                    }
                }
                DebugLog(sb.ToString());


                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        arrayWrap.SetValue(arrayWrapOutData.startItemOffcet, i, "Fc%^" + (x * yL + y) * 100);
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
                ArrayWrapOutData arrayWrapOutData = arrayWrap.GetArrayData(array);

                DebugLog("取值后 输出\n(3,-4.5,97.4),(9999,-43,0.019),(55.3,-0.01,-130),(0,-1.2E+19,97.4),(9999,-100000,0.019),(55.3,-0.01,-130),(3,-4.5,5555.5557),(12321.444,-1E-06,0.019),(1234,-982.3,-299): ");
                int yL = arrayWrapOutData.arrayLengths[1];
                sb.Clear();
                for (int x = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++)
                    {
                        Vector3 value = *(Vector3*)(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y));
                        sb.Append(value + ",");
                    }
                }
                DebugLog(sb.ToString());


                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        var v = new Vector3(i, i, i);
                        GeneralTool.MemCpy(arrayWrapOutData.startItemOffcet + arrayWrap.elementTypeSize * (x * yL + y), GeneralTool.AsPointer(ref v), arrayWrap.elementTypeSize);
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
                        object value = arrayWrap.GetValue(arrayWrapOutData.startItemOffcet, (x * yL + y));
                        sb.Append(value + ",");
                    }
                }
                DebugLog(sb.ToString());


                for (int x = 0, i = 0; x < arrayWrapOutData.arrayLengths[0]; x++)
                {
                    for (int y = 0; y < arrayWrapOutData.arrayLengths[1]; y++, i++)
                    {
                        arrayWrap.SetValue(arrayWrapOutData.startItemOffcet, (x * yL + y), new Vector3(i * 100, i * 10, i * 1000));
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

        [Test]
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
                var addr1 = warp.Find(_str, fieldName_strLength);
                var addr2 = warp.Find(_one, fieldName_oneLength);
                var addr3 = warp.Find(_point, fieldName_pointLength);

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

                addr1 = warp.Find(_Str, fieldName_StrLength);
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
                    v1 = (string)warp.Find(_Str, fieldName_StrLength).GetValue(handleVoid);
                    v2 = (int)warp.Find(_One, fieldName_OneLength).GetValue(handleVoid);
                    v3 = (Vector3)warp.Find(_Point, fieldName_PointLength).GetValue(handleVoid);
                }
                oTime.Stop();
                DebugLog("指针方法 GetValue 使用object类型 ：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    v1 = warp.Find(_Str, fieldName_StrLength).propertyDelegateItem.getString(handleVoid);
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
                    ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1s);
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
                    ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1s);
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
                DebugLog("=============↓↓↓Multidimensional Array↓↓↓===============");
                DebugLog("");
                v1ss = new string[,] {
                { "ass", "#$%^&", "*SAHASww&()", "兀驦屮鲵傌" },
                { "adsadad", "⑤驦屮尼傌", "fun(a*(b+c))", "FSD手动阀手动阀" },
            };
                v2ss = new int[,,]
                {
                { { 1, 2 },{  252, 1331 },{ 55, 66 } },
                { { 11, 898},{ 13, -19 },{ -1, -999999 } },
                { { 4576, 8198 },{ 0, 0 },{ 4176, 8958 } }
                };
                v3ss = new Vector3[,] {
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
                DebugLog("Multidimensional Array GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1ss);
                    for (int x = 0; x < data1.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data1.arrayLengths[1]; y++)
                        {
                            //v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                            v1 = (string)arrayWrapV1.GetValue(data1.startItemOffcet, x * data1.arrayLengths[1] + y);
                        }
                    }
                    ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2ss);
                    for (int x = 0; x < data2.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data2.arrayLengths[1]; y++)
                        {
                            for (int z = 0; z < data2.arrayLengths[2]; z++)
                            {
                                v2 = (int)arrayWrapV2.GetValue(data2.startItemOffcet, (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[1] + z));
                            }
                        }
                    }
                    ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3ss);
                    for (int x = 0; x < data3.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data3.arrayLengths[1]; y++)
                        {
                            v3 = (Vector3)arrayWrapV3.GetValue(data3.startItemOffcet, x * data3.arrayLengths[1] + y);
                        }
                    }
                }
                oTime.Stop();
                DebugLog("Multidimensional ArrayWrapManager GetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1ss);
                    for (int x = 0; x < data1.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data1.arrayLengths[1]; y++)
                        {
                            v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                        }
                    }

                    ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2ss);
                    for (int x = 0; x < data2.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data2.arrayLengths[1]; y++)
                        {
                            for (int z = 0; z < data2.arrayLengths[2]; z++)
                            {
                                v2 = *(int*)(data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[1] + z));
                            }
                        }
                    }

                    ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3ss);
                    for (int x = 0; x < data3.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data3.arrayLengths[1]; y++)
                        {
                            v3 = *(Vector3*)(data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y));
                        }
                    }
                }
                oTime.Stop();
                DebugLog("Multidimensional ArrayWrapManager GetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

                {
                    ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1ss);
                    ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2ss);
                    ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3ss);
                    oTime.Reset(); oTime.Start();
                    for (int i = 0; i < testCount; i++)
                    {
                        for (int x = 0; x < data1.arrayLengths[0]; x++)
                        {
                            for (int y = 0; y < data1.arrayLengths[1]; y++)
                            {
                                v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                            }
                        }
                        for (int x = 0; x < data2.arrayLengths[0]; x++)
                        {
                            for (int y = 0; y < data2.arrayLengths[1]; y++)
                            {
                                for (int z = 0; z < data2.arrayLengths[2]; z++)
                                {
                                    v2 = *(int*)(data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[1] + z));
                                }
                            }
                        }
                        for (int x = 0; x < data3.arrayLengths[0]; x++)
                        {
                            for (int y = 0; y < data3.arrayLengths[1]; y++)
                            {
                                v3 = *(Vector3*)(data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y));
                            }
                        }
                    }
                }
                oTime.Stop();
                DebugLog("Multidimensional ArrayWrapManager GetValue 忽略Data查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

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
                DebugLog("Multidimensional Array SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1ss);
                    for (int x = 0; x < data1.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data1.arrayLengths[1]; y++)
                        {
                            //v1 = (string)GeneralTool.VoidPtrToObject(*(IntPtr**)(data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)));
                            arrayWrapV1.SetValue(data1.startItemOffcet, x * data1.arrayLengths[1] + y, v1);
                        }
                    }
                    ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2ss);
                    for (int x = 0; x < data2.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data2.arrayLengths[1]; y++)
                        {
                            for (int z = 0; z < data2.arrayLengths[2]; z++)
                            {
                                arrayWrapV2.SetValue(data2.startItemOffcet, (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[1] + z), v2);
                            }
                        }
                    }
                    ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3ss);
                    for (int x = 0; x < data3.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data3.arrayLengths[1]; y++)
                        {
                            arrayWrapV3.SetValue(data3.startItemOffcet, x * data3.arrayLengths[1] + y, v3);
                        }
                    }
                }
                oTime.Stop();
                DebugLog("Multidimensional ArrayWrapManager SetValue：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

                oTime.Reset(); oTime.Start();
                for (int i = 0; i < testCount; i++)
                {
                    ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1ss);
                    for (int x = 0; x < data1.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data1.arrayLengths[1]; y++)
                        {
                            GeneralTool.SetObject(
                                (data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)),
                                v1);
                        }
                    }

                    ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2ss);
                    for (int x = 0; x < data2.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data2.arrayLengths[1]; y++)
                        {
                            for (int z = 0; z < data2.arrayLengths[2]; z++)
                            {
                                *(int*)(data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[1] + z))
                                    = v2;
                            }
                        }
                    }

                    ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3ss);
                    for (int x = 0; x < data3.arrayLengths[0]; x++)
                    {
                        for (int y = 0; y < data3.arrayLengths[1]; y++)
                        {
                            GeneralTool.MemCpy((data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y)), GeneralTool.AsPointer(ref v3), arrayWrapV3.elementTypeSize);
                        }
                    }
                }
                oTime.Stop();
                DebugLog("Multidimensional ArrayWrapManager SetValue 确定类型的：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

                {
                    ArrayWrapOutData data1 = arrayWrapV1.GetArrayData(v1ss);
                    ArrayWrapOutData data2 = arrayWrapV2.GetArrayData(v2ss);
                    ArrayWrapOutData data3 = arrayWrapV3.GetArrayData(v3ss);
                    oTime.Reset(); oTime.Start();
                    for (int i = 0; i < testCount; i++)
                    {
                        for (int x = 0; x < data1.arrayLengths[0]; x++)
                        {
                            for (int y = 0; y < data1.arrayLengths[1]; y++)
                            {
                                GeneralTool.SetObject((data1.startItemOffcet + arrayWrapV1.elementTypeSize * (x * data1.arrayLengths[1] + y)), v1);
                            }
                        }
                        for (int x = 0; x < data2.arrayLengths[0]; x++)
                        {
                            for (int y = 0; y < data2.arrayLengths[1]; y++)
                            {
                                for (int z = 0; z < data2.arrayLengths[2]; z++)
                                {
                                    *(int*)(data2.startItemOffcet + arrayWrapV2.elementTypeSize * (x * data2.arrayLengths[1] * data2.arrayLengths[2] + y * data2.arrayLengths[1] + z)) = v2;
                                }
                            }
                        }
                        for (int x = 0; x < data3.arrayLengths[0]; x++)
                        {
                            for (int y = 0; y < data3.arrayLengths[1]; y++)
                            {
                                *(Vector3*)(data3.startItemOffcet + arrayWrapV3.elementTypeSize * (x * data3.arrayLengths[1] + y)) = v3;
                            }
                        }
                    }
                }
                oTime.Stop();
                DebugLog("Multidimensional ArrayWrapManager SetValue 忽略Data查询：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");

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
        }
    }


}
