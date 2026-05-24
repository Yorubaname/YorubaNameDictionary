using Api.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YorubaOrganization.Core.Cache;

namespace Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class EtymologySyncController(
        IBackgroundJobClientV2 backgroundJobClient,
        NameEtymologyDefinitionsSyncJob nameSyncJob,
        WordEtymologyDefinitionsSyncJob wordSyncJob,
        ISimpleCache simpleCache) : ControllerBase
    {
        private readonly IBackgroundJobClientV2 _backgroundJobClient = backgroundJobClient;
        private readonly NameEtymologyDefinitionsSyncJob _nameSyncJob = nameSyncJob;
        private readonly WordEtymologyDefinitionsSyncJob _wordSyncJob = wordSyncJob;
        private readonly ISimpleCache _simpleCache = simpleCache;

        [HttpPost("api/v1/names/etymology-sync/run")]
        public Task<IActionResult> TriggerNamesEtymologyDefinitionsSync()
        {
            return TriggerAsync(_nameSyncJob.CheckpointPageKey, () => _backgroundJobClient.Enqueue(() => _nameSyncJob.ExecuteAsync()));
        }

        [HttpGet("api/v1/names/etymology-sync/status")]
        public Task<IActionResult> GetNamesEtymologyDefinitionsSyncStatus()
        {
            return GetStatusAsync(
                _nameSyncJob.CheckpointPageKey,
                _nameSyncJob.LastProcessedPageKey,
                _nameSyncJob.LastRunIdKey,
                _nameSyncJob.LastRunStatusKey,
                _nameSyncJob.LastRunStartedAtKey,
                _nameSyncJob.LastRunCompletedAtKey,
                _nameSyncJob.LastRunErrorKey);
        }

        [HttpPost("api/v1/words/etymology-sync/run")]
        public Task<IActionResult> TriggerWordsEtymologyDefinitionsSync()
        {
            return TriggerAsync(_wordSyncJob.CheckpointPageKey, () => _backgroundJobClient.Enqueue(() => _wordSyncJob.ExecuteAsync()));
        }

        [HttpGet("api/v1/words/etymology-sync/status")]
        public Task<IActionResult> GetWordsEtymologyDefinitionsSyncStatus()
        {
            return GetStatusAsync(
                _wordSyncJob.CheckpointPageKey,
                _wordSyncJob.LastProcessedPageKey,
                _wordSyncJob.LastRunIdKey,
                _wordSyncJob.LastRunStatusKey,
                _wordSyncJob.LastRunStartedAtKey,
                _wordSyncJob.LastRunCompletedAtKey,
                _wordSyncJob.LastRunErrorKey);
        }

        private async Task<IActionResult> TriggerAsync(string checkpointPageKey, Func<string> enqueueJob)
        {
            var checkpointPage = await _simpleCache.GetAsync<int>(checkpointPageKey);
            var jobId = enqueueJob();

            return Accepted(new
            {
                jobId,
                checkpointPage,
                nextPage = Math.Max(1, checkpointPage + 1)
            });
        }

        private async Task<IActionResult> GetStatusAsync(
            string checkpointPageKey,
            string lastProcessedPageKey,
            string lastRunIdKey,
            string lastRunStatusKey,
            string lastRunStartedAtKey,
            string lastRunCompletedAtKey,
            string lastRunErrorKey)
        {
            var checkpointPage = await _simpleCache.GetAsync<int>(checkpointPageKey);
            var lastProcessedPage = await _simpleCache.GetAsync<int>(lastProcessedPageKey);
            var lastRunId = await _simpleCache.GetAsync<string>(lastRunIdKey);
            var lastRunStatus = await _simpleCache.GetAsync<string>(lastRunStatusKey);
            var lastRunStartedAt = await _simpleCache.GetAsync<string>(lastRunStartedAtKey);
            var lastRunCompletedAt = await _simpleCache.GetAsync<string>(lastRunCompletedAtKey);
            var lastRunError = await _simpleCache.GetAsync<string>(lastRunErrorKey);

            return Ok(new
            {
                checkpointPage,
                lastProcessedPage,
                lastRunId,
                lastRunStatus,
                lastRunStartedAt,
                lastRunCompletedAt,
                lastRunError,
                nextPage = Math.Max(1, checkpointPage + 1)
            });
        }
    }
}