using Meilisearch;
using Phys.Shared;
using System.Diagnostics;
using System.Text.Json;

namespace Phys.Lib.Search.Meilisearch
{
    internal static class TaskUtils
    {
        public static void ThrowIfError(TaskInfo taskInfo)
        {
            if (taskInfo.Error?.Count > 0)
                throw new PhysException(JsonSerializer.Serialize(taskInfo.Error));
        }

        public static void ThrowIfError(TaskResource taskResource)
        {
            if (taskResource.Error?.Count > 0)
                throw new PhysException(JsonSerializer.Serialize(taskResource.Error));
        }

        public async static Task WaitToCompleteAsync(global::Meilisearch.Index index, TaskInfo taskInfo)
        {
            ThrowIfError(taskInfo);

            var sw = Stopwatch.StartNew();
            TaskResource taskResource;
            do
            {
                taskResource = await index.GetTaskAsync(taskInfo.TaskUid);
                ThrowIfError(taskResource);
                if (sw.Elapsed > TimeSpan.FromMinutes(1))
                    throw new PhysException($"timed out waiting task {taskResource.Uid} to complete");
                await Task.Delay(TimeSpan.FromSeconds(1));
            } while (!taskResource.FinishedAt.HasValue);
        }
    }
}
