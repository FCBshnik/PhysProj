namespace Phys.Lib.Admin.Api.Api.Models
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

        public IResult ToResult()
        {
            return Results.BadRequest(this);
        }

        public static ErrorModel NotFound(string message)
        {
            return new ErrorModel(ErrorCode.NotFound, message);
        }

        public static ErrorModel InvalidArgument(string message)
        {
            return new ErrorModel(ErrorCode.InvalidArgument, message);
        }
    }
}
