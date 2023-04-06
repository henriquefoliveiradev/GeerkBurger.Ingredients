using GeekBurger.Ingredients.Api.Events.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Events
{
    public class Event
    {
        public void TraceEvent(string messageBody)
        {
            var context = new Dictionary<string, string>
            {
                { "message", messageBody }
            };

            var telemetryClient = new TelemetryClient();
            telemetryClient.TrackEvent("LabelReceived", context);
        }

        public Task ExceptionHandler(ExceptionReceivedEventArgs arg)
        {
            var context = new Dictionary<string, string>
            {
                { "Endpoint", arg.ExceptionReceivedContext.Endpoint },
                { "Path", arg.ExceptionReceivedContext.EntityPath },
                { "Action", arg.ExceptionReceivedContext.Action }
            };

            var telemetry = new TelemetryClient();
            telemetry.TrackException(arg.Exception);

            return Task.CompletedTask;
        }
    }
}
