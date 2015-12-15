using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HearthstoneDisunity.Util
{
    // some random stuff to figure out whats going on
    internal class DebugUtils
    {
        public static void PrintReverseMapCount(IEnumerable<KeyValuePair<string, string>> map)
        {
            Dictionary<string, int> reverse = new Dictionary<string, int>();
            foreach (var m in map)
            {
                if (!reverse.ContainsKey(m.Value))
                    reverse[m.Value] = 0;
                reverse[m.Value]++;
            }
            Console.WriteLine("Reverse: " + reverse.Keys.Count);
            Console.WriteLine("Total: " + reverse.Values.Sum());
        }

        public static bool IsValidCardTex(List<string> cards, string texname)
        {
            foreach (var pathname in cards)
            {
                if (pathname.Contains(texname))
                    return true;
            }
            return false;
        }

        public static void PrintFilesNotFound(string dir, List<string> cardIds, Dictionary<string, string> map)
        {
            var created = Directory.GetFiles(dir, "*.dds");
            var missing = 0;
            foreach (var id in cardIds)
            {
                if (!map.ContainsKey(id) || string.IsNullOrWhiteSpace(map[id]))
                {
                    Console.WriteLine("Mapping not found for: {0}", id);
                }
                else
                {
                    var fileinfo = new FileInfo("C:/" + map[id]);
                    var filename = fileinfo.Name.Replace(fileinfo.Extension, "");
                    var found = false;
                    foreach (var path in created)
                    {
                        if (path.Contains(filename))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Console.WriteLine("File not found: {0} ({1})", filename, id);
                        missing++;
                    }
                }
            }
            Console.WriteLine("Missing: " + missing);
        }

        public static string AllPropsToString(Object obj)
        {
            PropertyInfo[] props = obj.GetType().GetProperties();
            StringBuilder str = new StringBuilder();
            foreach (var p in props)
            {
                Type t = p.PropertyType;
                object objValue = p.GetValue(obj, null);
                if (typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string))
                {
                    IEnumerable e = (IEnumerable)objValue;
                    str.AppendLine(p.Name + ":");
                    foreach (var i in e)
                    {
                        str.AppendLine(i.ToString());
                    }
                }
                else
                {
                    str.AppendLine(p.Name + " = " + objValue);
                }
            }
            return str.ToString();
        }
    }
}