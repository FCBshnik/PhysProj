using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Api.Admin.Api.Models;
using Phys.Lib.Core;
using Phys.Lib.Core.Authors;

namespace Phys.Lib.Api.Admin.Api.Authors
{
    public class AuthorsEndpoint
    {
        private static readonly AuthorsMapper mapper = new AuthorsMapper();

        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([FromQuery] string? search, [FromServices] IAuthorsSearch authors) =>
            {
                var items = authors.FindByText(search ?? ".");
                return Results.Ok(items.Select(mapper.Map));
            })
            .ProducesOk<List<AuthorModel>>()
            .WithName("ListAuthors");

            builder.MapGet("/{code}", (string code, [FromServices] IAuthorsSearch authors) =>
            {
                var author = authors.FindByCode(code);
                if (author == null)
                    return Results.BadRequest(ErrorModel.NotFound($"author '{code}' not found"));

                return Results.Ok(mapper.Map(author));
            })
            .ProducesResponse<AuthorModel>("GetAuthor");

            builder.MapPost("/", ([FromBody] AuthorCreateModel model, [FromServices] IAuthorsEditor authors) =>
            {
                var author = authors.Create(model.Code);
                return Results.Ok(mapper.Map(author));
            })
            .ProducesResponse<AuthorModel>("CreateAuthor");

            builder.MapPost("/{code}/lifetime", (string code, [FromBody]AuthorLifetimeUpdateModel model, [FromServices] IAuthorsSearch authorsSearch, [FromServices] IAuthorsEditor authorsEditor) =>
            {
                var author = authorsSearch.FindByCode(code);
                if (author == null)
                    return Results.BadRequest(ErrorModel.NotFound($"author '{code}' not found"));

                author = authorsEditor.UpdateLifetime(author, model.Born, model.Died);
                return Results.Ok(mapper.Map(author));
            })
            .ProducesResponse<AuthorModel>("UpdateAuthorLifetime");

            builder.MapDelete("/{code}", (string code, [FromServices] IAuthorsSearch authorsSearch, [FromServices] IAuthorsEditor authorsEditor) =>
            {
                var author = authorsSearch.FindByCode(code);
                if (author != null)
                    authorsEditor.Delete(author);

                return TypedResults.Ok(OkModel.Ok);
            })
            .ProducesError()
            .WithName("DeleteAuthor");

            builder.MapPost("/{code}/info/{language}", (string code, string language, [FromBody]AuthorInfoUpdateModel model, [FromServices] IAuthorsSearch authorsSearch, [FromServices] IAuthorsEditor authorsEditor) =>
            {
                var author = authorsSearch.FindByCode(code);
                if (author == null)
                    return ErrorModel.NotFound($"author '{code}' not found").ToResult();
                language = Language.NormalizeAndValidate(language);

                author = authorsEditor.UpdateInfo(author, new AuthorDbo.InfoDbo { Language = language, Name = model.Name, Description = model.Description });
                return Results.Ok(mapper.Map(author));
            })
            .ProducesResponse<AuthorModel>("UpdateAuthorInfo");

            builder.MapDelete("/{code}/info/{language}", (string code, string language, [FromServices] IAuthorsSearch authorsSearch, [FromServices] IAuthorsEditor authorsEditor) =>
            {
                var author = authorsSearch.FindByCode(code);
                if (author == null)
                    return Results.BadRequest(ErrorModel.NotFound($"author '{code}' not found"));

                author = authorsEditor.DeleteInfo(author, language);
                return Results.Ok(mapper.Map(author));
            })
            .ProducesResponse<AuthorModel>("DeleteAuthorInfo");
        }
    }
}
