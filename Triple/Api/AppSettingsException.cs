using System;

namespace Triple.Api
{
    public class AppSettingsException : Exception
    {
        public AppSettingsException(string message)
            : this(message, null)
        {
        }

        public AppSettingsException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
