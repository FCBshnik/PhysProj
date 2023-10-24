using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Admin.Api.Api.Models;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Files.Storage;

namespace Phys.Lib.Admin.Api.Api.Files
{
    public static class FilesEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([AsParameters] FilesQuery query, [FromServices] IFilesService service) =>
            {
                var filesLinks = service.Find(query.Search);
                return TypedResults.Ok(filesLinks.Select(FilesMapper.Map).ToList());
            }).ProducesResponse<List<FileModel>>("ListFiles");

            builder.MapDelete("/{code}", (string code, [FromServices] IFilesService service) =>
            {
                var file = service.FindByCode(code);
                if (file != null)
                    service.Delete(file);

                return TypedResults.Ok(OkModel.Ok);
            }).ProducesResponse<OkModel>("DeleteFile");

            builder.MapGet("storages", ([FromServices] IFileStoragesService storages) =>
            {
                return TypedResults.Ok(storages.List().Select(FilesMapper.Map).ToList());
            }).ProducesResponse<List<FileStorageModel>>("ListStorages");

            builder.MapGet("storages/{storageCode}/files", (string storageCode, [AsParameters]FilesQuery query, [FromServices]IFileStoragesService storages) =>
            {
                var storage = storages.Get(storageCode);
                return TypedResults.Ok(storage.List(query.Search).Select(FilesMapper.Map).ToList());
            }).ProducesResponse<List<FileStorageFileModel>>("ListStorageFiles");

            builder.MapPost("storages/{storageCode}/files/link", (string storageCode, [FromBody]FileStorageLinkModel model, [FromServices] IFilesService service) =>
            {
                var file = service.CreateFileFromStorage(storageCode, model.FileId);
                return TypedResults.Ok(FilesMapper.Map(file));
            }).ProducesResponse<FileModel>("LinkStorageFile");

            builder.MapPost("storages/{storageCode}/refresh", (string storageCode, [FromServices] IFileStoragesService storages) =>
            {
                var storage = storages.Get(storageCode);
                storage.Refresh();
                return TypedResults.Ok(OkModel.Ok);
            }).ProducesResponse<OkModel>("RefreshStorage");
        }
    }
}
