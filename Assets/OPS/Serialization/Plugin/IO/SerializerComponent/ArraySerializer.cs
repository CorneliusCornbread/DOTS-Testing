using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.IO
{
    internal class ArraySerializer : ISerializerComponent
    {
        private class ArrayIndexer
        {
            readonly int totalLength;
            readonly int lastIndexLength;
            readonly int[] lengths;
            readonly int[] lowerBounds;
            int current;
            readonly int[] currentZeroBased;

            public ArrayIndexer(int[] lengths, int[] lowerBounds)
            {
                lastIndexLength = lengths[lengths.Length - 1];
                totalLength = lengths[0];
                for (int i = 1; i < lengths.Length; i++)
                {
                    totalLength *= lengths[i];
                }
                this.lengths = lengths;
                this.lowerBounds = lowerBounds;
                currentZeroBased = new int[lengths.Length];
                current = -1;
            }

            public bool MoveNext()
            {
                current++;
                if (current != 0)
                {
                    int currLastIndex = current % lastIndexLength;
                    currentZeroBased[currentZeroBased.Length - 1] = currLastIndex;
                    if (currLastIndex == 0)
                    {
                        for (int i = currentZeroBased.Length - 2; i >= 0; i--)
                        {
                            currentZeroBased[i]++;
                            if (currentZeroBased[i] != lengths[i])
                                break;
                            currentZeroBased[i] = 0;
                        }
                    }
                }
                return current < totalLength;
            }

            public int[] Current
            {
                get
                {
                    int[] result = new int[currentZeroBased.Length];
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = currentZeroBased[i] + lowerBounds[i];
                    }
                    return result;
                }
            }
        }


        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (!_Type.IsArray)
            {
                _Bytes = null;
                return false;
            }

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

            Array var_Array = _Object as Array;
            
            Type var_ElementType = _Type.GetElementType();
            
            //Write dimensions!
            var_Writer.Write((int)var_Array.Rank);
            //Go through each dimension and write the length!
            int[] var_ArrayLength = new int[var_Array.Rank];
            for(int d = 0; d < var_Array.Rank; d++)
            {
                var_ArrayLength[d] = (int)var_Array.GetLength(d);
                var_Writer.Write((int)var_Array.GetLength(d));
            }
            //Go through each dimension and write the lowerbound!
            int[] var_ArrayLowerBound = new int[var_Array.Rank];
            for (int d = 0; d < var_Array.Rank; d++)
            {
                var_ArrayLowerBound[d] = (int)var_Array.GetLowerBound(d);
                var_Writer.Write((int)var_Array.GetLowerBound(d));
            }

            ArrayIndexer arrayIndexer = new ArrayIndexer(var_ArrayLength, var_ArrayLowerBound);

            if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(var_ElementType))
            {
                while (arrayIndexer.MoveNext())
                {
                    var_Writer.Write(var_ElementType, var_Array.GetValue(arrayIndexer.Current));
                }
            }
            else
            {
                while (arrayIndexer.MoveNext())
                {
                    byte[] var_Bytes = Serializer.Internal_Serialize(var_ElementType, var_Array.GetValue(arrayIndexer.Current));
                    var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
                }
            }

            _Bytes = var_Writer.ToArray();
            return true;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if(!_Type.IsArray)
            {
                _Object = null;
                return false;
            }

            ByteReader var_Reader = new ByteReader(_Bytes);

            bool var_IsNull = var_Reader.ReadBoolean();
            if (var_IsNull)
            {
                _Object = null;
                return true;
            }

            //

            Type var_ElementType = _Type.GetElementType();

            //Read dimensions
            int var_Dimensions = var_Reader.ReadInt32();

            //Go through each dimension and read the length!
            int[] var_ArrayLength = new int[var_Dimensions];
            for (int d = 0; d < var_Dimensions; d++)
            {
                var_ArrayLength[d] = var_Reader.ReadInt32();
            }

            //Go through each dimension and read the lowerbound!
            int[] var_ArrayLowerBound = new int[var_Dimensions];
            for (int d = 0; d < var_Dimensions; d++)
            {
                var_ArrayLowerBound[d] = var_Reader.ReadInt32();
            }
            
            var var_Array = Array.CreateInstance(var_ElementType, var_ArrayLength);

            ArrayIndexer arrayIndexer = new ArrayIndexer(var_ArrayLength, var_ArrayLowerBound);

            if (PrimitiveTypeMatcher.TypeDictionary.ContainsKey(var_ElementType))
            {
                while (arrayIndexer.MoveNext())
                {
                    var_Array.SetValue(var_Reader.Read(var_ElementType), arrayIndexer.Current);
                }
            }
            else
            {
                while (arrayIndexer.MoveNext())
                {
                    byte[] var_Bytes = var_Reader.ReadBytesAndSize();
                    Object var_Item = Serializer.DeSerialize(_Type.GetElementType(), var_Bytes);
                    var_Array.SetValue(var_Item, arrayIndexer.Current);
                }
            }

            _Object = var_Array;
            return true;
        }
    }
}
