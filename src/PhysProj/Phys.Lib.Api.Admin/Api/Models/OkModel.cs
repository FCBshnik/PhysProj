namespace Phys.Lib.Api.Admin.Api.Models
{
    public class OkModel
    {
        public static OkModel Ok => new OkModel { Time = DateTime.UtcNow };

        public DateTime Time { get; set; }
    }
}
