using OPS.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OPS.Serialization.IO
{
    internal class MeshSerializer : ISerializerComponent
    {
        public bool Serialize(Type _Type, object _Object, out byte[] _Bytes)
        {
            if (_Type != typeof(Mesh))
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

            Mesh var_Mesh = _Object as Mesh;

            //Bindposes
            byte[] var_Bytes = Serializer.Internal_Serialize(typeof(Matrix4x4[]), var_Mesh.bindposes);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            //BoneWeights
            var_Bytes = Serializer.Internal_Serialize(typeof(BoneWeight[]), var_Mesh.boneWeights);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            //Bounds
            var_Bytes = Serializer.Internal_Serialize(typeof(Bounds), var_Mesh.bounds);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            //Vertices
            var_Bytes = Serializer.Internal_Serialize(typeof(Vector3[]), var_Mesh.vertices);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            //Normals
            var_Bytes = Serializer.Internal_Serialize(typeof(Vector3[]), var_Mesh.normals);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            //Colors
            var_Bytes = Serializer.Internal_Serialize(typeof(Color[]), var_Mesh.colors);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            //Colors32
            var_Bytes = Serializer.Internal_Serialize(typeof(Color32[]), var_Mesh.colors32);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            //UV
            var_Bytes = Serializer.Internal_Serialize(typeof(Vector2[]), var_Mesh.uv);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
            var_Bytes = Serializer.Internal_Serialize(typeof(Vector2[]), var_Mesh.uv2);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
            var_Bytes = Serializer.Internal_Serialize(typeof(Vector2[]), var_Mesh.uv3);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
            var_Bytes = Serializer.Internal_Serialize(typeof(Vector2[]), var_Mesh.uv4);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            //SubMeshCount
            var_Writer.Write(var_Mesh.subMeshCount);
            for(int s = 0; s < var_Mesh.subMeshCount; s++)
            {
                var_Bytes = Serializer.Internal_Serialize(typeof(int[]), var_Mesh.GetIndices(s));
                var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

                var_Writer.Write((byte)var_Mesh.GetTopology(s));

                var_Bytes = Serializer.Internal_Serialize(typeof(int[]), var_Mesh.GetTriangles(s));
                var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);
            }

            //Tangents
            var_Bytes = Serializer.Internal_Serialize(typeof(Vector4[]), var_Mesh.tangents);
            var_Writer.WriteBytesAndSize(var_Bytes, var_Bytes.Length);

            _Bytes = var_Writer.ToArray();
            return true;
        }

        public bool DeSerialize(Type _Type, byte[] _Bytes, out object _Object)
        {
            if (_Type != typeof(Mesh))
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

            Mesh var_Mesh = new Mesh();

            //Bindposes
            byte[] var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.bindposes = Serializer.DeSerialize<Matrix4x4[]>(var_Bytes);

            //BoneWeights
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.boneWeights = Serializer.DeSerialize<BoneWeight[]>(var_Bytes);

            //Bound
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.bounds = Serializer.DeSerialize<Bounds>(var_Bytes);

            //Vertices
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetVertices(Serializer.DeSerialize<Vector3[]>(var_Bytes).ToList());

            //Normales
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetNormals(Serializer.DeSerialize<Vector3[]>(var_Bytes).ToList());

            //Color
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetColors(Serializer.DeSerialize<Color[]>(var_Bytes).ToList());

            //Color32
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetColors(Serializer.DeSerialize<Color32[]>(var_Bytes).ToList());

            //UV
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetUVs(0, Serializer.DeSerialize<Vector2[]>(var_Bytes).ToList());
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetUVs(1, Serializer.DeSerialize<Vector2[]>(var_Bytes).ToList());
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetUVs(2, Serializer.DeSerialize<Vector2[]>(var_Bytes).ToList());
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetUVs(3, Serializer.DeSerialize<Vector2[]>(var_Bytes).ToList());

            //SubMeshes
            int var_SubMeshCount = var_Reader.ReadInt32();
            var_Mesh.subMeshCount = var_SubMeshCount;
            for (int s = 0; s < var_SubMeshCount; s++)
            {
                var_Bytes = var_Reader.ReadBytesAndSize();
                int[] var_Indices = Serializer.DeSerialize<int[]>(var_Bytes);

                MeshTopology var_MeshTopology = (MeshTopology)var_Reader.ReadByte();

                var_Mesh.SetIndices(var_Indices, var_MeshTopology, s);

                var_Bytes = var_Reader.ReadBytesAndSize();
                var_Mesh.SetTriangles(Serializer.DeSerialize<int[]>(var_Bytes), s);
            }

            //Tangents
            var_Bytes = var_Reader.ReadBytesAndSize();
            var_Mesh.SetTangents(Serializer.DeSerialize<Vector4[]>(var_Bytes).ToList());

            _Object = var_Mesh;
            return true;
        }
    }
}
