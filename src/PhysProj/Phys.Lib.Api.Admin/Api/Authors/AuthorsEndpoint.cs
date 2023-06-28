using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Api.Admin.Api.Models;
using Phys.Lib.Core.Authors;

namespace Phys.Lib.Api.Admin.Api.Authors
{
    public class AuthorsEndpoint
    {
        private static readonly AuthorsMapper mapper = new AuthorsMapper();

        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([FromServices] IAuthorsService authors) =>
            {
                var items = authors.Search(".");
                return Results.Ok(items.Select(mapper.Map));
            })
            .ProducesOk<List<AuthorModel>>()
            .WithName("ListAuthors");

            builder.MapGet("/{code}", (string code, [FromServices] IAuthorsService authors) =>
            {
                var author = authors.GetByCode(code);
                if (author == null)
                    return Results.BadRequest(ErrorModel.NotFound($"author '{code}' not found"));

                return Results.Ok(mapper.Map(author));
            })
            .ProducesOk<AuthorModel>()
            .ProducesError()
            .WithName("GetAuthor");

            builder.MapPost("/", ([FromBody] AuthorCreateModel model, [FromServices] IAuthorsService authors) =>
            {
                var author = authors.Create(model.Code);
                return Results.Ok(mapper.Map(author));
            })
            .ProducesOk<AuthorModel>()
            .ProducesError()
            .WithName("CreateAuthor");

            builder.MapPost("/{code}", (string code, [FromBody]AuthorUpdateModel model, [FromServices] IAuthorsService authors) =>
            {
                var author = authors.GetByCode(code);
                if (author == null)
                    return Results.BadRequest(ErrorModel.NotFound($"author '{code}' not found"));

                author = authors.Update(author, new AuthorUpdate { Born = model.Born, Died = model.Died });
                return Results.Ok(mapper.Map(author));
            })
            .ProducesOk<AuthorModel>()
            .ProducesError()
            .WithName("UpdateAuthor");

            builder.MapDelete("/{code}", (string code, [FromServices] IAuthorsService authors) =>
            {
                var author = authors.GetByCode(code);
                if (author != null)
                    authors.Delete(author);

                return TypedResults.Ok(OkModel.Ok);
            })
            .ProducesError()
            .WithName("DeleteAuthor");

            builder.MapPost("/{code}/info/{language}", (string code, string language, [FromBody]AuthorInfoUpdateModel model, [FromServices] IAuthorsService authors) =>
            {
                var author = authors.GetByCode(code);
                if (author == null)
                    return Results.BadRequest(ErrorModel.NotFound($"author '{code}' not found"));

                author = authors.Update(author, new AuthorUpdate { AddInfo = new AuthorDbo.InfoDbo { Language = language, Name = model.Name, Description = model.Description } });
                return Results.Ok(mapper.Map(author));
            })
            .ProducesOk<AuthorModel>()
            .ProducesError()
            .WithName("UpdateAuthorInfo");

            builder.MapDelete("/{code}/info/{language}", (string code, string language, [FromServices] IAuthorsService authors) =>
            {
                var author = authors.GetByCode(code);
                if (author == null)
                    return Results.BadRequest(ErrorModel.NotFound($"author '{code}' not found"));

                author = authors.Update(author, new AuthorUpdate { DeleteInfo = language });
                return Results.Ok(mapper.Map(author));
            })
            .ProducesOk<AuthorModel>()
            .ProducesError()
            .WithName("DeleteAuthorInfo");
        }
    }
}
