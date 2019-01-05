namespace ThoughtWorks.QRCode.ExceptionHandler
{
    using System;

    [Serializable]
    public class AlignmentPatternNotFoundException : ArgumentException
    {
        internal string message = null;

        public AlignmentPatternNotFoundException(string message)
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

