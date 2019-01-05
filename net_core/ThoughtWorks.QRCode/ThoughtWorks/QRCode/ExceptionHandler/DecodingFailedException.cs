namespace ThoughtWorks.QRCode.ExceptionHandler
{
    using System;

    [Serializable]
    public class DecodingFailedException : ArgumentException
    {
        internal string message = null;

        public DecodingFailedException(string message)
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

