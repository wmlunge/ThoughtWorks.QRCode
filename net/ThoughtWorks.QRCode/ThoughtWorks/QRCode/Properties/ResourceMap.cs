using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThoughtWorks.QRCode.Properties
{
    public static class ResourceMap
    {
        private static Dictionary<string, string> mMap = new Dictionary<string, string>();

        static string mPath = "Base64s.txt";
        static ResourceMap()
        {
            if (File.Exists(mPath))
            {
                var lines = File.ReadAllLines(mPath, Encoding.ASCII);
                for (int index = 0; index < lines.Length; index += 2)
                    mMap.Add(lines[index], lines[index + 1]);
            }
        }

        public static byte[] Get(string name)
        {
            if (mMap.ContainsKey(name))
                return Convert.FromBase64String(mMap[name]);

            var bytes = (byte[])Resources.ResourceManager.GetObject(name);
            var base64 = Convert.ToBase64String(bytes);

            Console.WriteLine("//TODO:把结果添加到Map中，提供给.net_core版本 " + name);
            //TODO:把结果添加到Map中，提供给.net_core版本
            mMap.Add(name, base64);

            return bytes;
        }

        public static void Save()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in mMap)
            {
                sb.AppendLine(item.Key);
                sb.AppendLine(item.Value);
            }
            File.WriteAllText(mPath, sb.ToString(), Encoding.ASCII);
        }
    }
}
