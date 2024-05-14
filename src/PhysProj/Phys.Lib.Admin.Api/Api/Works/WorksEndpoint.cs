using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Core;
using Phys.Lib.Core.Works;
using Phys.Lib.Admin.Api.Api.Models;

namespace Phys.Lib.Admin.Api.Api.Works
{
    public static class WorksEndpoint
    {
        private static readonly WorksMapper mapper = new WorksMapper();

        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([AsParameters] WorksQuery query, [FromServices] IWorksSearch search) =>
            {
                var works = search.Find(query.Search);
                return Results.Ok(works.Select(mapper.Map));
            })
            .ProducesResponse<List<WorkModel>>("ListWorks");

            builder.MapGet("/{code}", (string code, [FromServices] IWorksSearch search) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("GetWork");

            builder.MapPost("/", ([FromBody] WorkCreateModel model, [FromServices] IWorksEditor editor) =>
            {
                var work = editor.Create(model.Code);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("CreateWork");

            builder.MapPost("/{code}", (string code, [FromBody] WorkUpdateModel model, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                if (model.Date != null)
                    work = editor.UpdateDate(work, model.Date);
                if (model.Language != null)
                    work = editor.UpdateLanguage(work, model.Language);
                if (model.IsPublic != null)
                    work = editor.UpdateIsPublic(work, model.IsPublic.Value);

                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UpdateWork");

            builder.MapDelete("/{code}", (string code, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work != null)
                    editor.Delete(work);

                return TypedResults.Ok(OkModel.Ok);
            })
            .ProducesError()
            .WithName("DeleteWork");

            builder.MapPost("/{code}/info/{language}", (string code, string language, [FromBody] WorkInfoUpdateModel model, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();
                language = Language.NormalizeAndValidate(language);

                work = editor.AddInfo(work, mapper.Map(model, language));
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UpdateWorkInfo");

            builder.MapDelete("/{code}/info/{language}", (string code, string language, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                work = editor.DeleteInfo(work, language);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("DeleteWorkInfo");

            builder.MapPost("/{code}/authors/{authorCode}", (string code, string authorCode, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                work = editor.LinkAuthor(work, authorCode);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("LinkAuthorToWork");

            builder.MapDelete("/{code}/authors/{authorCode}", (string code, string authorCode, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                work = editor.UnlinkAuthor(work, authorCode);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UnlinkAuthorFromWork");

            builder.MapPost("/{collectedWorkCode}/works/{linkWorkCode}", (string collectedWorkCode, string linkWorkCode, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(collectedWorkCode);
                if (work == null)
                    return ErrorModel.NotFound($"work '{collectedWorkCode}' not found").ToResult();

                work = editor.LinkWork(work, linkWorkCode);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("LinkWorkToCollectedWork");

            builder.MapDelete("/{collectedWorkCode}/works/{linkWorkCode}", (string collectedWorkCode, string linkWorkCode, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(collectedWorkCode);
                if (work == null)
                    return ErrorModel.NotFound($"work '{collectedWorkCode}' not found").ToResult();

                work = editor.UnlinkWork(work, linkWorkCode);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UnlinkWorkFromCollectedWork");

            builder.MapPost("/{code}/sub-works-authors/{authorCode}", (string code, string authorCode, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                work = editor.LinkSubWorkAuthor(work, authorCode);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("LinkSubWorkAuthorToWork");

            builder.MapDelete("/{code}/sub-works-authors/{authorCode}", (string code, string authorCode, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                work = editor.UnlinkSubWorkAuthor(work, authorCode);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UnlinkSubWorkAuthorFromWork");

            builder.MapPost("/{code}/files/{fileCode}", (string code, string fileCode, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                work = editor.LinkFile(work, fileCode);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("LinkFileToWork");

            builder.MapDelete("/{code}/files/{fileCode}", (string code, string fileCode, [FromServices] IWorksSearch search, [FromServices] IWorksEditor editor) =>
            {
                var work = search.FindByCode(code);
                if (work == null)
                    return ErrorModel.NotFound($"work '{code}' not found").ToResult();

                work = editor.UnlinkFile(work, fileCode);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UnlinkFileFromWork");
        }
    }
}
