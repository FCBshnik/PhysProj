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
            }).ProducesResponse<List<FileLinksModel>>("ListFiles");

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
            }).ProducesResponse<List<FileStorageFileInfoModel>>("ListStorageFiles");

            builder.MapPost("storages/{storageCode}/files/link", (string storageCode, [FromBody]FileStorageLinkModel model, [FromServices] IFileStoragesService storages) =>
            {
                var file = storages.CreateFileFromStorage(storageCode, model.FilePath);
                return TypedResults.Ok(FilesMapper.Map(file));
            }).ProducesResponse<FileLinksModel>("LinkStorageFile");
        }
    }
}
