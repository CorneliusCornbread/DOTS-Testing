using OPS.Serialization.Attributes;
using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OPS.Serialization.IO
{
    internal class ClassSerializer : ISerializerComponent
    {
        private static bool HasSerializeClass(Type _Type)
        {
            var var_Attributes = _Type.GetCustomAttributes(false);

            foreach (var var_Attribute in var_Attributes)
            {
                if (var_Attribute is SerializeAbleClassAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetInheritanceIndex(Type _Type, Type _InheritanceType)
        {
            var var_Attributes = _Type.GetCustomAttributes(false);

            foreach (var var_Attribute in var_Attributes)
            {
                if (var_Attribute is ClassInheritanceAttribute)
                {
                    ClassInheritanceAttribute var_ClassInheritanceAttribute = var_Attribute as ClassInheritanceAttribute;
                    if(var_ClassInheritanceAttribute.InheritanceType == _InheritanceType)
                    {
                        return (var_Attribute as ClassInheritanceAttribute).Index;
                    }
                }
            }

            return -1;
        }

        private static Type GetInheritanceType(Type _Type, int _InheritanceIndex)
        {
            var var_Attributes = _Type.GetCustomAttributes(false);

            foreach (var var_Attribute in var_Attributes)
            {
                if (var_Attribute is ClassInheritanceAttribute)
                {
                    ClassInheritanceAttribute var_ClassInheritanceAttribute = var_Attribute as ClassInheritanceAttribute;
                    if (var_ClassInheritanceAttribute.Index == _InheritanceIndex)
                    {
                        return (var_Attribute as ClassInheritanceAttribute).InheritanceType;
                    }
                }
            }

            return null;
        }

        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (!HasSerializeClass(_Type))
            {
                _Bytes = null;
                return false;
            }

            ByteWriter var_Writer = new ByteWriter();

            if (_Object == null)
            {
                var_Writer.Write(true);
                _Bytes = var_Writer.ToArray();
                return true;
            }
            else
            {
                var_Writer.Write(false);
            }

            //Use Object Type!

            _Type = _Object.GetType();

            //Got through bases!

            List<Type> var_SerializeAbleBaseList = new List<Type>();
            List<int> var_InheritanceIndexList = new List<int>();

            Type var_InheritanceType = _Type;
            while(_Type != null)
            {
                if (HasSerializeClass(_Type))
                {
                    var_SerializeAbleBaseList.Insert(0, _Type);
                    //Check is not start type.
                    if(_Type != var_InheritanceType)
                    {
                        int var_InheritanceIndex = GetInheritanceIndex(_Type, var_InheritanceType);

                        if (var_InheritanceIndex == -1)
                        {
                            throw new Exception("Base class: " + _Type.ToString() + " has not set an ClassInheritanceAttribute for: " + var_InheritanceType.ToString());
                        }

                        var_InheritanceIndexList.Add(var_InheritanceIndex);

                        var_InheritanceType = _Type;
                    }
                }
                _Type = _Type.BaseType;
            }

            //Write inheritance indexes

            var_Writer.Write((byte)var_InheritanceIndexList.Count);

            for(int i = 0; i < var_InheritanceIndexList.Count; i++)
            {
                var_Writer.Write((byte)var_InheritanceIndexList[i]);
            }

            //Start with first type.

            this.InternSerialize(_Object, var_Writer, var_SerializeAbleBaseList, 0);

            _Bytes = var_Writer.ToArray();
            return true;
        }

        private void InternSerialize(Object _Object, ByteWriter _Writer, List<Type> _SerializeAbleBaseList, int _CurrentIndex)
        {
            Type var_CurrentType = _SerializeAbleBaseList[_CurrentIndex];

            //
            FieldInfo[] var_FoundFields;
            int[] var_FoundFieldsId;
            bool[] var_FoundFieldsOptional;

            FieldContext.LoadFields(var_CurrentType, out var_FoundFields, out var_FoundFieldsId, out var_FoundFieldsOptional);

            _Writer.Write((byte)var_FoundFields.Length);

            for (int i = 0; i < var_FoundFields.Length; i++)
            {
                //
                _Writer.Write((byte)var_FoundFieldsId[i]);

                //
                var var_FieldType = var_FoundFields[i].FieldType;

                //
                var var_Objectvalue = var_FoundFields[i].GetValue(_Object);

                if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(var_FieldType))
                {
                    _Writer.Write(var_FieldType, var_Objectvalue);
                }
                else
                {
                    byte[] var_Bytes = Serializer.Internal_Serialize(var_FieldType, var_Objectvalue);
                    _Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
                }
            }

            if(_CurrentIndex == _SerializeAbleBaseList.Count - 1)
            {
                return;
            }

            this.InternSerialize(_Object, _Writer, _SerializeAbleBaseList, _CurrentIndex + 1);
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if (!HasSerializeClass(_Type))
            {
                _Object = null;
                return false;
            }

            ByteReader var_Reader = new ByteReader(_Bytes);

            bool var_IsNull = var_Reader.ReadBoolean();
            if(var_IsNull)
            {
                _Object = null;
                return true;
            }

            //Go earliest base!
            Type var_LatestSerializeAbleType = null;

            List<Type> var_SerializeAbleBaseList = new List<Type>();

            //Got to latest base that has SerializeAbleClassAttribute!
            while (_Type != null)
            {
                if (HasSerializeClass(_Type))
                {
                    var_LatestSerializeAbleType = _Type;
                }
                _Type = _Type.BaseType;
            }

            if(var_LatestSerializeAbleType == null)
            {
                throw new Exception("Cannot deserialize " + _Type.ToString() + " because the Type has not SerializeAbleClassAttribute!");
            }

            //Add last base!
            var_SerializeAbleBaseList.Add(var_LatestSerializeAbleType);

            //Get count of inheritances!
            int var_InheritanceCount = var_Reader.ReadByte();

            //The last inherited type!
            Type var_EarliestSerializeAbleType = var_LatestSerializeAbleType;

            //Iterate upwards to latest class!
            while (var_InheritanceCount != 0)
            {
                int var_InheritanceIndex = var_Reader.ReadByte();

                var_EarliestSerializeAbleType = GetInheritanceType(var_EarliestSerializeAbleType, var_InheritanceIndex);

                var_SerializeAbleBaseList.Add(var_EarliestSerializeAbleType);

                var_InheritanceCount -= 1;
            }

            //
            _Object = Activator.CreateInstance(var_EarliestSerializeAbleType);

            this.InternDeSerialize(_Object, var_Reader, var_SerializeAbleBaseList, 0);

            return true;
        }

        private void InternDeSerialize(Object _Object, ByteReader _Reader, List<Type> _SerializeAbleBaseList, int _CurrentIndex)
        {
            Type var_CurrentType = _SerializeAbleBaseList[_CurrentIndex];

            Dictionary<int, FieldInfo> var_FoundFieldDictionary = new Dictionary<int, FieldInfo>();

            FieldInfo[] var_FoundFields;
            int[] var_FoundFieldsId;
            bool[] var_FoundFieldsOptional;

            FieldContext.LoadFields(var_CurrentType, out var_FoundFields, out var_FoundFieldsId, out var_FoundFieldsOptional);

            for (int i = 0; i < var_FoundFields.Length; i++)
            {
                var_FoundFieldDictionary.Add(var_FoundFieldsId[i], var_FoundFields[i]);
            }

            int var_FieldLength = _Reader.ReadByte();

            for (int i = 0; i < var_FieldLength; i++)
            {
                int var_FieldId = _Reader.ReadByte();

                FieldInfo var_FieldInfo;
                if (var_FoundFieldDictionary.TryGetValue(var_FieldId, out var_FieldInfo))
                {
                    var var_FieldType = var_FieldInfo.FieldType;

                    //
                    if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(var_FieldType))
                    {
                        var_FieldInfo.SetValue(_Object, _Reader.Read(var_FieldType));
                    }
                    else
                    {
                        byte[] var_Bytes = _Reader.ReadBytesAndSize();
                        Object var_Value = Serializer.DeSerialize(var_FieldType, var_Bytes);
                        var_FieldInfo.SetValue(_Object, var_Value);
                    }

                    //
                    var_FoundFieldDictionary.Remove(var_FieldId);
                }
                else
                {
                    Console.WriteLine(var_FieldId + " not found!");
                }
            }

            foreach (var var_Pair in var_FoundFieldDictionary)
            {
                if (var_FoundFieldsOptional[var_Pair.Key])
                {
                    Console.WriteLine(var_Pair.Key + " is optional!");
                }
                else
                {
                    Console.WriteLine(var_Pair.Key + " is not optional and not set!");
                }
            }

            //Inheritance
            if (_Reader.Position != _Reader.Length)
            {
                this.InternDeSerialize(_Object, _Reader, _SerializeAbleBaseList, _CurrentIndex + 1);
            }
        }
    }
}
