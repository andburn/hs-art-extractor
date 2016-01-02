using System.Collections.Generic;
using System.Text;

namespace HsArtExtractor.Util
{
    public class StringTable
    {
        private const int FLAG_INTERNAL = 1 << 31;

        private readonly Dictionary<int, string> strings = new Dictionary<int, string>()
        {
            {-2147483648, "AABB"},
            {-2147483643, "AnimationClip"},
            {-2147483629, "AnimationCurve"},
            {-2147483614, "AnimationState"},
            {-2147483599, "Array"},
            {-2147483593, "Base"},
            {-2147483588, "BitField"},
            {-2147483579, "bitset"},
            {-2147483572, "bool"},
            {-2147483567, "char"},
            {-2147483562, "ColorRGBA"},
            {-2147483552, "Component"},
            {-2147483542, "data"},
            {-2147483537, "deque"},
            {-2147483531, "double"},
            {-2147483524, "dynamic_array"},
            {-2147483510, "FastPropertyName"},
            {-2147483493, "first"},
            {-2147483487, "float"},
            {-2147483481, "Font"},
            {-2147483476, "GameObject"},
            {-2147483465, "Generic Mono"},
            {-2147483452, "GradientNEW"},
            {-2147483440, "GUID"},
            {-2147483435, "GUIStyle"},
            {-2147483426, "int"},
            {-2147483422, "list"},
            {-2147483417, "long long"},
            {-2147483407, "map"},
            {-2147483403, "Matrix4x4f"},
            {-2147483392, "MdFour"},
            {-2147483385, "MonoBehaviour"},
            {-2147483371, "MonoScript"},
            {-2147483360, "m_ByteSize"},
            {-2147483349, "m_Curve"},
            {-2147483341, "m_EditorClassIdentifier"},
            {-2147483317, "m_EditorHideFlags"},
            {-2147483299, "m_Enabled"},
            {-2147483289, "m_ExtensionPtr"},
            {-2147483274, "m_GameObject"},
            {-2147483261, "m_Index"},
            {-2147483253, "m_IsArray"},
            {-2147483243, "m_IsStatic"},
            {-2147483232, "m_MetaFlag"},
            {-2147483221, "m_Name"},
            {-2147483214, "m_ObjectHideFlags"},
            {-2147483196, "m_PrefabInternal"},
            {-2147483179, "m_PrefabParentObject"},
            {-2147483158, "m_Script"},
            {-2147483149, "m_StaticEditorFlags"},
            {-2147483129, "m_Type"},
            {-2147483122, "m_Version"},
            {-2147483112, "Object"},
            {-2147483105, "pair"},
            {-2147483100, "PPtr<Component>"},
            {-2147483084, "PPtr<GameObject>"},
            {-2147483067, "PPtr<Material>"},
            {-2147483052, "PPtr<MonoBehaviour>"},
            {-2147483032, "PPtr<MonoScript>"},
            {-2147483015, "PPtr<Object>"},
            {-2147483002, "PPtr<Prefab>"},
            {-2147482989, "PPtr<Sprite>"},
            {-2147482976, "PPtr<TextAsset>"},
            {-2147482960, "PPtr<Texture>"},
            {-2147482946, "PPtr<Texture2D>"},
            {-2147482930, "PPtr<Transform>"},
            {-2147482914, "Prefab"},
            {-2147482907, "Quaternionf"},
            {-2147482895, "Rectf"},
            {-2147482889, "RectInt"},
            {-2147482881, "RectOffset"},
            {-2147482870, "second"},
            {-2147482863, "set"},
            {-2147482859, "short"},
            {-2147482853, "size"},
            {-2147482848, "SInt16"},
            {-2147482841, "SInt32"},
            {-2147482834, "SInt64"},
            {-2147482827, "SInt8"},
            {-2147482821, "staticvector"},
            {-2147482808, "string"},
            {-2147482801, "TextAsset"},
            {-2147482791, "TextMesh"},
            {-2147482782, "Texture"},
            {-2147482774, "Texture2D"},
            {-2147482764, "Transform"},
            {-2147482754, "TypelessData"},
            {-2147482741, "UInt16"},
            {-2147482734, "UInt32"},
            {-2147482727, "UInt64"},
            {-2147482720, "UInt8"},
            {-2147482714, "unsigned int"},
            {-2147482701, "unsigned long long"},
            {-2147482682, "unsigned short"},
            {-2147482667, "vector"},
            {-2147482660, "Vector2f"},
            {-2147482651, "Vector3f"},
            {-2147482642, "Vector4f"},
            {-2147482633, "m_ScriptingClassIdentifier"}
        };

        public StringTable()
        {
            // original strings are read from file, for easier updates I assume
            //byte[] data = File.ReadAllBytes("./Data/strings.dat");
            //LoadStrings(data, true);
            Logger.Log(LogLevel.DEBUG, "StringTable initialized, strings size: {0}", strings.Count);
        }

        public StringTable(byte[] data)
        {
            LoadStrings(data, false);
        }

        private void LoadStrings(byte[] data, bool isInternal)
        {
            for (int i = 0, n = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                {
                    string str = Encoding.ASCII.GetString(data, n, i - n);

                    if (isInternal)
                        n |= FLAG_INTERNAL;

                    strings.Add(n, str);

                    n = i + 1;
                }
            }
        }

        public void LoadStrings(byte[] data)
        {
            LoadStrings(data, false);
        }

        public string GetString(int offset)
        {
            return strings[offset];
        }
    }
}