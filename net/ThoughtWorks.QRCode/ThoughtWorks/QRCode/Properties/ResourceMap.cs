using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThoughtWorks.QRCode.Properties
{
    public static class ResourceMap
    {
        public static byte[] Get(string name)
        {
            return (byte[])Resources.ResourceManager.GetObject(name);
        }

        /// <summary>
        /// 把结果提供给.net_core版本
        /// </summary>
        public static void Save()
        {
            var namesString = "qrv1_0,qrv1_1,qrv1_2,qrv1_3,qrv10_0,qrv10_1,qrv10_2,qrv10_3,qrv11_0,qrv11_1,qrv11_2,qrv11_3,qrv12_0,qrv12_1,qrv12_2,qrv12_3,qrv13_0,qrv13_1,qrv13_2,qrv13_3,qrv14_0,qrv14_1,qrv14_2,qrv14_3,qrv15_0,qrv15_1,qrv15_2,qrv15_3,qrv16_0,qrv16_1,qrv16_2,qrv16_3,qrv17_0,qrv17_1,qrv17_2,qrv17_3,qrv18_0,qrv18_1,qrv18_2,qrv18_3,qrv19_0,qrv19_1,qrv19_2,qrv19_3,qrv2_0,qrv2_1,qrv2_2,qrv2_3,qrv20_0,qrv20_1,qrv20_2,qrv20_3,qrv21_0,qrv21_1,qrv21_2,qrv21_3,qrv22_0,qrv22_1,qrv22_2,qrv22_3,qrv23_0,qrv23_1,qrv23_2,qrv23_3,qrv24_0,qrv24_1,qrv24_2,qrv24_3,qrv25_0,qrv25_1,qrv25_2,qrv25_3,qrv26_0,qrv26_1,qrv26_2,qrv26_3,qrv27_0,qrv27_1,qrv27_2,qrv27_3,qrv28_0,qrv28_1,qrv28_2,qrv28_3,qrv29_0,qrv29_1,qrv29_2,qrv29_3,qrv3_0,qrv3_1,qrv3_2,qrv3_3,qrv30_0,qrv30_1,qrv30_2,qrv30_3,qrv31_0,qrv31_1,qrv31_2,qrv31_3,qrv32_0,qrv32_1,qrv32_2,qrv32_3,qrv33_0,qrv33_1,qrv33_2,qrv33_3,qrv34_0,qrv34_1,qrv34_2,qrv34_3,qrv35_0,qrv35_1,qrv35_2,qrv35_3,qrv36_0,qrv36_1,qrv36_2,qrv36_3,qrv37_0,qrv37_1,qrv37_2,qrv37_3,qrv38_0,qrv38_1,qrv38_2,qrv38_3,qrv39_0,qrv39_1,qrv39_2,qrv39_3,qrv4_0,qrv4_1,qrv4_2,qrv4_3,qrv40_0,qrv40_1,qrv40_2,qrv40_3,qrv5_0,qrv5_1,qrv5_2,qrv5_3,qrv6_0,qrv6_1,qrv6_2,qrv6_3,qrv7_0,qrv7_1,qrv7_2,qrv7_3,qrv8_0,qrv8_1,qrv8_2,qrv8_3,qrv9_0,qrv9_1,qrv9_2,qrv9_3,qrvfr1,qrvfr10,qrvfr11,qrvfr12,qrvfr13,qrvfr14,qrvfr15,qrvfr16,qrvfr17,qrvfr18,qrvfr19,qrvfr2,qrvfr20,qrvfr21,qrvfr22,qrvfr23,qrvfr24,qrvfr25,qrvfr26,qrvfr27,qrvfr28,qrvfr29,qrvfr3,qrvfr30,qrvfr31,qrvfr32,qrvfr33,qrvfr34,qrvfr35,qrvfr36,qrvfr37,qrvfr38,qrvfr39,qrvfr4,qrvfr40,qrvfr5,qrvfr6,qrvfr7,qrvfr8,qrvfr9,rsc10,rsc13,rsc15,rsc16,rsc17,rsc18,rsc20,rsc22,rsc24,rsc26,rsc28,rsc30,rsc32,rsc34,rsc36,rsc40,rsc42,rsc44,rsc46,rsc48,rsc50,rsc52,rsc54,rsc56,rsc58,rsc60,rsc62,rsc64,rsc66,rsc68,rsc7";

            StringBuilder sb = new StringBuilder();
            foreach (var name in namesString.Split(','))
            {
                sb.AppendLine(name);
                sb.AppendLine(Convert.ToBase64String(Get(name)));
            }
            File.WriteAllText("Base64s.txt", sb.ToString(), Encoding.ASCII);
        }
    }
}
