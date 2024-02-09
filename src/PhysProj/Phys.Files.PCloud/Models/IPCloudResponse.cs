namespace Phys.Files.PCloud.Models
{
    internal interface IPCloudResponse
    {
        public int Result { get; }

        public string? Error { get; }
    }
}
