namespace Phys.Lib.Cli
{
    public interface ICommand<T>
    {
        void Run(T options);
    }
}
