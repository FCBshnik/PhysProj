using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Admin.Api.Api.Models;
using Phys.Lib.Core.Files;

namespace Phys.Lib.Admin.Api.Api.Files
{
    public static class FilesEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([AsParameters] FilesQuery query, [FromServices] IFileLinksService service) =>
            {
                var filesLinks = service.Find(query.Search);
                return TypedResults.Ok(filesLinks.Select(FilesMapper.Map).ToList());
            }).ProducesResponse<List<FileLinksModel>>("ListFilesLinks");

            builder.MapDelete("/", () =>
            {
                return TypedResults.Ok();
            }).ProducesResponse<OkModel>("DeleteFileLinks");

            builder.MapGet("storages", ([FromServices] IFileStoragesService storages) =>
            {
                return TypedResults.Ok(storages.List().Select(FilesMapper.Map).ToList());
            }).ProducesResponse<List<FileStorageModel>>("ListFileStorages");

            builder.MapGet("storages/{storageCode}/files", (string storageCode, [AsParameters]FilesQuery query, [FromServices]IFileStoragesService storages) =>
            {
                var storage = storages.Get(storageCode);
                return TypedResults.Ok(storage.List(query.Search).Select(FilesMapper.Map).ToList());
            }).ProducesResponse<List<FileStorageFileInfoModel>>("ListFileStorageFiles");

            builder.MapPost("storages/{storageCode}/files/link", (string storageCode, [FromBody]FileStorageLinkModel model, [FromServices]IFileLinksService service) =>
            {
                var fileLinks = service.CreateFromStorageFile(storageCode, model.FilePath);
                return TypedResults.Ok(FilesMapper.Map(fileLinks));
            }).ProducesResponse<FileLinksModel>("LinkFileStorageFile");
        }
    }
}
