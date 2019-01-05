namespace ThoughtWorks.QRCode.Codec.Util
{
    using System;
    using System.IO;
    using System.Text;

    public class SystemUtils
    {
        public static int ReadInput(Stream sourceStream, sbyte[] target, int start, int count)
        {
            if (target.Length == 0)
            {
                return 0;
            }
            byte[] buffer = new byte[target.Length];
            int num = sourceStream.Read(buffer, start, count);
            if (num == 0)
            {
                return -1;
            }
            for (int i = start; i < (start + num); i++)
            {
                target[i] = (sbyte) buffer[i];
            }
            return num;
        }

        public static int ReadInput(TextReader sourceTextReader, short[] target, int start, int count)
        {
            if (target.Length == 0)
            {
                return 0;
            }
            char[] buffer = new char[target.Length];
            int num = sourceTextReader.Read(buffer, start, count);
            if (num == 0)
            {
                return -1;
            }
            for (int i = start; i < (start + num); i++)
            {
                target[i] = (short) buffer[i];
            }
            return num;
        }

        public static byte[] ToByteArray(string sourceString)
        {
            return Encoding.UTF8.GetBytes(sourceString);
        }

        public static byte[] ToByteArray(object[] tempObjectArray)
        {
            byte[] buffer = null;
            if (tempObjectArray != null)
            {
                buffer = new byte[tempObjectArray.Length];
                for (int i = 0; i < tempObjectArray.Length; i++)
                {
                    buffer[i] = (byte) tempObjectArray[i];
                }
            }
            return buffer;
        }

        public static byte[] ToByteArray(sbyte[] sbyteArray)
        {
            byte[] buffer = null;
            if (sbyteArray != null)
            {
                buffer = new byte[sbyteArray.Length];
                for (int i = 0; i < sbyteArray.Length; i++)
                {
                    buffer[i] = (byte) sbyteArray[i];
                }
            }
            return buffer;
        }

        public static char[] ToCharArray(byte[] byteArray)
        {
            return Encoding.UTF8.GetChars(byteArray);
        }

        public static char[] ToCharArray(sbyte[] sByteArray)
        {
            return Encoding.UTF8.GetChars(ToByteArray(sByteArray));
        }

        public static sbyte[] ToSByteArray(byte[] byteArray)
        {
            sbyte[] numArray = null;
            if (byteArray != null)
            {
                numArray = new sbyte[byteArray.Length];
                for (int i = 0; i < byteArray.Length; i++)
                {
                    numArray[i] = (sbyte) byteArray[i];
                }
            }
            return numArray;
        }

        public static int URShift(int number, int bits)
        {
            if (number >= 0)
            {
                return (number >> bits);
            }
            return ((number >> bits) + (((int) 2) << ~bits));
        }

        public static int URShift(int number, long bits)
        {
            return URShift(number, (int) bits);
        }

        public static long URShift(long number, int bits)
        {
            if (number >= 0L)
            {
                return (number >> bits);
            }
            return ((number >> bits) + (((long) 2L) << ~bits));
        }

        public static long URShift(long number, long bits)
        {
            return URShift(number, (int) bits);
        }

        public static void WriteStackTrace(Exception throwable, TextWriter stream)
        {
            stream.Write(throwable.StackTrace);
            stream.Flush();
        }
    }
}

