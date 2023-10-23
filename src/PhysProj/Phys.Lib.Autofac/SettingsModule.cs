using Autofac;
using Phys.Files.PCloud;
using Phys.Shared.Settings;

namespace Phys.Lib.Autofac
{
    internal class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterSettings(builder, "pcloud", PCloudStorageSettings.Default);
        }

        private void RegisterSettings<T>(ContainerBuilder builder, string code, T defaultValue) where T : notnull
        {
            builder.Register(c => c.Resolve<ISettings>().Get(code))
                .As<T>().InstancePerDependency();

            builder.RegisterBuildCallback(s => s.Resolve<ISettings>().Register(code, defaultValue));
        }
    }
}
