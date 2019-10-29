using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OPS.Serialization.Byte
{
    internal class ByteWriter
    {
        private const int maxStringLength = 65535;
        private const int maxByteLength = 2147483647;

        private Buffer m_Buffer;

        private static Encoding s_Encoding;

        private static byte[] s_StringWriteBuffer;

        private static UIntFloat s_FloatConverter;
        private static LongDouble s_DoubleConverter;

        /// <summary>
        ///   <para>The current position of the internal buffer.</para>
        /// </summary>
        public uint Position
        {
            get
            {
                return this.m_Buffer.Position;
            }
        }

        /// <summary>
        ///   <para>Creates a new ByteWriter object.</para>
        /// </summary>
        /// <param name="buffer">A buffer to write into. This is not copied.</param>
        public ByteWriter()
        {
            this.m_Buffer = new Buffer();
            if (ByteWriter.s_Encoding == null)
            {
                ByteWriter.s_Encoding = new UTF8Encoding();
                ByteWriter.s_StringWriteBuffer = new byte[maxStringLength];
            }
        }

        /// <summary>
        ///   <para>Creates a new ByteWriter object.</para>
        /// </summary>
        /// <param name="buffer">A buffer to write into. This is not copied.</param>
        public ByteWriter(byte[] buffer)
        {
            this.m_Buffer = new Buffer(buffer);
            if (ByteWriter.s_Encoding == null)
            {
                ByteWriter.s_Encoding = new UTF8Encoding();
                ByteWriter.s_StringWriteBuffer = new byte[maxStringLength];
            }
        }

        /// <summary>
        ///   <para>Returns a copy of internal array of bytes the writer is using, it copies only the bytes used.</para>
        /// </summary>
        /// <returns>
        ///   <para>Copy of data used by the writer.</para>
        /// </returns>
        public byte[] ToArray()
        {
            byte[] array = new byte[this.m_Buffer.AsArraySegment().Count];
            Array.Copy(this.m_Buffer.AsArraySegment().Array, array, this.m_Buffer.AsArraySegment().Count);
            return array;
        }

        /// <summary>
        ///   <para>Returns the internal array of bytes the writer is using. This is NOT a copy.</para>
        /// </summary>
        /// <returns>
        ///   <para>Internal buffer.</para>
        /// </returns>
        public byte[] AsArray()
        {
            return this.AsArraySegment().Array;
        }

        internal ArraySegment<byte> AsArraySegment()
        {
            return this.m_Buffer.AsArraySegment();
        }

        /// <summary>
        ///   <para>This writes the 32-bit value to the stream using variable-length-encoding.</para>
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WritePackedUInt32(uint value)
        {
            if (value <= 240)
            {
                this.Write((byte)value);
            }
            else if (value <= 2287)
            {
                this.Write((byte)((value - 240) / 256u + 241));
                this.Write((byte)((value - 240) % 256u));
            }
            else if (value <= 67823)
            {
                this.Write((byte)249);
                this.Write((byte)((value - 2288) / 256u));
                this.Write((byte)((value - 2288) % 256u));
            }
            else if (value <= 16777215)
            {
                this.Write((byte)250);
                this.Write((byte)(value & 0xFF));
                this.Write((byte)(value >> 8 & 0xFF));
                this.Write((byte)(value >> 16 & 0xFF));
            }
            else
            {
                this.Write((byte)251);
                this.Write((byte)(value & 0xFF));
                this.Write((byte)(value >> 8 & 0xFF));
                this.Write((byte)(value >> 16 & 0xFF));
                this.Write((byte)(value >> 24 & 0xFF));
            }
        }

        /// <summary>
        ///   <para>This writes the 64-bit value to the stream using variable-length-encoding.</para>
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WritePackedUInt64(ulong value)
        {
            if (value <= 240)
            {
                this.Write((byte)value);
            }
            else if (value <= 2287)
            {
                this.Write((byte)((value - 240) / 256uL + 241));
                this.Write((byte)((value - 240) % 256uL));
            }
            else if (value <= 67823)
            {
                this.Write((byte)249);
                this.Write((byte)((value - 2288) / 256uL));
                this.Write((byte)((value - 2288) % 256uL));
            }
            else if (value <= 16777215)
            {
                this.Write((byte)250);
                this.Write((byte)(value & 0xFF));
                this.Write((byte)(value >> 8 & 0xFF));
                this.Write((byte)(value >> 16 & 0xFF));
            }
            else if (value <= 4294967295u)
            {
                this.Write((byte)251);
                this.Write((byte)(value & 0xFF));
                this.Write((byte)(value >> 8 & 0xFF));
                this.Write((byte)(value >> 16 & 0xFF));
                this.Write((byte)(value >> 24 & 0xFF));
            }
            else if (value <= 1099511627775L)
            {
                this.Write((byte)252);
                this.Write((byte)(value & 0xFF));
                this.Write((byte)(value >> 8 & 0xFF));
                this.Write((byte)(value >> 16 & 0xFF));
                this.Write((byte)(value >> 24 & 0xFF));
                this.Write((byte)(value >> 32 & 0xFF));
            }
            else if (value <= 281474976710655L)
            {
                this.Write((byte)253);
                this.Write((byte)(value & 0xFF));
                this.Write((byte)(value >> 8 & 0xFF));
                this.Write((byte)(value >> 16 & 0xFF));
                this.Write((byte)(value >> 24 & 0xFF));
                this.Write((byte)(value >> 32 & 0xFF));
                this.Write((byte)(value >> 40 & 0xFF));
            }
            else if (value <= 72057594037927935L)
            {
                this.Write((byte)254);
                this.Write((byte)(value & 0xFF));
                this.Write((byte)(value >> 8 & 0xFF));
                this.Write((byte)(value >> 16 & 0xFF));
                this.Write((byte)(value >> 24 & 0xFF));
                this.Write((byte)(value >> 32 & 0xFF));
                this.Write((byte)(value >> 40 & 0xFF));
                this.Write((byte)(value >> 48 & 0xFF));
            }
            else
            {
                this.Write((byte)255);
                this.Write((byte)(value & 0xFF));
                this.Write((byte)(value >> 8 & 0xFF));
                this.Write((byte)(value >> 16 & 0xFF));
                this.Write((byte)(value >> 24 & 0xFF));
                this.Write((byte)(value >> 32 & 0xFF));
                this.Write((byte)(value >> 40 & 0xFF));
                this.Write((byte)(value >> 48 & 0xFF));
                this.Write((byte)(value >> 56 & 0xFF));
            }
        }

        //
        public void Write(Type _Type, System.Object _Value)
        {
            EPrimitiveType var_EType;
            if(PrimitiveTypeMatcher.TypeDictionary.TryGetValue(_Type, out var_EType))
            {
                switch(var_EType)
                {
                    case EPrimitiveType.BYTE:
                        {
                            this.Write((byte)_Value);
                            break;
                        }
                    case EPrimitiveType.BOOL:
                        {
                            this.Write((bool)_Value);
                            break;
                        }
                    case EPrimitiveType.INT16:
                        {
                            this.Write((Int16)_Value);
                            break;
                        }
                    case EPrimitiveType.INT32:
                        {
                            this.Write((Int32)_Value);
                            break;
                        }
                    case EPrimitiveType.INT64:
                        {
                            this.Write((Int64)_Value);
                            break;
                        }
                    case EPrimitiveType.UINT16:
                        {
                            this.Write((UInt16)_Value);
                            break;
                        }
                    case EPrimitiveType.UINT32:
                        {
                            this.Write((UInt32)_Value);
                            break;
                        }
                    case EPrimitiveType.UINT64:
                        {
                            this.Write((UInt64)_Value);
                            break;
                        }
                    case EPrimitiveType.FLOAT:
                        {
                            this.Write((float)_Value);
                            break;
                        }
                    case EPrimitiveType.DOUBLE:
                        {
                            this.Write((double)_Value);
                            break;
                        }
                    case EPrimitiveType.DECIMAL:
                        {
                            this.Write((decimal)_Value);
                            break;
                        }
                    case EPrimitiveType.CHAR:
                        {
                            this.Write((char)_Value);
                            break;
                        }
                    case EPrimitiveType.COLOR:
                        {
                            this.Write((Color)_Value);
                            break;
                        }
                    case EPrimitiveType.COLOR32:
                        {
                            this.Write((Color32)_Value);
                            break;
                        }
                    case EPrimitiveType.VECTOR2:
                        {
                            this.Write((Vector2)_Value);
                            break;
                        }
                    case EPrimitiveType.VECTOR3:
                        {
                            this.Write((Vector3)_Value);
                            break;
                        }
                    case EPrimitiveType.VECTOR4:
                        {
                            this.Write((Vector4)_Value);
                            break;
                        }
                    case EPrimitiveType.QUATERNION:
                        {
                            this.Write((Quaternion)_Value);
                            break;
                        }
                    case EPrimitiveType.MATRIX4X4:
                        {
                            this.Write((Matrix4x4)_Value);
                            break;
                        }
                    case EPrimitiveType.PLANE:
                        {
                            this.Write((Plane)_Value);
                            break;
                        }
                    case EPrimitiveType.RAY:
                        {
                            this.Write((Ray)_Value);
                            break;
                        }
                    case EPrimitiveType.RECT:
                        {
                            this.Write((Rect)_Value);
                            break;
                        }
                    case EPrimitiveType.STRING:
                        {
                            this.Write((String)_Value);
                            break;
                        }
                }
            }
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(char value)
        {
            this.m_Buffer.WriteByte((byte)value);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(byte value)
        {
            this.m_Buffer.WriteByte(value);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(sbyte value)
        {
            this.m_Buffer.WriteByte((byte)value);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(short value)
        {
            this.m_Buffer.WriteByte2((byte)(value & 0xFF), (byte)(value >> 8 & 0xFF));
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(ushort value)
        {
            this.m_Buffer.WriteByte2((byte)(value & 0xFF), (byte)(value >> 8 & 0xFF));
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(int value)
        {
            this.m_Buffer.WriteByte4((byte)(value & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 24 & 0xFF));
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(uint value)
        {
            this.m_Buffer.WriteByte4((byte)(value & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 24 & 0xFF));
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(long value)
        {
            this.m_Buffer.WriteByte8((byte)(value & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 24 & 0xFF), (byte)(value >> 32 & 0xFF), (byte)(value >> 40 & 0xFF), (byte)(value >> 48 & 0xFF), (byte)(value >> 56 & 0xFF));
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(ulong value)
        {
            this.m_Buffer.WriteByte8((byte)(value & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 24 & 0xFF), (byte)(value >> 32 & 0xFF), (byte)(value >> 40 & 0xFF), (byte)(value >> 48 & 0xFF), (byte)(value >> 56 & 0xFF));
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(float value)
        {
            ByteWriter.s_FloatConverter.floatValue = value;
            this.Write(ByteWriter.s_FloatConverter.intValue);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(double value)
        {
            ByteWriter.s_DoubleConverter.doubleValue = value;
            this.Write(ByteWriter.s_DoubleConverter.longValue);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(decimal value)
        {
            int[] bits = decimal.GetBits(value);
            this.Write(bits[0]);
            this.Write(bits[1]);
            this.Write(bits[2]);
            this.Write(bits[3]);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(string value)
        {
            if (value == null)
            {
                this.Write((int)-1);
            }
            else
            {
                int byteCount = ByteWriter.s_Encoding.GetByteCount(value);
                if (byteCount >= maxStringLength)
                {
                    throw new IndexOutOfRangeException("Serialize(string) too long: " + value.Length + "! Maximal length: " + maxStringLength);
                }
                this.Write((int)byteCount);
                int bytes = ByteWriter.s_Encoding.GetBytes(value, 0, value.Length, ByteWriter.s_StringWriteBuffer, 0);
                this.m_Buffer.WriteBytes(ByteWriter.s_StringWriteBuffer, (int)bytes);
            }
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(bool value)
        {
            if (value)
            {
                this.m_Buffer.WriteByte(1);
            }
            else
            {
                this.m_Buffer.WriteByte(0);
            }
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(byte[] buffer, int count)
        {
            if (count > maxByteLength)
            {
                Debug.LogError("ByteWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 2000M bytes.");
            }
            else
            {
                this.m_Buffer.WriteBytes(buffer, (int)count);
            }
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (count > maxByteLength)
            {
                Debug.LogError("ByteWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 2000M bytes.");
            }
            else
            {
                this.m_Buffer.WriteBytesAtOffset(buffer, (int)offset, (int)count);
            }
        }

        /// <summary>
        ///   <para>This writes a 32-bit count and a array of bytes of that length to the stream.</para>
        /// </summary>
        /// <param name="buffer">Array of bytes to write.</param>
        /// <param name="count">Number of bytes from the array to write.</param>
        public void WriteBytesAndSize(byte[] buffer, int count)
        {
            if (buffer == null || count == 0)
            {
                this.Write((int)0);
            }
            else if (count > maxByteLength)
            {
                Debug.LogError("ByteWriter WriteBytesAndSize: buffer is too large (" + count + ") bytes. The maximum buffer size is 2000M bytes.");
            }
            else
            {
                this.Write((int)count);
                this.m_Buffer.WriteBytes(buffer, (int)count);
            }
        }

        /// <summary>
        ///   <para>This writes a 32-bit count and an array of bytes of that size to the stream.</para>
        /// </summary>
        /// <param name="buffer">Bytes to write.</param>
        public void WriteBytesFull(byte[] buffer)
        {
            if (buffer == null)
            {
                this.Write((int)0);
            }
            else if (buffer.Length > maxByteLength)
            {
                Debug.LogError("ByteWriter WriteBytes: buffer is too large (" + buffer.Length + ") bytes. The maximum buffer size is 2000M bytes.");
            }
            else
            {
                this.Write((int)buffer.Length);
                this.m_Buffer.WriteBytes(buffer, (int)buffer.Length);
            }
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Vector2 value)
        {
            this.Write(value.x);
            this.Write(value.y);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Vector3 value)
        {
            this.Write(value.x);
            this.Write(value.y);
            this.Write(value.z);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Vector4 value)
        {
            this.Write(value.x);
            this.Write(value.y);
            this.Write(value.z);
            this.Write(value.w);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Color value)
        {
            this.Write(value.r);
            this.Write(value.g);
            this.Write(value.b);
            this.Write(value.a);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Color32 value)
        {
            this.Write(value.r);
            this.Write(value.g);
            this.Write(value.b);
            this.Write(value.a);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Quaternion value)
        {
            this.Write(value.x);
            this.Write(value.y);
            this.Write(value.z);
            this.Write(value.w);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Rect value)
        {
            this.Write(value.xMin);
            this.Write(value.yMin);
            this.Write(value.width);
            this.Write(value.height);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Plane value)
        {
            this.Write(value.normal);
            this.Write(value.distance);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Ray value)
        {
            this.Write(value.direction);
            this.Write(value.origin);
        }

        /// <summary>
        ///   <para>This writes a reference to an object, value, buffer or network message, to the stream.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(Matrix4x4 value)
        {
            this.Write(value.m00);
            this.Write(value.m01);
            this.Write(value.m02);
            this.Write(value.m03);
            this.Write(value.m10);
            this.Write(value.m11);
            this.Write(value.m12);
            this.Write(value.m13);
            this.Write(value.m20);
            this.Write(value.m21);
            this.Write(value.m22);
            this.Write(value.m23);
            this.Write(value.m30);
            this.Write(value.m31);
            this.Write(value.m32);
            this.Write(value.m33);
        }
    }
}