using Phys.Files.PCloud.Models;
using Refit;

namespace Phys.Files.PCloud
{
    public interface IPCloudApiClient
    {
        [Get("/userinfo?getauth=1&timeformat=timestamp")]
        Task<UserInfoResponse> GetUserInfo(string username, string password);

        [Get("/listfolder?auth={auth}&path={path}&recursive=1&timeformat=timestamp")]
        Task<ResultMetadataResponse> ListFolder(string path, string auth);

        [Get("/listfolder?auth={auth}&folderid={folderid}&recursive=1&timeformat=timestamp")]
        Task<ResultMetadataResponse> ListFolder(long folderid, string auth);

        [Get("/createfolderifnotexists?auth={auth}&folderid={folderid}&name={name}&timeformat=timestamp")]
        Task<ResultMetadataResponse> CreateFolderIfNotExists(string folderid, string name, string auth);

        [Get("/getfilepublink?auth={auth}&fileid={fileid}&timeformat=timestamp")]
        Task<GetFilePubLinkResponse> GetFilePubLink(long fileid, string auth);

        [Get("/getpublinkdownload?code={code}&timeformat=timestamp")]
        Task<GetPubLinkDownloadResponse> GetPubLinkDownload(string code);

        [Get("/stat?auth={auth}&fileid={fileid}&timeformat=timestamp")]
        Task<ResultMetadataResponse> GetFileStat(long fileid, string auth);

        [Put("/uploadfile?auth={auth}&folderid={folderid}&filename={filename}&timeformat=timestamp")]
        Task<UploadFileResponse> UploadFile(long folderid, string auth, string filename, [Body]Stream content);
    }
}
