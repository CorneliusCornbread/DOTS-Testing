using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OPS.Serialization.IO
{
    internal class ColliderSerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (_Type != typeof(Collider))
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

            Collider var_Collider = _Object as Collider;
            
            //Shared
            var_Writer.Write(var_Collider.contactOffset);
            var_Writer.Write(var_Collider.enabled);
            var_Writer.Write(var_Collider.isTrigger);

            if(var_Collider is BoxCollider)
            {
                var_Writer.Write(((BoxCollider)var_Collider).center);
                var_Writer.Write(((BoxCollider)var_Collider).size);
            }
            else if (var_Collider is SphereCollider)
            {
                var_Writer.Write(((SphereCollider)var_Collider).center);
                var_Writer.Write(((SphereCollider)var_Collider).radius);
            }
            else if (var_Collider is CapsuleCollider)
            {
                var_Writer.Write(((CapsuleCollider)var_Collider).center);
                var_Writer.Write((Int32)((CapsuleCollider)var_Collider).direction);
                var_Writer.Write(((CapsuleCollider)var_Collider).height);
                var_Writer.Write(((CapsuleCollider)var_Collider).radius);
            }
            else if (var_Collider is MeshCollider)
            {
                var_Writer.Write(((MeshCollider)var_Collider).convex);
                //var_Writer.Write(((MeshCollider)var_Collider).cookingOptions);
                var_Writer.Write(((MeshCollider)var_Collider).inflateMesh);

                //var_Writer.Write(.Write(((MeshCollider)var_Collider).sharedMesh);
                //byte[] var_Bytes = Serializer.Serialize(var_ElementType, var_Array.GetValue(arrayIndexer.Current));
                //var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

                var_Writer.Write(((MeshCollider)var_Collider).skinWidth);
            }

            _Bytes = var_Writer.ToArray();
            return true;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if (_Type != typeof(Collider))
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

            Collider var_Collider = new Collider();

            //Shared
            var_Collider.contactOffset = var_Reader.ReadSingle();
            var_Collider.enabled = var_Reader.ReadBoolean();
            var_Collider.isTrigger = var_Reader.ReadBoolean();

            if (var_Collider is BoxCollider)
            {
                ((BoxCollider)var_Collider).center = var_Reader.ReadVector3();
                ((BoxCollider)var_Collider).size = var_Reader.ReadVector3();
            }
            else if (var_Collider is SphereCollider)
            {
                ((SphereCollider)var_Collider).center = var_Reader.ReadVector3();
                ((SphereCollider)var_Collider).radius = var_Reader.ReadSingle();
            }
            else if (var_Collider is CapsuleCollider)
            {
                ((CapsuleCollider)var_Collider).center = var_Reader.ReadVector3();
                ((CapsuleCollider)var_Collider).direction = var_Reader.ReadInt32();
                ((CapsuleCollider)var_Collider).height = var_Reader.ReadSingle();
                ((CapsuleCollider)var_Collider).radius = var_Reader.ReadSingle();
            }
            else if (var_Collider is MeshCollider)
            {
                ((MeshCollider)var_Collider).convex = var_Reader.ReadBoolean();
                ((MeshCollider)var_Collider).inflateMesh = var_Reader.ReadBoolean();
                ((MeshCollider)var_Collider).skinWidth = var_Reader.ReadSingle();
            }

            _Object = var_Collider;
            return true;
        }
    }
}
