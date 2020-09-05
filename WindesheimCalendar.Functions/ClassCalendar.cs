using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WindesheimCalendar.Data.Extensions;
using WindesheimCalendar.Functions.Clients;

namespace WindesheimCalendar.Functions
{
    public class ClassCalendar
    {
        private readonly IWindesheimApiClient _windesheimApiClient;
        private readonly CalendarSerializer _calendarSerializer = new CalendarSerializer();

        public ClassCalendar(IWindesheimApiClient windesheimApiClient)
        {
            _windesheimApiClient = windesheimApiClient ?? throw new ArgumentNullException(nameof(windesheimApiClient));
        }

        [FunctionName("ClassCalendar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "class/{classCode}/ics")] HttpRequest req, 
            string classCode,
            ILogger log)
        {
            Calendar templateCalendar;

            var latestCalendarAsync = _windesheimApiClient.GetClassCalendarAsync(classCode);

            using (var sr = new StreamReader($"Templates/{classCode}.ics"))
            {
                var content = await sr.ReadToEndAsync();
                templateCalendar = Calendar.Load(content);
            }

            var latestCalendar = await latestCalendarAsync;

            if (string.IsNullOrEmpty(latestCalendar))
                return new NotFoundResult();

            var newCalendar = Calendar.Load(latestCalendar.ToCalendarString());

            var tempEvents = new Dictionary<string, CalendarEvent>();
            foreach (var templateCalendarEvent in templateCalendar.Events)
            {
                var newCalendarEvent = newCalendar.Events
                    .FirstOrDefault(e => 
                        e.Summary == templateCalendarEvent.Summary &&
                        Equals(e.DtStart.AsUtc, templateCalendarEvent.DtStart.AsUtc) && 
                        e.Location == templateCalendarEvent.Location);

                if (newCalendarEvent != null)
                {
                    tempEvents.Add(templateCalendarEvent.Uid, newCalendarEvent);
                }
            }

            foreach (var (key, value) in tempEvents)
            {
                templateCalendar.Events[key] = value;
            }

            return new FileContentResult(Encoding.UTF8.GetBytes(_calendarSerializer.SerializeToString(templateCalendar)), "text/calendar")
            {
                FileDownloadName = $"Windesheim-{classCode}.ics"
            };
        }
    }
}
