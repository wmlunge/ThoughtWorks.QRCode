using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThoughtWorks.QRCode.Properties
{
    public static class ResourceMap
    {
        private static Dictionary<string, string> mMapNameBase64 = new Dictionary<string, string>();

        static string mPath = "Base64s.txt";
        static ResourceMap()
        {
            var lines = File.ReadAllLines(mPath);
            for (int index = 0; index < lines.Length; index += 2)
                mMapNameBase64.Add(lines[index], lines[index + 1]);
        }

        public static byte[] Get(string name)
        {
            if (mMapNameBase64.ContainsKey(name))
                return Convert.FromBase64String(mMapNameBase64[name]);

            var bytes = (byte[])Resources.ResourceManager.GetObject(name);
            var base64 = Convert.ToBase64String(bytes);
            //TODO:把结果添加到Map中，提供给.net_core版本
            File.AppendAllLines(mPath, new string[] { name, base64 }, Encoding.ASCII);
            Console.WriteLine("//TODO:把结果添加到Map中，提供给.net_core版本");

            return bytes;
        }
    }
}
