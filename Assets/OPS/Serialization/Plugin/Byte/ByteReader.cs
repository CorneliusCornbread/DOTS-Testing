using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OPS.Serialization.Byte
{
    internal class ByteReader
    {
        private Buffer m_buf;

        private const int maxStringLength = 65535;
        private const int maxByteLength = 2147483647;

        private const int k_InitialStringBufferSize = 1024;

        private static byte[] s_StringReaderBuffer;

        private static Encoding s_Encoding;

        private static UIntFloat s_FloatConverter;
        private static LongDouble s_DoubleConverter;

        /// <summary>
        ///   <para>The current position within the buffer.</para>
        /// </summary>
        public uint Position
        {
            get
            {
                return this.m_buf.Position;
            }
        }

        /// <summary>
        ///   <para>The current length of the buffer.</para>
        /// </summary>
        public int Length
        {
            get
            {
                return this.m_buf.Length;
            }
        }

        /// <summary>
        ///   <para>Creates a new ByteReader object.</para>
        /// </summary>
        /// <param name="buffer">A buffer to construct the reader with, this buffer is NOT copied.</param>
        public ByteReader()
        {
            this.m_buf = new Buffer();
            ByteReader.Initialize();
        }

        /// <summary>
        ///   <para>Creates a new ByteReader object.</para>
        /// </summary>
        /// <param name="buffer">A buffer to construct the reader with, this buffer is NOT copied.</param>
        public ByteReader(byte[] buffer)
        {
            this.m_buf = new Buffer(buffer);
            ByteReader.Initialize();
        }

        private static void Initialize()
        {
            if (ByteReader.s_Encoding == null)
            {
                ByteReader.s_StringReaderBuffer = new byte[k_InitialStringBufferSize];
                ByteReader.s_Encoding = new UTF8Encoding();
            }
        }

        /// <summary>
        ///   <para>Sets the current position of the reader's stream to the start of the stream.</para>
        /// </summary>
        public void SeekZero()
        {
            this.m_buf.SeekZero();
        }

        internal void Replace(byte[] buffer)
        {
            this.m_buf.Replace(buffer);
        }

        /// <summary>
        ///   <para>Reads a 32-bit variable-length-encoded value.</para>
        /// </summary>
        /// <returns>
        ///   <para>The 32 bit value read.</para>
        /// </returns>
        public uint ReadPackedUInt32()
        {
            byte b = this.ReadByte();
            if (b < 241)
            {
                return b;
            }
            byte b2 = this.ReadByte();
            if (b >= 241 && b <= 248)
            {
                return (uint)(240 + 256 * (b - 241) + b2);
            }
            byte b3 = this.ReadByte();
            if (b == 249)
            {
                return (uint)(2288 + 256 * b2 + b3);
            }
            byte b4 = this.ReadByte();
            if (b == 250)
            {
                return (uint)(b2 + (b3 << 8) + (b4 << 16));
            }
            byte b5 = this.ReadByte();
            if (b >= 251)
            {
                return (uint)(b2 + (b3 << 8) + (b4 << 16) + (b5 << 24));
            }
            throw new IndexOutOfRangeException("ReadPackedUInt32() failure: " + b);
        }

        /// <summary>
        ///   <para>Reads a 64-bit variable-length-encoded value.</para>
        /// </summary>
        /// <returns>
        ///   <para>The 64 bit value read.</para>
        /// </returns>
        public ulong ReadPackedUInt64()
        {
            byte b = this.ReadByte();
            if (b < 241)
            {
                return b;
            }
            byte b2 = this.ReadByte();
            if (b >= 241 && b <= 248)
            {
                return (ulong)(240 + 256 * ((long)b - 241L) + b2);
            }
            byte b3 = this.ReadByte();
            if (b == 249)
            {
                return (ulong)(2288 + 256L * (long)b2 + b3);
            }
            byte b4 = this.ReadByte();
            if (b == 250)
            {
                return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16);
            }
            byte b5 = this.ReadByte();
            if (b == 251)
            {
                return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24);
            }
            byte b6 = this.ReadByte();
            if (b == 252)
            {
                return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32);
            }
            byte b7 = this.ReadByte();
            if (b == 253)
            {
                return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40);
            }
            byte b8 = this.ReadByte();
            if (b == 254)
            {
                return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40) + ((ulong)b8 << 48);
            }
            byte b9 = this.ReadByte();
            if (b == 255)
            {
                return b2 + ((ulong)b3 << 8) + ((ulong)b4 << 16) + ((ulong)b5 << 24) + ((ulong)b6 << 32) + ((ulong)b7 << 40) + ((ulong)b8 << 48) + ((ulong)b9 << 56);
            }
            throw new IndexOutOfRangeException("ReadPackedUInt64() failure: " + b);
        }

        //
        public System.Object Read(Type _Type)
        {
            EPrimitiveType var_EType;
            if (PrimitiveTypeMatcher.TypeDictionary.TryGetValue(_Type, out var_EType))
            {
                switch (var_EType)
                {
                    case EPrimitiveType.BYTE:
                        {
                            return this.ReadByte();
                        }
                    case EPrimitiveType.BOOL:
                        {
                            return this.ReadBoolean();
                        }
                    case EPrimitiveType.INT16:
                        {
                            return this.ReadInt16();
                        }
                    case EPrimitiveType.INT32:
                        {
                            return this.ReadInt32();
                        }
                    case EPrimitiveType.INT64:
                        {
                            return this.ReadInt64();
                        }
                    case EPrimitiveType.UINT16:
                        {
                            return this.ReadUInt16();
                        }
                    case EPrimitiveType.UINT32:
                        {
                            return this.ReadUInt32();
                        }
                    case EPrimitiveType.UINT64:
                        {
                            return this.ReadUInt64();
                        }
                    case EPrimitiveType.FLOAT:
                        {
                            return this.ReadSingle();
                        }
                    case EPrimitiveType.DOUBLE:
                        {
                            return this.ReadDouble();
                        }
                    case EPrimitiveType.DECIMAL:
                        {
                            return this.ReadDecimal();
                        }
                    case EPrimitiveType.CHAR:
                        {
                            return this.ReadChar();
                        }
                    case EPrimitiveType.COLOR:
                        {
                            return this.ReadColor();
                        }
                    case EPrimitiveType.COLOR32:
                        {
                            return this.ReadColor32();
                        }
                    case EPrimitiveType.VECTOR2:
                        {
                            return this.ReadVector2();
                        }
                    case EPrimitiveType.VECTOR3:
                        {
                            return this.ReadVector3();
                        }
                    case EPrimitiveType.VECTOR4:
                        {
                            return this.ReadVector4();
                        }
                    case EPrimitiveType.QUATERNION:
                        {
                            return this.ReadQuaternion();
                        }
                    case EPrimitiveType.MATRIX4X4:
                        {
                            return this.ReadMatrix4x4();
                        }
                    case EPrimitiveType.PLANE:
                        {
                            return this.ReadPlane();
                        }
                    case EPrimitiveType.RAY:
                        {
                            return this.ReadRay();
                        }
                    case EPrimitiveType.RECT:
                        {
                            return this.ReadRect();
                        }
                    case EPrimitiveType.STRING:
                        {
                            return this.ReadString();
                        }
                }
            }
            if (_Type.IsValueType)
            {
                return Activator.CreateInstance(_Type);
            }
            return null;
        }

        /// <summary>
        ///   <para>Reads a byte from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>The value read.</para>
        /// </returns>
        public byte ReadByte()
        {
            return this.m_buf.ReadByte();
        }

        /// <summary>
        ///   <para>Reads a signed byte from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public sbyte ReadSByte()
        {
            return (sbyte)this.m_buf.ReadByte();
        }

        /// <summary>
        ///   <para>Reads a signed 16 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public short ReadInt16()
        {
            ushort num = 0;
            num = (ushort)(num | this.m_buf.ReadByte());
            num = (ushort)(num | (ushort)(this.m_buf.ReadByte() << 8));
            return (short)num;
        }

        /// <summary>
        ///   <para>Reads an unsigned 16 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public ushort ReadUInt16()
        {
            ushort num = 0;
            num = (ushort)(num | this.m_buf.ReadByte());
            return (ushort)(num | (ushort)(this.m_buf.ReadByte() << 8));
        }

        /// <summary>
        ///   <para>Reads a signed 32bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public int ReadInt32()
        {
            uint num = 0u;
            num |= this.m_buf.ReadByte();
            num = (uint)((int)num | this.m_buf.ReadByte() << 8);
            num = (uint)((int)num | this.m_buf.ReadByte() << 16);
            return (int)num | this.m_buf.ReadByte() << 24;
        }

        /// <summary>
        ///   <para>Reads an unsigned 32 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public uint ReadUInt32()
        {
            uint num = 0u;
            num |= this.m_buf.ReadByte();
            num = (uint)((int)num | this.m_buf.ReadByte() << 8);
            num = (uint)((int)num | this.m_buf.ReadByte() << 16);
            return (uint)((int)num | this.m_buf.ReadByte() << 24);
        }

        /// <summary>
        ///   <para>Reads a signed 64 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public long ReadInt64()
        {
            ulong num = 0uL;
            ulong num2 = this.m_buf.ReadByte();
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 8;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 16;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 24;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 32;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 40;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 48;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 56;
            return (long)(num | num2);
        }

        /// <summary>
        ///   <para>Reads an unsigned 64 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public ulong ReadUInt64()
        {
            ulong num = 0uL;
            ulong num2 = this.m_buf.ReadByte();
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 8;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 16;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 24;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 32;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 40;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 48;
            num |= num2;
            num2 = (ulong)this.m_buf.ReadByte() << 56;
            return num | num2;
        }

        /// <summary>
        ///   <para>Reads a decimal from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public decimal ReadDecimal()
        {
            return new decimal(new int[4]
            {
            this.ReadInt32(),
            this.ReadInt32(),
            this.ReadInt32(),
            this.ReadInt32()
            });
        }

        /// <summary>
        ///   <para>Reads a float from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public float ReadSingle()
        {
            s_FloatConverter.intValue = this.ReadUInt32();
            return s_FloatConverter.floatValue;
        }

        /// <summary>
        ///   <para>Reads a double from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public double ReadDouble()
        {
            s_DoubleConverter.longValue = this.ReadUInt64();
            return s_DoubleConverter.doubleValue;
        }

        /// <summary>
        ///   <para>Reads a string from the stream. (max of 32k bytes).</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public string ReadString()
        {
            int num = this.ReadInt32();
            if (num == -1)
            {
                return null;
            }
            if (num == 0)
            {
                return "";
            }
            if (num >= maxStringLength)
            {
                throw new IndexOutOfRangeException("ReadString() too long: " + num + "! Maximal length: " + maxStringLength);
            }
            while (num > ByteReader.s_StringReaderBuffer.Length)
            {
                ByteReader.s_StringReaderBuffer = new byte[ByteReader.s_StringReaderBuffer.Length * 2];
            }
            this.m_buf.ReadBytes(ByteReader.s_StringReaderBuffer, num);
            char[] chars = ByteReader.s_Encoding.GetChars(ByteReader.s_StringReaderBuffer, 0, num);
            return new string(chars);
        }

        /// <summary>
        ///   <para>Reads a char from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>Value read.</para>
        /// </returns>
        public char ReadChar()
        {
            return (char)this.m_buf.ReadByte();
        }

        /// <summary>
        ///   <para>Reads a boolean from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>The value read.</para>
        /// </returns>
        public bool ReadBoolean()
        {
            int num = this.m_buf.ReadByte();
            return num == 1;
        }

        /// <summary>
        ///   <para>Reads a number of bytes from the stream.</para>
        /// </summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>
        ///   <para>Bytes read. (this is a copy).</para>
        /// </returns>
        public byte[] ReadBytes(int count)
        {
            if (count < 0)
            {
                throw new IndexOutOfRangeException("ByteReader ReadBytes " + count);
            }
            byte[] array = new byte[count];
            this.m_buf.ReadBytes(array, (int)count);
            return array;
        }

        /// <summary>
        ///   <para>This read a 32-bit byte count and a array of bytes of that size from the stream.</para>
        /// </summary>
        /// <returns>
        ///   <para>The bytes read from the stream.</para>
        /// </returns>
        public byte[] ReadBytesAndSize()
        {
            int num = this.ReadInt32();
            if (num == 0)
            {
                return new byte[0];
            }
            return this.ReadBytes(num);
        }

        /// <summary>
        ///   <para>Reads a Unity Vector2 object.</para>
        /// </summary>
        /// <returns>
        ///   <para>The vector read from the stream.</para>
        /// </returns>
        public Vector2 ReadVector2()
        {
            return new Vector2(this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        ///   <para>Reads a Unity Vector3 objects.</para>
        /// </summary>
        /// <returns>
        ///   <para>The vector read from the stream.</para>
        /// </returns>
        public Vector3 ReadVector3()
        {
            return new Vector3(this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        ///   <para>Reads a Unity Vector4 object.</para>
        /// </summary>
        /// <returns>
        ///   <para>The vector read from the stream.</para>
        /// </returns>
        public Vector4 ReadVector4()
        {
            return new Vector4(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        ///   <para>Reads a unity Color objects.</para>
        /// </summary>
        /// <returns>
        ///   <para>The color read from the stream.</para>
        /// </returns>
        public Color ReadColor()
        {
            return new Color(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        ///   <para>Reads a unity color32 objects.</para>
        /// </summary>
        /// <returns>
        ///   <para>The colo read from the stream.</para>
        /// </returns>
        public Color32 ReadColor32()
        {
            return new Color32(this.ReadByte(), this.ReadByte(), this.ReadByte(), this.ReadByte());
        }

        /// <summary>
        ///   <para>Reads a Unity Quaternion object.</para>
        /// </summary>
        /// <returns>
        ///   <para>The quaternion read from the stream.</para>
        /// </returns>
        public Quaternion ReadQuaternion()
        {
            return new Quaternion(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        ///   <para>Reads a Unity Rect object.</para>
        /// </summary>
        /// <returns>
        ///   <para>The rect read from the stream.</para>
        /// </returns>
        public Rect ReadRect()
        {
            return new Rect(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        ///   <para>Reads a unity Plane object.</para>
        /// </summary>
        /// <returns>
        ///   <para>The plane read from the stream.</para>
        /// </returns>
        public Plane ReadPlane()
        {
            return new Plane(this.ReadVector3(), this.ReadSingle());
        }

        /// <summary>
        ///   <para>Reads a Unity Ray object.</para>
        /// </summary>
        /// <returns>
        ///   <para>The ray read from the stream.</para>
        /// </returns>
        public Ray ReadRay()
        {
            return new Ray(this.ReadVector3(), this.ReadVector3());
        }

        /// <summary>
        ///   <para>Reads a unity Matrix4x4 object.</para>
        /// </summary>
        /// <returns>
        ///   <para>The matrix read from the stream.</para>
        /// </returns>
        public Matrix4x4 ReadMatrix4x4()
        {
            Matrix4x4 result = default(Matrix4x4);
            result.m00 = this.ReadSingle();
            result.m01 = this.ReadSingle();
            result.m02 = this.ReadSingle();
            result.m03 = this.ReadSingle();
            result.m10 = this.ReadSingle();
            result.m11 = this.ReadSingle();
            result.m12 = this.ReadSingle();
            result.m13 = this.ReadSingle();
            result.m20 = this.ReadSingle();
            result.m21 = this.ReadSingle();
            result.m22 = this.ReadSingle();
            result.m23 = this.ReadSingle();
            result.m30 = this.ReadSingle();
            result.m31 = this.ReadSingle();
            result.m32 = this.ReadSingle();
            result.m33 = this.ReadSingle();
            return result;
        }
        
        /// <summary>
        ///   <para>Returns a string representation of the reader's buffer.</para>
        /// </summary>
        /// <returns>
        ///   <para>Buffer contents.</para>
        /// </returns>
        public override string ToString()
        {
            return this.m_buf.ToString();
        }

        //
        private object GetDefaultValue(Type _Type)
        {
            if (_Type.IsValueType)
                return Activator.CreateInstance(_Type);

            return null;
        }
    }
}