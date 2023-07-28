using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core.Files;

namespace Phys.Lib.Admin.Api.Api.Files
{
    public static class FilesEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([AsParameters] FilesQuery query) =>
            {
                return TypedResults.Ok();
            }).ProducesResponse<List<FileInfoModel>>("ListFiles");

            builder.MapGet("storages", ([FromServices] IFileStoragesService storages) =>
            {
                return TypedResults.Ok(storages.List().Select(FilesMapper.Map).ToList());
            }).ProducesResponse<List<FileStorageModel>>("ListStorages");

            builder.MapGet("storages/{code}/files", (string code, [AsParameters]FilesQuery query, [FromServices]IFileStoragesService storages) =>
            {
                var storage = storages.Get(code);
                return TypedResults.Ok(storage.List(query.Search).Select(FilesMapper.Map));
            }).ProducesResponse<List<FileStorageFileModel>>("ListStorageFiles");
        }
    }
}
