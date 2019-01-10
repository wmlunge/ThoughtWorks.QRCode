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

        static ResourceMap()
        {
            //TODO:从.net版本中获取bytes并存储到这里
            var lines = File.ReadAllLines("Base64s.txt", Encoding.ASCII);
            for (int index = 0; index < lines.Length; index += 2)
                mMap.Add(lines[index], lines[index + 1]);
        }

        public static byte[] Get(string name)
        {
            return Convert.FromBase64String(mMap[name]);
        }
    }
}
