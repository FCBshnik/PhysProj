using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultFilePao : IPublicApiObject
    {
        public required string Code { get; set; }

        public required string Format { get; set; }

        public required long Size { get; set; }

        public static SearchResultFilePao Map(FileDbo file)
        {
            return new SearchResultFilePao
            {
                Code = file.Code,
                Format = file.Format,
                Size = file.Size,
            };
        }
    }
}
