namespace ThoughtWorks.QRCode.ExceptionHandler
{
    using System;

    [Serializable]
    public class InvalidDataBlockException : ArgumentException
    {
        internal string message = null;

        public InvalidDataBlockException(string message)
        {
            this.message = message;
        }

        public override string Message
        {
            get
            {
                return this.message;
            }
        }
    }
}

