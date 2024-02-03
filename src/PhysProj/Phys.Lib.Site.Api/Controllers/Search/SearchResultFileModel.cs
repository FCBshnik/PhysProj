using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultFileModel
    {
        public required string Code { get; set; }

        public required string Format { get; set; }

        public required long Size { get; set; }

        public static SearchResultFileModel Map(FileDbo file)
        {
            return new SearchResultFileModel
            {
                Code = file.Code,
                Format = file.Format,
                Size = file.Size,
            };
        }
    }
}
