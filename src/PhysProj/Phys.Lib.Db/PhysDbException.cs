namespace Phys.Lib.Db
{
    public class PhysDbException : Exception
    {
        public PhysDbException() : base()
        {
        }

        public PhysDbException(string? message) : base(message)
        {
        }

        public PhysDbException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
