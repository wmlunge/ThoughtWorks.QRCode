namespace ThoughtWorks.QRCode.Codec.Util
{
    using System;

    public class ContentConverter
    {
        internal static char n = '\n';

        public static string convert(string targetString)
        {
            if (targetString != null)
            {
                if (targetString.IndexOf("MEBKM:") > -1)
                {
                    targetString = convertDocomoBookmark(targetString);
                }
                if (targetString.IndexOf("MECARD:") > -1)
                {
                    targetString = convertDocomoAddressBook(targetString);
                }
                if (targetString.IndexOf("MATMSG:") > -1)
                {
                    targetString = convertDocomoMailto(targetString);
                }
                if (targetString.IndexOf(@"http\://") > -1)
                {
                    targetString = replaceString(targetString, @"http\://", "\nhttp://");
                }
            }
            return targetString;
        }

        private static string convertDocomoAddressBook(string targetString)
        {
            targetString = removeString(targetString, "MECARD:");
            targetString = removeString(targetString, ";");
            targetString = replaceString(targetString, "N:", "NAME1:");
            targetString = replaceString(targetString, "SOUND:", n + "NAME2:");
            targetString = replaceString(targetString, "TEL:", n + "TEL1:");
            targetString = replaceString(targetString, "EMAIL:", n + "MAIL1:");
            targetString = targetString + n;
            return targetString;
        }

        private static string convertDocomoBookmark(string targetString)
        {
            targetString = removeString(targetString, "MEBKM:");
            targetString = removeString(targetString, "TITLE:");
            targetString = removeString(targetString, ";");
            targetString = removeString(targetString, "URL:");
            return targetString;
        }

        private static string convertDocomoMailto(string s)
        {
            string str = s;
            char ch = '\n';
            return (replaceString(replaceString(replaceString(removeString(removeString(str, "MATMSG:"), ";"), "TO:", "MAILTO:"), "SUB:", ch + "SUBJECT:"), "BODY:", ch + "BODY:") + ch);
        }

        private static string removeString(string s, string s1)
        {
            return replaceString(s, s1, "");
        }

        private static string replaceString(string s, string s1, string s2)
        {
            string str = s;
            for (int i = str.IndexOf(s1, 0); i > -1; i = str.IndexOf(s1, (int) (i + s2.Length)))
            {
                str = str.Substring(0, i) + s2 + str.Substring(i + s1.Length);
            }
            return str;
        }
    }
}

