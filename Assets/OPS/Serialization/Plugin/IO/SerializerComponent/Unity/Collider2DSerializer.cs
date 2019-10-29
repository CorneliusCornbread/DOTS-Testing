using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OPS.Serialization.IO
{
    internal class Collider2DSerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (_Type != typeof(Collider2D))
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

            Collider2D var_Collider = _Object as Collider2D;

            //Shared
            var_Writer.Write(var_Collider.enabled);
            var_Writer.Write(var_Collider.density);
            var_Writer.Write(var_Collider.isTrigger);
            var_Writer.Write(var_Collider.offset);

            if (var_Collider is BoxCollider2D)
            {
                var_Writer.Write(((BoxCollider2D)var_Collider).autoTiling);
                var_Writer.Write(((BoxCollider2D)var_Collider).edgeRadius);
                var_Writer.Write(((BoxCollider2D)var_Collider).size);
            }
            if (var_Collider is EdgeCollider2D)
            {
                var_Writer.Write(((EdgeCollider2D)var_Collider).edgeRadius);

                byte[] var_Bytes = Serializer.Internal_Serialize(typeof(Vector2[]), ((EdgeCollider2D)var_Collider).points);
                var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
            }
            else if (var_Collider is CapsuleCollider2D)
            {
                var_Writer.Write((int)((CapsuleCollider2D)var_Collider).direction);
                var_Writer.Write(((CapsuleCollider2D)var_Collider).size);
            }
            else if (var_Collider is CircleCollider2D)
            {
                var_Writer.Write(((CircleCollider2D)var_Collider).radius);
            }
            else if (var_Collider is PolygonCollider2D)
            {
                var_Writer.Write(((PolygonCollider2D)var_Collider).autoTiling);
                var_Writer.Write(((PolygonCollider2D)var_Collider).pathCount);

                byte[] var_Bytes = Serializer.Internal_Serialize(typeof(Vector2[]), ((PolygonCollider2D)var_Collider).points);
                var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
            }

            _Bytes = var_Writer.ToArray();
            return true;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if (_Type != typeof(Collider2D))
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

            Collider2D var_Collider = new Collider2D();

            //Shared
            var_Collider.enabled = var_Reader.ReadBoolean();
            var_Collider.density = var_Reader.ReadSingle();
            var_Collider.isTrigger = var_Reader.ReadBoolean();
            var_Collider.offset = var_Reader.ReadVector2();

            if (var_Collider is BoxCollider2D)
            {
                ((BoxCollider2D)var_Collider).autoTiling = var_Reader.ReadBoolean();
                ((BoxCollider2D)var_Collider).edgeRadius = var_Reader.ReadSingle();
                ((BoxCollider2D)var_Collider).size = var_Reader.ReadVector2();
            }
            if (var_Collider is EdgeCollider2D)
            {
                ((EdgeCollider2D)var_Collider).edgeRadius = var_Reader.ReadSingle();

                byte[] var_Bytes = var_Reader.ReadBytesAndSize();
                ((EdgeCollider2D)var_Collider).points = (Vector2[])Serializer.DeSerialize(typeof(Vector2[]), var_Bytes);
            }
            else if (var_Collider is CapsuleCollider2D)
            {
                ((CapsuleCollider2D)var_Collider).direction = (CapsuleDirection2D)var_Reader.ReadInt32();
                ((CapsuleCollider2D)var_Collider).size = var_Reader.ReadVector2();
            }
            else if (var_Collider is CircleCollider2D)
            {
                ((CircleCollider2D)var_Collider).radius = var_Reader.ReadSingle();
            }
            else if (var_Collider is PolygonCollider2D)
            {
                ((PolygonCollider2D)var_Collider).autoTiling = var_Reader.ReadBoolean();
                ((PolygonCollider2D)var_Collider).pathCount = var_Reader.ReadInt32();

                byte[] var_Bytes = var_Reader.ReadBytesAndSize();
                ((PolygonCollider2D)var_Collider).points = (Vector2[])Serializer.DeSerialize(typeof(Vector2[]), var_Bytes);
            }

            _Object = var_Collider;
            return true;
        }
    }
}
