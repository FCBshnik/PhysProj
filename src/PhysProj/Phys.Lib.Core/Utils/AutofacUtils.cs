using Autofac;

namespace Phys.Lib.Core.Utils
{
    public static class AutofacUtils
    {
        public static ContainerBuilder RegisterService<S, I>(this ContainerBuilder builder)
            where S: notnull
            where I : notnull
        {
            builder.RegisterType<S>().As<I>().SingleInstance();
            return builder;
        }
    }
}
