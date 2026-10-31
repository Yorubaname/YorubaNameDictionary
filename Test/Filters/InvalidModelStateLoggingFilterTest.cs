using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Api.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Test.Filters
{
    public class InvalidModelStateLoggingFilterTest
    {
        [Fact]
        public async Task LogsRequestBody_WhenApiControllerRejectsInvalidModel()
        {
            var logs = new CapturingLoggerProvider();
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            builder.Logging.ClearProviders().AddProvider(logs);
            builder.Services.AddScoped<InvalidModelStateLoggingFilter>();
            builder.Services
                .AddControllers(options => options.Filters.AddService<InvalidModelStateLoggingFilter>(int.MinValue))
                .AddApplicationPart(typeof(ModelStateTestController).Assembly);

            await using var app = builder.Build();
            app.MapControllers();
            await app.StartAsync();

            var body = "{\"level\":\"NotANumber\"}";
            var response = await app.GetTestClient().PutAsync(
                "/model-state-test",
                new StringContent(body, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var logged = Assert.Single(logs.Messages, message => message.StartsWith("Invalid ModelState for PUT /model-state-test"));
            Assert.Contains("NotANumber", logged);
        }

        private sealed class CapturingLoggerProvider : ILoggerProvider
        {
            public ConcurrentQueue<string> Messages { get; } = new();

            public ILogger CreateLogger(string categoryName) => new CapturingLogger(Messages);

            public void Dispose()
            {
            }

            private sealed class CapturingLogger(ConcurrentQueue<string> messages) : ILogger
            {
                public System.IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

                public bool IsEnabled(LogLevel logLevel) => true;

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception? exception, System.Func<TState, System.Exception?, string> formatter) =>
                    messages.Enqueue(formatter(state, exception));
            }
        }
    }

    public class ModelStateTestPayload
    {
        public int Level { get; set; }
    }

    [ApiController]
    [Route("model-state-test")]
    public class ModelStateTestController : ControllerBase
    {
        [HttpPut]
        public IActionResult Put([FromBody] ModelStateTestPayload payload) => Ok();
    }
}
