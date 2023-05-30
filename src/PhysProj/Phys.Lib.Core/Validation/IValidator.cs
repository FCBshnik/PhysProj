namespace Phys.Lib.Core.Validation
{
    public interface IValidator
    {
        void Validate<T>(T value);
    }
}
