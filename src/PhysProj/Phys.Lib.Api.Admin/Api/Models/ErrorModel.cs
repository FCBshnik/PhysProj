namespace Phys.Lib.Api.Admin.Api.Models
{
    public record ErrorModel
    {
        public ErrorCode Code { get; set; }

        public string Message { get; set; }

        public ErrorModel(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public static ErrorModel NotFound(string message)
        {
            return new ErrorModel(ErrorCode.NotFound, message);
        }
    }
}
