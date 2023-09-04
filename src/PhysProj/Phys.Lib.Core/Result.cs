namespace Phys.Lib.Core
{
    public static class Result
    {
        public static Result<T> Fail<T>(string error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new Result<T>(default!, error);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, null!);
        }
    }

    public class Result<T> : Result<T, string>
    {
        public Result(T value, string error) : base(value, error)
        {
        }
    }

    public class Result<T, E> where E : class
    {
        private readonly T value;
        private readonly E error;

        public Result(T value, E error)
        {
            this.error = error;
            this.value = value;
        }

        public bool Ok => error == null;

        public bool Fail => !Ok;

        public T Value => Ok ? value : throw new InvalidOperationException();

        public E Error => Fail ? error : throw new InvalidOperationException();

        public static implicit operator bool(Result<T, E> result) => result.Ok;
    }
}
