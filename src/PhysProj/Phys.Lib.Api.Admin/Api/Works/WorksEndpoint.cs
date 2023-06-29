using Microsoft.AspNetCore.Mvc;
using Phys.Lib.Api.Admin.Api.Models;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Works;

namespace Phys.Lib.Api.Admin.Api.Works
{
    public static class WorksEndpoint
    {
        private static readonly WorksMapper mapper = new WorksMapper();

        public static void Map(RouteGroupBuilder builder)
        {
            builder.MapGet("/", ([FromServices] IWorksService service) =>
            {
                var works = service.Search(".");
                return Results.Ok(works.Select(mapper.Map));
            })
            .ProducesResponse<List<WorkModel>>("ListWorks");

            builder.MapGet("/{code}", (string code, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(code);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{code}' not found");

                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("GetWork");

            builder.MapPost("/", ([FromBody] WorkCreateModel model, [FromServices] IWorksService service) =>
            {
                var work = service.Create(model.Code);
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("CreateWork");

            builder.MapPost("/{code}", (string code, [FromBody] WorkUpdateModel model, [FromServices] IWorksService service) =>
            {
                var author = service.GetByCode(code);
                if (author == null)
                    return ErrorModel.NotFoundResult($"work '{code}' not found");

                author = service.Update(author, mapper.Map(model));
                return Results.Ok(mapper.Map(author));
            })
            .ProducesResponse<WorkModel>("UpdateWork");

            builder.MapDelete("/{code}", (string code, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(code);
                if (work != null)
                    service.Delete(work);

                return TypedResults.Ok(OkModel.Ok);
            })
            .ProducesError()
            .WithName("DeleteWork");

            builder.MapPost("/{code}/info/{language}", (string code, string language, [FromBody] WorkInfoUpdateModel model, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(code);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{code}' not found");

                work = service.Update(work, new WorkUpdate { AddInfo = mapper.Map(model, language) });
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UpdateWorkInfo");

            builder.MapDelete("/{code}/info/{language}", (string code, string language, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(code);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{code}' not found");

                work = service.Update(work, new WorkUpdate { DeleteInfo = language });
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("DeleteWorkInfo");

            builder.MapPost("/{code}/authors/{authorCode}", (string code, string authorCode, [FromServices] IWorksService service, [FromServices] IAuthorsService authorsService) =>
            {
                var work = service.GetByCode(code);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{code}' not found");
                var author = authorsService.GetByCode(authorCode);
                if (author == null)
                    return ErrorModel.NotFoundResult($"author '{code}' not found");

                work = service.Update(work, new WorkUpdate { AddAuthor = author });
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("LinkAuthorToWork");

            builder.MapDelete("/{code}/authors/{authorCode}", (string code, string authorCode, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(code);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{code}' not found");

                work = service.Update(work, new WorkUpdate { DeleteAuthor = authorCode });
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UnlinkAuthorFromWork");

            builder.MapPost("/{code}/original/{originalCode}", (string code, string originalCode, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(code);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{code}' not found");
                var original = service.GetByCode(originalCode);
                if (original == null)
                    return ErrorModel.NotFoundResult($"original work '{code}' not found");

                work = service.Update(work, new WorkUpdate { Original = original });
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("LinkWorkToOriginal");

            builder.MapDelete("/{code}/original", (string code, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(code);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{code}' not found");

                work = service.Update(work, new WorkUpdate { Original = WorkDbo.None });
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UnlinkWorkFromOriginal");

            builder.MapPost("/{collectedWorkCode}/works/{subWorkCode}", (string collectedWorkCode, string subWorkCode, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(collectedWorkCode);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{collectedWorkCode}' not found");
                var subWork = service.GetByCode(subWorkCode);
                if (subWork == null)
                    return ErrorModel.NotFoundResult($"work '{collectedWorkCode}' not found");

                work = service.Update(work, new WorkUpdate { AddWork = subWork });
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("LinkWorkToCollectedWork");

            builder.MapDelete("/{collectedWorkCode}/works/{subWorkCode}", (string collectedWorkCode, string subWorkCode, [FromServices] IWorksService service) =>
            {
                var work = service.GetByCode(collectedWorkCode);
                if (work == null)
                    return ErrorModel.NotFoundResult($"work '{collectedWorkCode}' not found");

                work = service.Update(work, new WorkUpdate { DeleteWork = subWorkCode });
                return Results.Ok(mapper.Map(work));
            })
            .ProducesResponse<WorkModel>("UnlinkWorkFromCollectedWork");
        }
    }
}
