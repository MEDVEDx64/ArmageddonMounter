using System;

namespace ArmageddonMounter
{
    public class WrappedFileException : Exception
    {
        public Exception LastException { get; }
        public string LastFaultedKey { get; }

        public string MessageBoxText =>
            "Last error: " + LastException.GetType().ToString() + " (" + LastException.Message + ")\n"
            + "File: " + LastFaultedKey;

        public WrappedFileException(Exception e, string key) : base("One or more files has failed to convert")
        {
            LastException = e;
            LastFaultedKey = key;
        }
    }
}
