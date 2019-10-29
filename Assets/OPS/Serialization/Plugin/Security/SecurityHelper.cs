using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.Security
{
    internal static class SecurityHelper
    {
        private static bool Cryption(byte[] _Data, int _Key)
        {
            byte[] var_SplitKey = new byte[4];
            var_SplitKey[0] = (byte)(_Key & 0xFF);
            var_SplitKey[1] = (byte)(_Key >> 8 & 0xFF);
            var_SplitKey[2] = (byte)(_Key >> 16 & 0xFF);
            var_SplitKey[3] = (byte)(_Key >> 24 & 0xFF);

            return Cryption(_Data, var_SplitKey);
        }

        private static bool Cryption(byte[] _Data, String _Key)
        {
            if(String.IsNullOrEmpty(_Key))
            {
                return false;
            }

            byte[] var_StringBytes = System.Text.Encoding.UTF8.GetBytes(_Key);

            int var_Step = (var_StringBytes.Length-1) / 4;

            byte[] var_SplitKey = new byte[4];
            var_SplitKey[0] = var_StringBytes[var_Step];
            var_SplitKey[1] = var_StringBytes[var_Step * 2];
            var_SplitKey[2] = var_StringBytes[var_Step * 3];
            var_SplitKey[3] = var_StringBytes[var_Step * 4];

            return Cryption(_Data, var_SplitKey);
        }

        private static bool Cryption(byte[] _Data, byte[] _Key)
        {
            //Potenz von 10
            int var_DataLength = (int)Math.Log10(_Data.Length);
            var_DataLength = Math.Max(0, var_DataLength - 2);

            //(Potenz - 2) * 10
            int var_Step = (int)Math.Pow(10, var_DataLength);

            int k = 0;
            for (int i = 0; i < _Data.Length; i += var_Step)
            {
                if (k == 4)
                {
                    k = 0;
                }

                _Data[i] = (byte)(_Data[i] ^ _Key[k]);

                k += 1;
            }

            return true;
        }

        public static bool Encrypt(byte[] _Data, int _Key)
        {
            if(_Data == null)
            {
                return false;
            }

            return Cryption(_Data, _Key);
        }

        public static bool Encrypt(byte[] _Data, String _Key)
        {
            if (_Data == null)
            {
                return false;
            }

            return Cryption(_Data, _Key);
        }

        public static bool Decrypt(byte[] _Data, int _Key)
        {
            if (_Data == null)
            {
                return false;
            }

            return Cryption(_Data, _Key);
        }

        public static bool Decrypt(byte[] _Data, String _Key)
        {
            if (_Data == null)
            {
                return false;
            }

            return Cryption(_Data, _Key);
        }
    }
}
