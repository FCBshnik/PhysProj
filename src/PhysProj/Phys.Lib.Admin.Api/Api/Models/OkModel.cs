using Phys.Lib.Admin.Api;
using Phys.Lib.Admin.Api.Api.Models;

namespace Phys.Lib.Admin.Api.Api.Models
{
    public class OkModel
    {
        public static OkModel Ok => new OkModel();

        public DateTime Time { get; set; } = DateTime.UtcNow;

        public string Version { get; set; } = Program.Version;
    }
}
