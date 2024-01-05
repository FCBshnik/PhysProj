namespace Phys.Lib.Cli
{
    public interface ICommand<in T>
    {
        void Run(T options);
    }
}
