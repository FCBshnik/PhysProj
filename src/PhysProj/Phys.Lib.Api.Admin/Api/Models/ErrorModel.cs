namespace Phys.Lib.Api.Admin.Api.Models
{
    public record ErrorModel
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public ErrorModel(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
