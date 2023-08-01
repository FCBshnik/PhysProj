﻿using Phys.Lib.Core.Files;
using Phys.Lib.Core.Files.Storage;

namespace Phys.Lib.Admin.Api.Api.Files
{
    internal static class FilesMapper
    {
        public static FileModel Map(FileDbo file)
        {
            return new FileModel
            {
                Id = file.Id,
                Code = file.Code,
                Format = file.Format,
                Size = file.Size,
                Links = file.Links.ConvertAll(l => new FileModel.LinkModel { Type = l.Type, Path = l.Path }),
            };
        }

        public static FileStorageModel Map(FileStorageDbo storage)
        {
            return new FileStorageModel
            {
                Code = storage.Code,
                Name = storage.Name,
            };
        }

        public static FileStorageFileModel Map(Lib.Files.FileInfo info)
        {
            return new FileStorageFileModel
            {
                Path = info.Path,
                Size = info.Size,
                Updated = info.Updated,
            };
        }
    }
}