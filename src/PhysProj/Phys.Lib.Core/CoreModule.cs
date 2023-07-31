using Autofac;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Users;
using Phys.Lib.Core.Utils;
using Phys.Lib.Core.Validation;
using Phys.Lib.Core.Works;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Phys.Lib.Tests.Unit")]

namespace Phys.Lib.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Validator>().AsImplementedInterfaces().SingleInstance();

            builder
                .RegisterService<UsersService, IUsersService>()
                .RegisterService<AuthorsSearch, IAuthorsSearch>()
                .RegisterService<AuthorsEditor, IAuthorsEditor>()
                .RegisterService<WorksSearch, IWorksSearch>()
                .RegisterService<WorksEditor, IWorksEditor>()
                .RegisterService<FileStoragesService, IFileStoragesService>()
                .RegisterService<FilesService, IFilesService>();

            builder.RegisterModule(new ValidationModule(ThisAssembly));
        }
    }
}
