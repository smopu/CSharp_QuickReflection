using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PtrReflection
{
    public unsafe class TypeAddrReflectionWrapper
    {
        public IntPtr byteArrayHead = default(IntPtr);
        public static Dictionary<string, FieldInfo> GetAllFieldInfo(Type type)
        {
            Dictionary<string, FieldInfo> nameOfField = new Dictionary<string, FieldInfo>();
            FieldInfo[] typeAddrFieldsNow = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var item in typeAddrFieldsNow)
            {
                if (nameOfField.ContainsKey(item.Name))
                {
                    nameOfField[item.Name] = type.GetField(item.Name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }
                else
                {
                    nameOfField[item.Name] = item;
                }
            }
            var loopType = type;
            while (loopType.BaseType != typeof(object))
            {
                foreach (var item in loopType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (item.Attributes == FieldAttributes.Private)
                    {
                        if (!nameOfField.ContainsKey(item.Name))
                        {
                            nameOfField[item.Name] = item;
                        }
                    }
                }
                loopType = loopType.BaseType;
            }
            return nameOfField;
        }

        public static Dictionary<string, PropertyInfo> GetAllPropertyInfo(Type type)
        {
            Dictionary<string, PropertyInfo> nameOfField = new Dictionary<string, PropertyInfo>();
            PropertyInfo[] typeAddrFieldsNow = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var item in typeAddrFieldsNow)
            {
                if (nameOfField.ContainsKey(item.Name))
                {
                    nameOfField[item.Name] = type.GetProperty(item.Name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }
                else
                {
                    nameOfField[item.Name] = item;
                }
            }
            var loopType = type;
            while (loopType.BaseType != typeof(object))
            {
                foreach (var item in loopType.BaseType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (!nameOfField.ContainsKey(item.Name))
                    {
                        nameOfField[item.Name] = item;
                    }
                }
                loopType = loopType.BaseType;
            }
            return nameOfField;
        }


        ~TypeAddrReflectionWrapper()
        {
            Marshal.FreeHGlobal(delegateValueIntPtr);
        }
        public static bool IsFundamental(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(DateTime));
        }

        MulticastDelegateValue* delegateValue;
        IntPtr delegateValueIntPtr;

        public static TypeAddrReflectionWrapper GetWrapper(Type type)
        {
            TypeAddrReflectionWrapper wrapper;
            if (!all.TryGetValue(type, out wrapper))
            {
                all[type] = wrapper = new TypeAddrReflectionWrapper(type);
            }
            return wrapper;
        }

        static Dictionary<Type, TypeAddrReflectionWrapper> all = new Dictionary<Type, TypeAddrReflectionWrapper>();
        unsafe TypeAddrReflectionWrapper(Type type)
        {
            isValueType = type.IsValueType;
            Dictionary<string, TypeAddrFieldAndProperty> nameOfField = new Dictionary<string, TypeAddrFieldAndProperty>();
            FieldInfo[] typeAddrFieldsNow = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            Type maxFieldType = null;
            int maxOffset = 0;

            foreach (var item in typeAddrFieldsNow)
            {
                int offset = UnsafeOperation.GetFieldOffset(item);
                if (maxOffset < offset)
                {
                    maxOffset = offset;
                    maxFieldType = item.FieldType;
                }
                if (nameOfField.ContainsKey(item.Name))
                {
                    nameOfField[item.Name] = new TypeAddrFieldAndProperty(type.GetField(item.Name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                }
                else
                {
                    nameOfField[item.Name] = new TypeAddrFieldAndProperty(item);
                }
            }

            var loopType = type;
            while (loopType.BaseType != typeof(object))
            {
                foreach (var item in loopType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (item.Attributes == FieldAttributes.Private)
                    {
                        int offset = UnsafeOperation.GetFieldOffset(item);
                        if (maxOffset < offset)
                        {
                            maxOffset = offset;
                            maxFieldType = item.FieldType;
                        }
                        if (!nameOfField.ContainsKey(item.Name))
                        {
                            nameOfField[item.Name] = new TypeAddrFieldAndProperty(item);
                        }
                    }
                }
                loopType = loopType.BaseType;
            }

            //获得所有属性 get set
            //如果属性是值类型且不是基本数据类型，且不是DateTime
            //额外处理
            //计算属性数量 构造非托管内存
            //讲属性方法设置到非托管内存
            //int propertySize = 0;
            PropertyInfo[] propertyInfosNow = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, TypeAddrFieldAndProperty> nameOfProperty = new Dictionary<string, TypeAddrFieldAndProperty>();
            foreach (var item in propertyInfosNow)
            {
                if (nameOfField.ContainsKey(item.Name))
                {
                    //nameOfProperty[item.Name] = new TypeAddrFieldAndProperty(type.GetField(item.Name,
                    //    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                }
                else
                {
                    if (item.Name != "Item")
                    {
                        var property = new TypeAddrFieldAndProperty(type, item);
                        //if (property.isPropertyGet)
                        //{
                        //    ++propertySize;
                        //}
                        //if (property.isPropertySet)
                        //{
                        //    ++propertySize;
                        //}
                        nameOfField[item.Name] = property;
                        nameOfProperty[item.Name] = property;
                    }
                }
            }

            int maxFieldSize = 0;
            if (maxFieldType.IsValueType)
            {
                maxFieldSize = UnsafeOperation.SizeOfStack(maxFieldType);
            }
            else
            {
                maxFieldSize = UnsafeOperation.PTR_COUNT;
            }


            this.heapSize = UnsafeOperation.SizeOfHeap(type);
            this.stackSize = UnsafeOperation.SizeOfStack(type);
            this.sizeByte_1 = this.heapSize - UnsafeOperation.PTR_COUNT * 3;//this.heapSize / PTR_COUNT - 1;

            if (UnsafeOperation.IsCreate(type))
            {
                this.typeHead = UnsafeOperation.GetTypeHead(type);
            }

            this.allTypeField = new TypeAddrFieldAndProperty[nameOfField.Count];
            Dictionary<string, int> strs = new Dictionary<string, int>();
            int indexNow = 0;
            foreach (var item in nameOfField)
            {
                this.allTypeField[indexNow] = item.Value;
                strs[item.Key] = indexNow;
                indexNow++;
            }
            this.nameOfField = nameOfField;
            this.stringTable = new StringTable(strs);

            this.type = type;
        }


        public Dictionary<string, TypeAddrFieldAndProperty> nameOfField;
        StringTable stringTable;
        TypeAddrFieldAndProperty[] allTypeField;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TypeAddrFieldAndProperty Find(char* d, int length)
        {
            int id = stringTable.Find(d, length);
            if (id < 0)
            {
                return null;
            }
            return allTypeField[id];
        }

        /// <summary>
        ///  class struct
        /// </summary>
        public bool isValueType = false;
        public Type type;
        public int stackSize;
        public int heapSize;
        private int sizeByte_1;
        public IntPtr typeHead;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public object Create(out TypedReference handle)//, out byte* objPtr
        //{
        //    object obj = new byte[sizeByte_1];

        //    var d = __makeref(obj);
        //    IntPtr* ptr = **(IntPtr***)(&d);

        //    //handle = UnsafeOperation.GetObjectHandle(obj);
        //    *(IntPtr*)ptr = typeHead;
        //    *((IntPtr*)ptr + 1) = default(IntPtr);
        //    //objPtr = (byte*)ptr;
        //    handle = d;
        //    return obj;
        //}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create()//, out byte* objPtr
        {
            object obj = new byte[sizeByte_1];
            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
            *ptr = typeHead;
            *(ptr + 1) = default(IntPtr);
            return obj;
        }

    }


    public unsafe class TypeAddrFieldAndProperty
    {
        public TypeAddrReflectionWrapper wrapper;
        //public ReadCollectionLink read;
        //public IArrayWrap arrayWrap;

        /// <summary>
        ///  class struct
        /// </summary>
        public bool isValueType = false;

        public TypeAddrFieldAndProperty(FieldInfo fieldInfo)
        {
            this.isPublic = fieldInfo.IsPublic;
            this.isProperty = false;
            this.fieldInfo = fieldInfo;
            this.fieldOrPropertyType = fieldInfo.FieldType;
            offset = UnsafeOperation.GetFieldOffset(fieldInfo);
            typeCode = Type.GetTypeCode(fieldOrPropertyType);
            isValueType = fieldOrPropertyType.IsValueType;
            isArray = fieldOrPropertyType.IsArray;
            isEnum = fieldOrPropertyType.IsEnum;
            if (isValueType)
            {
                stackSize = UnsafeOperation.SizeOfStack(fieldOrPropertyType);
            }
            else
            {
                stackSize = UnsafeOperation.PTR_COUNT;
            }

            if (UnsafeOperation.IsCreate(fieldOrPropertyType))
            {
                typeHead = UnsafeOperation.GetTypeHead(fieldOrPropertyType);
                heapSize = UnsafeOperation.SizeOfHeap(fieldOrPropertyType);  
            }
            else
            {
                heapSize = 0;
            }
        }

        public TypeAddrFieldAndProperty(Type parntType, PropertyInfo propertyInfo)
        {
            this.isPublic = true;
            this.isProperty = true;
            this.propertyInfo = propertyInfo;
            this.fieldOrPropertyType = propertyInfo.PropertyType;
            this.typeCode = Type.GetTypeCode(fieldOrPropertyType);
            this.isValueType = fieldOrPropertyType.IsValueType;
            this.isArray = fieldOrPropertyType.IsArray;
            this.isEnum = fieldOrPropertyType.IsEnum;

            this.isPropertySet = propertyInfo.SetMethod != null;
            this.isPropertyGet = propertyInfo.GetMethod != null;
            this.propertyDelegateItem = new PropertyDelegateItem();
            if (this.isPropertyGet)
            {
                if (isValueType && !TypeAddrReflectionWrapper.IsFundamental(this.fieldOrPropertyType))
                {
                    Delegate sourceDelegate;
                    Delegate get = PropertyWrapper.CreateStructGet(parntType,
                        propertyInfo, out sourceDelegate);
                    this.propertyDelegateItem._get = get;
                }
                else
                {
                    this.propertyDelegateItem._get = PropertyWrapper.CreateClassGet(parntType, propertyInfo);
                }
            }
            if (this.isPropertySet)
            {
                if (isValueType && !TypeAddrReflectionWrapper.IsFundamental(this.fieldOrPropertyType))
                {
                    Delegate sourceDelegate;
                    Delegate set = PropertyWrapper.CreateStructIPropertyWrapperTarget(parntType,
                        propertyInfo, out sourceDelegate);
                    this.propertyDelegateItem._set = set;

                    //this.propertyWrapper = PropertyWrapper.CreateStructIPropertyWrapperTarget(propertyInfo);
                }
                else
                {
                    //this.propertyDelegateItem._set = PropertyWrapper.CreateSetTargetDelegate(propertyInfo);
                    this.propertyDelegateItem._set = PropertyWrapper.CreateClassIPropertyWrapperTarget(parntType, propertyInfo);
                }
            }

            typeHead = UnsafeOperation.GetTypeHead(fieldOrPropertyType);
            //if (isEnum)
            //{
            //    fieldOrPropertyType = typeof(EnumWrapper<>).MakeGenericType(fieldOrPropertyType);
            //}
            //StartReadCollectionLink();
        }


        public bool isArray = false;
        public bool isEnum = false;
        public bool isProperty = false;
        public bool isPublic = false;

        public FieldInfo fieldInfo;
        public IntPtr typeHead;
        public Type fieldOrPropertyType;
        public int offset;
        public int stackSize;
        public int heapSize;
        public TypeCode typeCode;
        

        public bool isPropertySet = true;
        public bool isPropertyGet = true;
        public PropertyInfo propertyInfo;
        public PropertyDelegateItem propertyDelegateItem;
        public IPropertyWrapperTarget propertyWrapper;


        public unsafe T ReadStruct<T>(void** source) where T : struct
        {
            if (isProperty)
            {
                return (T)propertyDelegateItem.getObject((byte*)*source + this.offset);
            }
            else
            {

                T v = new T();
                GeneralTool.MemCpy(GeneralTool.AsPointer<T>(ref v), (byte*)*source + this.offset, stackSize);
                return v;
            }
        }


        public unsafe void WriteStruct<T>(void** source, T v) where T : struct
        {
            if (isProperty)
            {
                propertyDelegateItem.setObject(*source, v);
            }
            else
            {

                GeneralTool.MemCpy((byte*)*source + this.offset, GeneralTool.AsPointer(ref v), stackSize);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetValue(void** source, object value)
        {
            if (isProperty)
            {
                if (this.isValueType)
                {
                    switch (this.typeCode)
                    {
                        case TypeCode.Boolean:
                            propertyDelegateItem.setBoolean(*source, (bool)value);
                            break;
                        case TypeCode.Byte:
                            propertyDelegateItem.setByte(*source, (Byte)value);
                            break;
                        case TypeCode.Char:
                            propertyDelegateItem.setChar(*source, (Char)value);
                            break;
                        case TypeCode.DateTime:
                            propertyDelegateItem.setDateTime(*source, (DateTime)value);
                            break;
                        case TypeCode.Decimal:
                            propertyDelegateItem.setDecimal(*source, (Decimal)value);
                            break;
                        case TypeCode.Double:
                            propertyDelegateItem.setDouble(*source, (Double)value);
                            break;
                        case TypeCode.Empty:
                            break;
                        case TypeCode.Int16:
                            propertyDelegateItem.setInt16(*source, (Int16)value);
                            break;
                        case TypeCode.Int32:
                            propertyDelegateItem.setInt32(*source, (Int32)value);
                            break;
                        case TypeCode.Int64:
                            propertyDelegateItem.setInt64(*source, (Int64)value);
                            break;
                        case TypeCode.SByte:
                            propertyDelegateItem.setSByte(*source, (SByte)value);
                            break;
                        case TypeCode.Single:
                            propertyDelegateItem.setSingle(*source, (Single)value);
                            break;
                        case TypeCode.UInt16:
                            propertyDelegateItem.setUInt16(*source, (UInt16)value);
                            break;
                        case TypeCode.UInt32:
                            propertyDelegateItem.setUInt32(*source, (UInt32)value);
                            break;
                        case TypeCode.UInt64:
                            propertyDelegateItem.setUInt64(*source, (UInt64)value);
                            break;
                        //case TypeCode.String:
                        case TypeCode.Object:
                            propertyDelegateItem.setObject(*source, value);
                            break;
                    }
                }
                else
                {
                    propertyDelegateItem.setObject(*source, value);
                }
            }
            else
            {
                void* field = (byte*)*source + this.offset;
                if (this.isValueType)
                {
                    switch (this.typeCode)
                    {
                        case TypeCode.Boolean:
                            *(bool*)field = (bool)value;
                            break;
                        case TypeCode.Byte:
                            *(Byte*)field = (Byte)value;
                            break;
                        case TypeCode.Char:
                            *(Char*)field = (Char)value;
                            break;
                        case TypeCode.DateTime:
                            *(DateTime*)field = (DateTime)value;
                            break;
                        case TypeCode.Decimal:
                            *(Decimal*)field = (Decimal)value;
                            break;
                        case TypeCode.Double:
                            *(Double*)field = (Double)value;
                            break;
                        case TypeCode.Empty:
                            break;
                        case TypeCode.Int16:
                            *(Int16*)field = (Int16)value;
                            break;
                        case TypeCode.Int32:
                            *(Int32*)field = (Int32)value;
                            break;
                        case TypeCode.Int64:
                            *(Int64*)field = (Int64)value;
                            break;
                        case TypeCode.SByte:
                            *(SByte*)field = (SByte)value;
                            break;
                        case TypeCode.Single:
                            *(Single*)field = (Single)value;
                            break;
                        case TypeCode.UInt16:
                            *(UInt16*)field = (UInt16)value;
                            break;
                        case TypeCode.UInt32:
                            *(UInt32*)field = (UInt32)value;
                            break;
                        case TypeCode.UInt64:
                            *(UInt64*)field = (UInt64)value;
                            break;
                        case TypeCode.String:
                        case TypeCode.Object:
                            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(value);
                            GeneralTool.MemCpy(field, ptr + 1, stackSize);
                            break;
                    }
                }
                else
                {
                    GeneralTool.SetObject(field, value);
                }
            }

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe object GetValue(void** source)
        {
            if (isProperty)
            {
                if (this.isValueType)
                {
                    switch (this.typeCode)
                    {
                        case TypeCode.Boolean:
                            return propertyDelegateItem.getBoolean(*source);
                        case TypeCode.Byte:
                            return propertyDelegateItem.getByte(*source);
                        case TypeCode.Char:
                            return propertyDelegateItem.getChar(*source);
                        case TypeCode.DateTime:
                            return propertyDelegateItem.getDateTime(*source);
                        case TypeCode.Decimal:
                            return propertyDelegateItem.getDecimal(*source);
                        case TypeCode.Double:
                            return propertyDelegateItem.getDouble(*source);
                        case TypeCode.Int16:
                            return propertyDelegateItem.getInt16(*source);
                        case TypeCode.Int32:
                            return propertyDelegateItem.getInt32(*source);
                        case TypeCode.Int64:
                            return propertyDelegateItem.getInt64(*source);
                        case TypeCode.SByte:
                            return propertyDelegateItem.getSByte(*source);
                        case TypeCode.Single:
                            return propertyDelegateItem.getSingle(*source);
                        case TypeCode.UInt16:
                            return propertyDelegateItem.getUInt16(*source);
                        case TypeCode.UInt32:
                            return propertyDelegateItem.getUInt32(*source);
                        case TypeCode.UInt64:
                            return propertyDelegateItem.getUInt64(*source);
                        case TypeCode.Object:
                        default:
                            return propertyDelegateItem.getObject(*source);
                    }
                }
                else
                {
                    return propertyDelegateItem.getObject(*source);
                }
            }
            else
            {

                void* field = (byte*)*source + this.offset;

                if (this.isValueType)
                {
                    switch (this.typeCode)
                    {
                        case TypeCode.Boolean:
                            return *(bool*)field;
                        case TypeCode.Byte:
                            return *(Byte*)field;
                        case TypeCode.Char:
                            return *(Char*)field;
                        case TypeCode.DateTime:
                            return *(DateTime*)field;
                        //case TypeCode.DBNull:
                        //    return GeneralTool.VoidPtrToObject(field);
                        //case TypeCode.String:
                        //    return GeneralTool.VoidPtrToObject(field);
                        case TypeCode.Decimal:
                            return *(Decimal*)field;
                        case TypeCode.Double:
                            return *(Double*)field;
                        //case TypeCode.Empty:
                        case TypeCode.Int16:
                            return *(Int16*)field;
                        case TypeCode.Int32:
                            return *(Int32*)field;
                        case TypeCode.Int64:
                            return *(Int64*)field;
                        case TypeCode.SByte:
                            return *(Int64*)field;
                        case TypeCode.Single:
                            return *(Single*)field;
                        case TypeCode.UInt16:
                            return *(UInt16*)field;
                        case TypeCode.UInt32:
                            return *(UInt32*)field;
                        case TypeCode.UInt64:
                            return *(UInt64*)field;
                        case TypeCode.Object:
                        default:
                            object obj = new byte[this.heapSize - 3 * UnsafeOperation.PTR_COUNT];
                            IntPtr* ptr = (IntPtr*)GeneralTool.ObjectToVoidPtr(obj);
                            *ptr = typeHead;
                            ptr += 1;
                            GeneralTool.MemCpy(ptr, field, this.stackSize);
                            return obj;
                    }
                }
                else
                {
                    return GeneralTool.VoidPtrToObject(*(IntPtr**)field);
                }
            }
        }

    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MulticastDelegateValue
    {
        public IntPtr typePointer;

        public void* _target;

        public void* _methodBase;

        public IntPtr _methodPtr;

        public IntPtr _methodPtrAux;

        public IntPtr _invocationList;

        //object
        public void* _invocationCount;

        private IntPtr __alignment;
    }



}
