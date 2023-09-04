namespace Phys.Shared
{
    public class PhysException : Exception
    {
        public PhysException() : base()
        {
        }

        public PhysException(string? message) : base(message)
        {
        }

        public PhysException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
