namespace Phys.Shared.Cache
{
    public class Expirable<T> where T : class
    {
        private readonly TimeSpan expiration;
        private readonly Func<T> factory;

        private T? value;
        private DateTime? createdAt;

        public Expirable(Func<T> factory, TimeSpan expiration)
        {
            this.factory = factory;
            this.expiration = expiration;
        }

        public T Value
        {
            get
            {
                if (value == null || DateTime.UtcNow - createdAt > expiration)
                {
                    value = factory();
                    createdAt = DateTime.UtcNow;
                }

                return value;
            }
        }

        public void Reset()
        {
            value = default;
        }
    }
}
