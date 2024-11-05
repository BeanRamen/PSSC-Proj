namespace lab3.Exceptions
{
    public class InvalidCartRegistrationNumberException : Exception
    {
        public InvalidCartRegistrationNumberException()
        {
        }

        public InvalidCartRegistrationNumberException(string? message) : base(message)
        {
        }

        public InvalidCartRegistrationNumberException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}