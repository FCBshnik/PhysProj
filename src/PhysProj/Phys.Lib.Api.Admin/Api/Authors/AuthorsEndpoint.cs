using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core;
using Phys.Lib.Core.Authors;

namespace Phys.Lib.Api.Admin.Api.Authors
{
    public class AuthorsEndpoint
    {
        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapPost("/", ([FromBody] AuthorCreateModel model, [FromServices] IAuthors authors) =>
            {
                var author = authors.Create(model.Code);
                return Results.Ok(author);
            })
            .ProducesOk<AuthorModel>()
            .ProducesError()
            .WithName("CreateAuthor");

            builder.MapGet("/", ([FromServices] IAuthors authors) =>
            {
                var items = authors.Search(".");
                return Results.Ok(items);
            })
            .ProducesOk<List<AuthorModel>>()
            .WithName("ListAuthors");
        }
    }
}
