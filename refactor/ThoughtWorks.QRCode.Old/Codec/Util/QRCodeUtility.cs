namespace ThoughtWorks.QRCode.Codec.Util
{
    using System;
    using System.Text;

    public class QRCodeUtility
    {
        public static byte[] AsciiStringToByteArray(string str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static string FromASCIIByteArray(byte[] characters)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(characters);
        }

        public static string FromUnicodeByteArray(byte[] characters) => 
            Encoding.UTF8.GetString(characters);

        public static bool IsUnicode(byte[] byteData)
        {
            string str = FromASCIIByteArray(byteData);
            string str2 = FromUnicodeByteArray(byteData);
            byte[] buffer = AsciiStringToByteArray(str);
            byte[] buffer2 = UnicodeStringToByteArray(str2);
            return (buffer[0] != buffer2[0]);
        }

        public static bool IsUniCode(string value)
        {
            byte[] characters = AsciiStringToByteArray(value);
            byte[] buffer2 = UnicodeStringToByteArray(value);
            string str = FromASCIIByteArray(characters);
            string str2 = FromUnicodeByteArray(buffer2);
            return (str != str2);
        }

        public static int sqrt(int val)
        {
            int num2 = 0;
            int num3 = 0x8000;
            int num4 = 15;
            do
            {
                int num;
                if (val >= (num = ((num2 << 1) + num3) << num4--))
                {
                    num2 += num3;
                    val -= num;
                }
            }
            while ((num3 = num3 >> 1) > 0);
            return num2;
        }

        public static byte[] UnicodeStringToByteArray(string str) => 
            Encoding.UTF8.GetBytes(str);
    }
}

