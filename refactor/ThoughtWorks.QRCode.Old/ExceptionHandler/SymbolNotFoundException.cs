namespace ThoughtWorks.QRCode.ExceptionHandler
{
    using System;

    [Serializable]
    public class SymbolNotFoundException : ArgumentException
    {
        internal string message = null;

        public SymbolNotFoundException(string message)
        {
            this.message = message;
        }

        public override string Message =>
            this.message;
    }
}

