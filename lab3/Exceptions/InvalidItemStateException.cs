using System;

namespace lab3.Exceptions
{
    public class InvalidItemStateException : Exception
    {
        public InvalidItemStateException()
        {
        }

        public InvalidItemStateException(string state) : base($"State {state} not implemented")
        {
        }

        public InvalidItemStateException(string state, Exception innerException) : base($"State {state} not implemented", innerException)
        {
        }
    }
}