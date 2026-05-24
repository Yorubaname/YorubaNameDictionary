using System.Diagnostics;
using Api.Configuration;
using Application.Services.Names;
using Hangfire;
using Microsoft.Extensions.Options;
using YorubaOrganization.Core.Cache;

namespace Api.Jobs
{
    public class NameEtymologyDefinitionsSyncJob(
        NameEtymologyDefinitionsSyncService syncService,
        ISimpleCache simpleCache,
        IOptions<NameEtymologyDefinitionsSyncConfig> config,
        ILogger<NameEtymologyDefinitionsSyncJob> logger)
    {
        private readonly NameEtymologyDefinitionsSyncService _syncService = syncService;
        private readonly ISimpleCache _simpleCache = simpleCache;
        private readonly NameEtymologyDefinitionsSyncConfig _config = config.Value;
        private readonly ILogger<NameEtymologyDefinitionsSyncJob> _logger = logger;

        public string CheckpointPageKey => BuildCacheKey("checkpoint-page");
        public string LastRunIdKey => BuildCacheKey("last-run-id");
        public string LastRunStatusKey => BuildCacheKey("last-run-status");
        public string LastRunStartedAtKey => BuildCacheKey("last-run-started-at");
        public string LastRunCompletedAtKey => BuildCacheKey("last-run-completed-at");
        public string LastRunErrorKey => BuildCacheKey("last-run-error");
        public string LastProcessedPageKey => BuildCacheKey("last-processed-page");

        [AutomaticRetry(Attempts = 2)]
        public async Task ExecuteAsync()
        {
            var runId = Guid.NewGuid().ToString("N");
            var overallStopwatch = Stopwatch.StartNew();
            var checkpointPage = await _simpleCache.GetAsync<int>(CheckpointPageKey);
            var startPage = Math.Max(1, checkpointPage + 1);

            await _simpleCache.SetAsync(LastRunIdKey, runId);
            await _simpleCache.SetAsync(LastRunStatusKey, "running");
            await _simpleCache.SetAsync(LastRunStartedAtKey, DateTimeOffset.UtcNow.ToString("O"));
            await _simpleCache.SetAsync(LastRunErrorKey, string.Empty);

            if (checkpointPage > 0)
            {
                _logger.LogInformation(
                    "NameEtymologyDefinitionsSync.JobResumedFromCheckpoint runId={RunId} startPage={StartPage} checkpointPage={CheckpointPage} pageSize={PageSize}",
                    runId,
                    startPage,
                    checkpointPage,
                    _config.PageSize);
            }
            else
            {
                _logger.LogInformation(
                    "NameEtymologyDefinitionsSync.JobStarted runId={RunId} startPage={StartPage} pageSize={PageSize}",
                    runId,
                    startPage,
                    _config.PageSize);
            }

            try
            {
                var processedPages = 0;

                for (var page = startPage; page < startPage + _config.MaxPagesPerRun; page++)
                {
                    processedPages++;
                    var pageStopwatch = Stopwatch.StartNew();

                    _logger.LogInformation(
                        "NameEtymologyDefinitionsSync.PageStarted runId={RunId} currentPage={CurrentPage} pageSize={PageSize}",
                        runId,
                        page,
                        _config.PageSize);

                    var pageResult = await _syncService.ProcessPageAsync(
                        page,
                        _config.PageSize,
                        runId,
                        _config.WordLogSampleSize);

                    var pageDurationMs = pageStopwatch.ElapsedMilliseconds;

                    if (pageResult.NamesRead == 0)
                    {
                        _logger.LogInformation(
                            "NameEtymologyDefinitionsSync.JobCompleted runId={RunId} reason=no-more-records processedPages={ProcessedPages} totalDurationMs={TotalDurationMs}",
                            runId,
                            processedPages - 1,
                            overallStopwatch.ElapsedMilliseconds);
                        await _simpleCache.SetAsync(LastRunStatusKey, "completed");
                        await _simpleCache.SetAsync(LastRunCompletedAtKey, DateTimeOffset.UtcNow.ToString("O"));
                        return;
                    }

                    _logger.LogInformation(
                        "NameEtymologyDefinitionsSync.PageCompleted runId={RunId} currentPage={CurrentPage} namesRead={NamesRead} etymologyRowsRead={EtymologyRowsRead} distinctParts={DistinctParts} distinctPartMeaningPairs={DistinctPartMeaningPairs} wordsCreated={WordsCreated} definitionsAdded={DefinitionsAdded} duplicatesSkipped={DuplicatesSkipped} partiallyCreated={PartiallyCreated} noValidDefinitions={NoValidDefinitions} pageDurationMs={PageDurationMs}",
                        runId,
                        pageResult.Page,
                        pageResult.NamesRead,
                        pageResult.EtymologyRowsRead,
                        pageResult.DistinctParts,
                        pageResult.DistinctPartMeaningPairs,
                        pageResult.CreatedWords,
                        pageResult.DefinitionsAdded,
                        pageResult.SkippedDuplicates,
                        pageResult.PartiallyCreated,
                        pageResult.NoValidDefinitions,
                        pageDurationMs);

                    await _simpleCache.SetAsync(CheckpointPageKey, pageResult.Page);
                    await _simpleCache.SetAsync(LastProcessedPageKey, pageResult.Page);

                    _logger.LogInformation(
                        "NameEtymologyDefinitionsSync.CheckpointSaved runId={RunId} currentPage={CurrentPage} checkpointWritten=true",
                        runId,
                        pageResult.Page);

                    if (pageResult.NamesRead < _config.PageSize)
                    {
                        _logger.LogInformation(
                            "NameEtymologyDefinitionsSync.JobCompleted runId={RunId} reason=last-page-detected processedPages={ProcessedPages} totalDurationMs={TotalDurationMs}",
                            runId,
                            processedPages,
                            overallStopwatch.ElapsedMilliseconds);
                        await _simpleCache.SetAsync(LastRunStatusKey, "completed");
                        await _simpleCache.SetAsync(LastRunCompletedAtKey, DateTimeOffset.UtcNow.ToString("O"));
                        return;
                    }
                }

                _logger.LogWarning(
                    "NameEtymologyDefinitionsSync.JobCompleted runId={RunId} reason=max-pages-cap-reached maxPagesPerRun={MaxPagesPerRun} totalDurationMs={TotalDurationMs}",
                    runId,
                    _config.MaxPagesPerRun,
                    overallStopwatch.ElapsedMilliseconds);

                await _simpleCache.SetAsync(LastRunStatusKey, "max-pages-cap-reached");
                await _simpleCache.SetAsync(LastRunCompletedAtKey, DateTimeOffset.UtcNow.ToString("O"));
            }
            catch (Exception ex)
            {
                await _simpleCache.SetAsync(LastRunStatusKey, "failed");
                await _simpleCache.SetAsync(LastRunCompletedAtKey, DateTimeOffset.UtcNow.ToString("O"));
                await _simpleCache.SetAsync(LastRunErrorKey, ex.Message);

                _logger.LogError(
                    ex,
                    "NameEtymologyDefinitionsSync.JobFailed runId={RunId} startPage={StartPage} failureType={FailureType} exceptionMessage={ExceptionMessage} totalDurationMs={TotalDurationMs}",
                    runId,
                    startPage,
                    ex.GetType().Name,
                    ex.Message,
                    overallStopwatch.ElapsedMilliseconds);

                throw;
            }
        }

        private string BuildCacheKey(string suffix)
        {
            return $"{_config.CheckpointKeyPrefix}:{suffix}";
        }
    }
}
