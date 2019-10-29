using System.Runtime.InteropServices;

namespace OPS.Serialization.Byte
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct LongDouble
    {
        [FieldOffset(0)]
        public double doubleValue;

        [FieldOffset(0)]
        public ulong longValue;
    }

}
