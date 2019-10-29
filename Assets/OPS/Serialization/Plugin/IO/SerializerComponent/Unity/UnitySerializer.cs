using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OPS.Serialization.IO
{
    internal class UnitySerializer : ISerializerComponent
    {
        internal enum EUnityType : byte
        {
            BONEWEIGHT,
            BOUNDS,
            COLOR,
            COLOR32,
        }

        internal static Dictionary<Type, EUnityType> TypeDictionary = new Dictionary<Type, EUnityType>()
        {
            { typeof(BoneWeight), EUnityType.BONEWEIGHT },
            { typeof(Bounds), EUnityType.BOUNDS },
            { typeof(Color), EUnityType.COLOR },
            { typeof(Color32), EUnityType.COLOR32 },
        };

        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (!TypeDictionary.ContainsKey(_Type))
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

            EUnityType var_UnityType = TypeDictionary[_Type];

            switch(var_UnityType)
            {
                case EUnityType.BONEWEIGHT:
                    {
                        BoneWeight var_BoneWeight = (BoneWeight)_Object;
                        var_Writer.Write(var_BoneWeight.boneIndex0);
                        var_Writer.Write(var_BoneWeight.boneIndex1);
                        var_Writer.Write(var_BoneWeight.boneIndex2);
                        var_Writer.Write(var_BoneWeight.boneIndex3);

                        var_Writer.Write(var_BoneWeight.weight0);
                        var_Writer.Write(var_BoneWeight.weight1);
                        var_Writer.Write(var_BoneWeight.weight2);
                        var_Writer.Write(var_BoneWeight.weight3);
                        break;
                    }
                case EUnityType.BOUNDS:
                    {
                        Bounds var_Bounds = (Bounds)_Object;
                        var_Writer.Write(var_Bounds.center);
                        var_Writer.Write(var_Bounds.extents);
                        var_Writer.Write(var_Bounds.max);
                        var_Writer.Write(var_Bounds.min);
                        var_Writer.Write(var_Bounds.size);
                        break;
                    }
                case EUnityType.COLOR:
                    {
                        Color var_Color = (Color)_Object;
                        var_Writer.Write(var_Color.r);
                        var_Writer.Write(var_Color.g);
                        var_Writer.Write(var_Color.b);
                        var_Writer.Write(var_Color.a);
                        break;
                    }
                case EUnityType.COLOR32:
                    {
                        Color32 var_Color = (Color32)_Object;
                        var_Writer.Write(var_Color.r);
                        var_Writer.Write(var_Color.g);
                        var_Writer.Write(var_Color.b);
                        var_Writer.Write(var_Color.a);
                        break;
                    }
            }

            _Bytes = var_Writer.ToArray();
            return true;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if (!TypeDictionary.ContainsKey(_Type))
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

            _Object = null;

            //
            EUnityType var_UnityType = TypeDictionary[_Type];

            switch (var_UnityType)
            {
                case EUnityType.BONEWEIGHT:
                    {
                        BoneWeight var_BoneWeight = new BoneWeight();
                        var_BoneWeight.boneIndex0 = var_Reader.ReadInt32();
                        var_BoneWeight.boneIndex1 = var_Reader.ReadInt32();
                        var_BoneWeight.boneIndex2 = var_Reader.ReadInt32();
                        var_BoneWeight.boneIndex3 = var_Reader.ReadInt32();

                        var_BoneWeight.weight0 = var_Reader.ReadSingle();
                        var_BoneWeight.weight1 = var_Reader.ReadSingle();
                        var_BoneWeight.weight2 = var_Reader.ReadSingle();
                        var_BoneWeight.weight3 = var_Reader.ReadSingle();

                        _Object = var_BoneWeight;
                        break;
                    }
                case EUnityType.BOUNDS:
                    {
                        Bounds var_Bounds = new Bounds();
                        var_Bounds.center = var_Reader.ReadVector3();
                        var_Bounds.extents = var_Reader.ReadVector3();
                        var_Bounds.max = var_Reader.ReadVector3();
                        var_Bounds.min = var_Reader.ReadVector3();
                        var_Bounds.size = var_Reader.ReadVector3();

                        _Object = var_Bounds;
                        break;
                    }
                case EUnityType.COLOR:
                    {
                        Color var_Color = new Color();
                        var_Color.r = var_Reader.ReadByte();
                        var_Color.g = var_Reader.ReadByte();
                        var_Color.b = var_Reader.ReadByte();
                        var_Color.a = var_Reader.ReadByte();

                        _Object = var_Color;
                        break;
                    }
                case EUnityType.COLOR32:
                    {
                        Color32 var_Color = new Color32();
                        var_Color.r = var_Reader.ReadByte();
                        var_Color.g = var_Reader.ReadByte();
                        var_Color.b = var_Reader.ReadByte();
                        var_Color.a = var_Reader.ReadByte();

                        _Object = var_Color;
                        break;
                    }
            }
            return true;
        }
    }
}
