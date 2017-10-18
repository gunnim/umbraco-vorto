using System;

namespace Our.Umbraco.Vorto.Exceptions
{
    /// <summary>
    /// Exceptions caused by the Vorto library
    /// </summary>
    public class VortoException : Exception
    {
        /// <summary>
        /// Create a Vorto exception
        /// </summary>
        /// <param name="message"></param>
        public VortoException(string message) : base(message) { }
    }
}
