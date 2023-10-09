using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Phys.Shared.Logging
{
    public sealed class HttpRequestsObserver : IDisposable, IObserver<DiagnosticListener>
    {
        private readonly ILoggerFactory loggerFactory;

        private IDisposable? subscription;

        public HttpRequestsObserver(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "HttpHandlerDiagnosticListener")
            {
                subscription = value.Subscribe(new HttpHandlerDiagnosticListener(loggerFactory.CreateLogger("http")));
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void Dispose()
        {
            subscription?.Dispose();
        }

        private sealed class HttpHandlerDiagnosticListener : IObserver<KeyValuePair<string, object?>>
        {
            private readonly ILogger log;

            public HttpHandlerDiagnosticListener(ILogger log)
            {
                this.log = log;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(KeyValuePair<string, object?> value)
            {
                switch (value.Key)
                {
                    case "System.Net.Http.HttpRequestOut.Start":
                        var type = value.Value!.GetType();
                        var request = type.GetProperty("Request")!.GetValue(value.Value) as HttpRequestMessage;
                        log.LogInformation("req {method} {path}", request!.Method.Method, request.RequestUri);
                        break;
                    case "System.Net.Http.HttpRequestOut.Stop":
                        type = value.Value!.GetType();
                        var response = type.GetProperty("Response")!.GetValue(value.Value) as HttpResponseMessage;
                        log.LogInformation("res {status} to {method} {path}", response!.StatusCode, response.RequestMessage?.Method.Method, response.RequestMessage?.RequestUri);
                        break;
                }
            }
        }
    }
}
