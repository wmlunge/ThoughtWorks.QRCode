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
        }

        public static byte[] Get(string name)
        {
            if (mMap.ContainsKey(name))
                return Convert.FromBase64String(mMap[name]);

            //.net core的ResourceManager不会用，不好用？
            var bytes = (byte[])Resources.ResourceManager.GetObject(name);

            return bytes;
        }
    }
}
