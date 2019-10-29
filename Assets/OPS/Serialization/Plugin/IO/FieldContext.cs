using OPS.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OPS.Serialization.IO
{
    internal static class FieldContext
    {
        private static object threadHandle = new object();

        private static Dictionary<Type, FieldInfo[]> typeFieldDictionary = new Dictionary<Type, FieldInfo[]>();
        private static Dictionary<Type, int[]> typeFieldIdDictionary = new Dictionary<Type, int[]>();
        private static Dictionary<Type, bool[]> typeFieldOptionalDictionary = new Dictionary<Type, bool[]>();

        public static void LoadFields(Type _Type, out FieldInfo[] _Fields, out int[] _Ids, out bool[] _Optionals)
        {
            lock (threadHandle)
            {
                if (typeFieldDictionary.ContainsKey(_Type))
                {
                    _Fields = typeFieldDictionary[_Type];
                    _Ids = typeFieldIdDictionary[_Type];
                    _Optionals = typeFieldOptionalDictionary[_Type];
                    return;
                }

                var var_Fields = _Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                List<FieldInfo> var_FieldInfoList = new List<FieldInfo>();
                List<int> var_IdList = new List<int>();
                List<bool> var_Optional = new List<bool>();

                foreach (var var_Field in var_Fields)
                {
                    var var_FieldAttributes = var_Field.GetCustomAttributes(false);

                    foreach (var var_Attribute in var_FieldAttributes)
                    {
                        if (var_Attribute is SerializeAbleFieldAttribute)
                        {
                            SerializeAbleFieldAttribute f = (SerializeAbleFieldAttribute)var_Attribute;

                            //
                            var_FieldInfoList.Add(var_Field);

                            if (var_IdList.Contains(f.Index))
                            {
                                throw new Exception("Type: " + _Type.ToString() + " already contains a field with serialize id: " + f.Index);
                            }

                            var_IdList.Add(f.Index);
                            var_Optional.Add(false);
                            break;
                        }
                        if (var_Attribute is SerializeAbleFieldOptionalAttribute)
                        {
                            SerializeAbleFieldOptionalAttribute f = (SerializeAbleFieldOptionalAttribute)var_Attribute;

                            //
                            var_FieldInfoList.Add(var_Field);

                            if (var_IdList.Contains(f.Index))
                            {
                                throw new Exception("Type: " + _Type.ToString() + " already contains a field with serialize id: " + f.Index);
                            }

                            var_IdList.Add(f.Index);
                            var_Optional.Add(true);
                            break;
                        }
                    }
                }

                _Fields = var_FieldInfoList.ToArray();
                _Ids = var_IdList.ToArray();
                _Optionals = var_Optional.ToArray();

                typeFieldDictionary.Add(_Type, _Fields);
                typeFieldIdDictionary.Add(_Type, _Ids);
                typeFieldOptionalDictionary.Add(_Type, _Optionals);
            }
        }
    }
}
