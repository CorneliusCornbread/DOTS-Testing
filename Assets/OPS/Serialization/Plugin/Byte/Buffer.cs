using System;
using UnityEngine;

namespace OPS.Serialization.Byte
{
    internal class Buffer
    {
        private byte[] m_Buffer;

        private uint m_Pos;

        private const int k_InitialSize = 64;

        private const float k_GrowthFactor = 1.5f;

        private const int k_BufferSizeWarning = 134217728;

        public uint Position
        {
            get
            {
                return this.m_Pos;
            }
        }

        public int Length
        {
            get
            {
                return this.m_Buffer.Length;
            }
        }

        public Buffer()
        {
            this.m_Buffer = new byte[64];
        }

        public Buffer(byte[] buffer)
        {
            this.m_Buffer = buffer;
        }

        public byte ReadByte()
        {
            if (this.m_Pos >= this.m_Buffer.Length)
            {
                throw new IndexOutOfRangeException("ByteReader:ReadByte out of range:" + this.ToString());
            }
            return this.m_Buffer[this.m_Pos++];
        }

        public void ReadBytes(byte[] buffer, int count)
        {
            if (this.m_Pos + count > this.m_Buffer.Length)
            {
                throw new IndexOutOfRangeException("ByteReader:ReadBytes out of range: (" + count + ") " + this.ToString());
            }
            for (int num = 0; num < count; num = (int)(num + 1))
            {
                buffer[num] = this.m_Buffer[this.m_Pos + num];
            }
            this.m_Pos += (uint)count;
        }

        internal ArraySegment<byte> AsArraySegment()
        {
            return new ArraySegment<byte>(this.m_Buffer, 0, (int)this.m_Pos);
        }

        public void WriteByte(byte value)
        {
            this.WriteCheckForSpace(1);
            this.m_Buffer[this.m_Pos] = value;
            this.m_Pos += 1u;
        }

        public void WriteByte2(byte value0, byte value1)
        {
            this.WriteCheckForSpace(2);
            this.m_Buffer[this.m_Pos] = value0;
            this.m_Buffer[this.m_Pos + 1] = value1;
            this.m_Pos += 2u;
        }

        public void WriteByte4(byte value0, byte value1, byte value2, byte value3)
        {
            this.WriteCheckForSpace(4);
            this.m_Buffer[this.m_Pos] = value0;
            this.m_Buffer[this.m_Pos + 1] = value1;
            this.m_Buffer[this.m_Pos + 2] = value2;
            this.m_Buffer[this.m_Pos + 3] = value3;
            this.m_Pos += 4u;
        }

        public void WriteByte8(byte value0, byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7)
        {
            this.WriteCheckForSpace(8);
            this.m_Buffer[this.m_Pos] = value0;
            this.m_Buffer[this.m_Pos + 1] = value1;
            this.m_Buffer[this.m_Pos + 2] = value2;
            this.m_Buffer[this.m_Pos + 3] = value3;
            this.m_Buffer[this.m_Pos + 4] = value4;
            this.m_Buffer[this.m_Pos + 5] = value5;
            this.m_Buffer[this.m_Pos + 6] = value6;
            this.m_Buffer[this.m_Pos + 7] = value7;
            this.m_Pos += 8u;
        }

        public void WriteBytesAtOffset(byte[] buffer, int targetOffset, int count)
        {
            uint num = (uint)(count + targetOffset);
            this.WriteCheckForSpace((int)num);
            if (targetOffset == 0 && count == buffer.Length)
            {
                buffer.CopyTo(this.m_Buffer, (int)this.m_Pos);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    this.m_Buffer[targetOffset + i] = buffer[i];
                }
            }
            if (num > this.m_Pos)
            {
                this.m_Pos = num;
            }
        }

        public void WriteBytes(byte[] buffer, int count)
        {
            this.WriteCheckForSpace(count);
            if (count == buffer.Length)
            {
                buffer.CopyTo(this.m_Buffer, (int)this.m_Pos);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    this.m_Buffer[this.m_Pos + i] = buffer[i];
                }
            }
            this.m_Pos += (uint)count;
        }

        private void WriteCheckForSpace(int count)
        {
            if (this.m_Pos + count >= this.m_Buffer.Length)
            {
                int num = (int)Math.Ceiling((double)((float)this.m_Buffer.Length * 1.5f));
                while (this.m_Pos + count >= num)
                {
                    num = (int)Math.Ceiling((double)((float)num * 1.5f));
                    if (num > 134217728)
                    {
                        Debug.LogWarning("Buffer size is " + num + " bytes!");
                    }
                }
                byte[] array = new byte[num];
                this.m_Buffer.CopyTo(array, 0);
                this.m_Buffer = array;
            }
        }

        public void SeekZero()
        {
            this.m_Pos = 0u;
        }

        public void Replace(byte[] buffer)
        {
            this.m_Buffer = buffer;
            this.m_Pos = 0u;
        }

        public override string ToString()
        {
            return string.Format("Buffer sz:{0} pos:{1}", this.m_Buffer.Length, this.m_Pos);
        }
    }

}
