using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WindesheimCalendar.Data.Extensions;
using WindesheimCalendar.Functions.Clients;

namespace WindesheimCalendar.Functions
{
    public class SubjectCalendar
    {
        private readonly IWindesheimApiClient _windesheimApiClient;

        public SubjectCalendar(IWindesheimApiClient windesheimApiClient)
        {
            _windesheimApiClient = windesheimApiClient ?? throw new ArgumentNullException(nameof(windesheimApiClient));
        }

        [FunctionName("SubjectCalendar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "subject/{subjectCode}/ics")] HttpRequest req, 
            string subjectCode,
            ILogger log)
        {
            var result = await _windesheimApiClient.GetSubjectCalendarAsync(subjectCode);

            if (string.IsNullOrEmpty(result))
                return new NotFoundResult();

            var calendar = result.ToCalendarString();

            return new FileContentResult(Encoding.UTF8.GetBytes(calendar), "text/calendar")
            {
                FileDownloadName = $"Windesheim-{subjectCode}.ics"
            };
        }
    }
}
