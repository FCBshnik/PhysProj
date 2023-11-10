namespace Phys.Lib.Site.Api.Controllers
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

        public IResult ToResult()
        {
            return Results.BadRequest(this);
        }

        public static ErrorModel NotFound(string message)
        {
            return new ErrorModel(ErrorCode.NotFound, message);
        }
    }
}
