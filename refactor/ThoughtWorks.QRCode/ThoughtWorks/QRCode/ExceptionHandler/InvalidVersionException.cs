namespace ThoughtWorks.QRCode.ExceptionHandler
{
    using System;

    [Serializable]
    public class InvalidVersionException : VersionInformationException
    {
        internal string message;

        public InvalidVersionException(string message)
        {
            this.message = message;
        }

        public override string Message =>
            this.message;
    }
}

