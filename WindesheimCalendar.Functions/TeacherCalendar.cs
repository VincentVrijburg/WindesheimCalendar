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
    public class TeacherCalendar
    {
        private readonly IWindesheimApiClient _windesheimApiClient;

        public TeacherCalendar(IWindesheimApiClient windesheimApiClient)
        {
            _windesheimApiClient = windesheimApiClient ?? throw new ArgumentNullException(nameof(windesheimApiClient));
        }

        [FunctionName("TeacherCalendar")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "teacher/{teacherCode}/ics")] HttpRequest req, 
            string teacherCode,
            ILogger log)
        {
            var result = await _windesheimApiClient.GetTeacherCalendarAsync(teacherCode);

            if (string.IsNullOrEmpty(result))
                return new NotFoundResult();

            var calendar = result.ToCalendarString();

            return new FileContentResult(Encoding.UTF8.GetBytes(calendar), "text/calendar")
            {
                FileDownloadName = $"Windesheim-{teacherCode}.ics"
            };
        }
    }
}
