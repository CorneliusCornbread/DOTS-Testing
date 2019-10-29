using OPS.Serialization.Byte;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.IO
{
    internal class QueueSerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (_Type.IsGenericType && _Type.GetGenericTypeDefinition() == typeof(Queue<>))
            {
                ByteWriter var_Writer = new ByteWriter();

                if (_Object == null)
                {
                    var_Writer.Write((bool)true);
                    _Bytes = var_Writer.ToArray();
                    return true;
                }
                else
                {
                    var_Writer.Write((bool)false);
                }

                //
                Type var_GenericItemType = _Type.GetGenericArguments()[0];

                IEnumerable var_Enumerable = _Object as IEnumerable;

                int var_ItemCount = 0;
                ByteWriter var_ItemWriter = new ByteWriter();

                if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(var_GenericItemType))
                {
                    IEnumerator var_Enumerator = var_Enumerable.GetEnumerator();
                    while (var_Enumerator.MoveNext())
                    {
                        Object var_Current = var_Enumerator.Current;

                        var_ItemWriter.Write(var_GenericItemType, var_Current);

                        var_ItemCount += 1;
                    }
                }
                else
                {
                    IEnumerator var_Enumerator = var_Enumerable.GetEnumerator();
                    while (var_Enumerator.MoveNext())
                    {
                        Object var_Current = var_Enumerator.Current;

                        byte[] var_Bytes = Serializer.Internal_Serialize(var_GenericItemType, var_Current);
                        var_ItemWriter.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

                        var_ItemCount += 1;
                    }
                }

                var_Writer.Write((UInt16)var_ItemCount);

                byte[] var_ItemBytes = var_ItemWriter.ToArray();
                var_Writer.WriteBytesAndSize(var_ItemBytes, var_ItemBytes.Length);

                _Bytes = var_Writer.ToArray();
                return true;
            }

            _Bytes = null;
            return false;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if(_Type.IsGenericType && _Type.GetGenericTypeDefinition() == typeof(Queue<>))
            {
                Type var_QueueType = typeof(Queue<>);
                Type var_GenericItemType = _Type.GetGenericArguments()[0];
                Type var_GenericQueueType = var_QueueType.MakeGenericType(var_GenericItemType);

                ByteReader var_Reader = new ByteReader(_Bytes);

                bool var_IsNull = var_Reader.ReadBoolean();
                if (var_IsNull)
                {
                    _Object = null;
                    return true;
                }

                //

                int var_ItemCount = var_Reader.ReadUInt16();

                byte[] var_ItemBytes = var_Reader.ReadBytesAndSize();

                ByteReader var_ItemReader = new ByteReader(var_ItemBytes);

                Type var_ListType = typeof(List<>);
                Type var_GenericListType = var_ListType.MakeGenericType(var_GenericItemType);

                IList var_Items = Activator.CreateInstance(var_GenericListType) as IList;

                if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(var_GenericItemType))
                {
                    for (int i = 0; i < var_ItemCount; i++)
                    {
                        var_Items.Add(var_ItemReader.Read(var_GenericItemType));
                    }
                }
                else
                {
                    for (int i = 0; i < var_ItemCount; i++)
                    {
                        byte[] var_Bytes = var_ItemReader.ReadBytesAndSize();
                        Object var_Item = Serializer.DeSerialize(var_GenericItemType, var_Bytes);
                        var_Items.Add(var_Item);
                    }
                }

                _Object = Activator.CreateInstance(var_GenericQueueType, var_Items);
                return true;
            }

            _Object = null;
            return false;
        }
    }
}
