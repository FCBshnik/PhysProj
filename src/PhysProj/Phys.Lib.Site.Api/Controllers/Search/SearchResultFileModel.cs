using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Library.Models
{
    public class SearchResultFileModel
    {
        public required string Code { get; set; }

        public required string Format { get; set; }

        public required long Size { get; set; }
    }
}
