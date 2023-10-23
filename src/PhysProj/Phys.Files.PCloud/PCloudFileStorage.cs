using Microsoft.Extensions.Logging;
using Phys.Files.PCloud.Models;
using Phys.Shared;
using Phys.Shared.Utils;
using System.Text.RegularExpressions;

namespace Phys.Files.PCloud
{
    public class PCloudFileStorage : IFileStorage
    {
        private static readonly HttpClient downloadHttp = new HttpClient();

        private readonly IPCloudApiClient api;
        private readonly ILogger<PCloudFileStorage> log;
        private readonly PCloudStorageSettings settings;

        private string? accessToken;
        private List<MetadataResponse>? list;

        public PCloudFileStorage(IPCloudApiClient api, PCloudStorageSettings settings, ILogger<PCloudFileStorage> log)
        {
            this.api = api;
            this.settings = settings;
            this.log = log;
        }

        public string Code => "pcloud";

        public void Refresh()
        {
            list = null;
            log.LogInformation("reset cache");
        }

        public void Delete(string path)
        {
            throw new NotImplementedException();
        }

        public Stream Download(string fileId)
        {
            var fileIdLong = long.Parse(fileId);
            EnsureLoggedIn();

            var pubLink = api.GetFilePubLink(fileIdLong, accessToken!).Result;
            var download = api.GetPubLinkDownload(pubLink.Code).Result;
            var url = new UriBuilder("https", download.Hosts.First());
            url.Path = download.Path;
            return downloadHttp.GetStreamAsync(url.ToString()).Result;
        }

        public StorageFileInfo? Get(string fileId)
        {
            var fileIdLong = long.Parse(fileId);
            EnsureList();
            var file = list!.FirstOrDefault(f => f.Fileid == fileIdLong);
            if (file == null)
                return null;
            return Map(file);
        }

        public List<StorageFileInfo> List(string? search)
        {
            EnsureList();

            return list!
                .Where(i => !i.Isfolder)
                .Where(i => search == null || i.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Take(20)
                .Select(Map)
                .ToList();
        }

        public StorageFileInfo Upload(Stream data, string name)
        {
            EnsureList();

            var path = NormilizePath(name);
            var pathParts = path.Split('/');
            var segments = pathParts.SkipLast(1).ToList();
            var fileName = pathParts.Last();
            var targetFolderId = settings.BaseFolderId;
            var level = 0;

            // build tree if not exists
            while (level < segments.Count)
            {
                var segment = segments[level];

                var folder = list!.FirstOrDefault(f => f.Isfolder && 
                    string.Equals(f.Name, segment, StringComparison.OrdinalIgnoreCase) &&
                    f.Parentfolderid == targetFolderId);

                if (folder == null)
                {
                    var folderResponse = api.CreateFolderIfNotExists(targetFolderId.ToString(), segment, accessToken!).Result;
                    if (folderResponse.Result != 0)
                        throw new PhysException($"pcloud returned {folderResponse.Result} {folderResponse.Error} for CreateFolderIfNotExists");

                    list!.Add(folderResponse.Metadata);
                    targetFolderId = folderResponse.Metadata.Folderid;
                }
                else
                    targetFolderId = folder.Folderid;

                level++;
            }

            var response = api.UploadFile(targetFolderId, accessToken!, fileName, data).Result;
            if (response.Result != 0)
                throw new PhysException($"pcloud returned {response.Result} {response.Error} for upload");

            var metadata = response.Metadata.First();
            list!.Add(metadata);
            return Map(metadata);
        }

        private string NormilizePath(string path)
        {
            return Regex.Replace(path.Replace("\\", "/").Trim('/'), "/+", "/");
        }

        private void EnsureLoggedIn()
        {
            if (accessToken == null)
            {
                var response = api.GetUserInfo(settings.Username, settings.Password).Result;
                if (response.Result != 0)
                    throw new PhysException($"pcloud returned {response.Result} {response.Error} for login");

                accessToken = response.Auth;
                log.LogInformation($"logged in");
            }
        }

        private void EnsureList()
        {
            EnsureLoggedIn();

            if (list == null)
            {
                var response = api.ListFolder(settings.BaseFolderId, accessToken!).Result;
                if (response.Result != 0)
                    throw new PhysException($"pcloud returned {response.Result} {response.Error} for list folder {settings.BaseFolderId}");

                list = Flatten(response.Metadata);
                log.LogInformation($"updated list");
            }
        }

        private StorageFileInfo Map(MetadataResponse metadata)
        {
            return new StorageFileInfo
            {
                Id = metadata.Fileid.ToString(),
                Name = GetPath(metadata),
                Size = metadata.Size,
                Updated = DateTimeUtils.UnixSecondsToDateTime(metadata.Modified)
            };
        }

        private string GetPath(MetadataResponse metadata, string? path = null)
        {
            path = NormilizePath(path == null ? metadata.Name : Path.Combine(metadata.Name, path));
            if (metadata.Parentfolderid == settings.BaseFolderId)
                return path;

            var parent = list?.First(m => m.Folderid == metadata.Parentfolderid);
            return GetPath(parent!, path);
        }

        private List<MetadataResponse> Flatten(MetadataResponse parent, List<MetadataResponse>? list = null)
        {
            list ??= new List<MetadataResponse>();

            if (parent.Contents != null)
            {
                foreach (var child in parent.Contents)
                {
                    list.Add(child);
                    Flatten(child, list);
                }
            }

            return list;
        }
    }
}