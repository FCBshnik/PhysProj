using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core;
using Phys.Lib.Core.Authors;
using Phys.Lib.Admin.Api.Api;
using Phys.Lib.Admin.Api.Api.Authors;
using Phys.Lib.Admin.Api.Api.Models;

namespace Phys.Lib.Admin.Api.Api.Authors
{
    public static class AuthorsEndpoint
    {
        private static readonly AuthorsMapper mapper = new();

        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([AsParameters] AuthorsQuery query, [FromServices] IAuthorsSearch authors) =>
            {
                var items = authors.Find(query.Search);
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

                author = authorsEditor.UpdateInfo(author, new AuthorDbo.InfoDbo { Language = language, FullName = model.FullName, Description = model.Description });
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
