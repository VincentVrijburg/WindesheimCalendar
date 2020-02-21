using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WindesheimCalendar.Api.Models;
using Ical.Net;
using Ical.Net.DataTypes;
using System.Text;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using Microsoft.Extensions.Options;
using WindesheimCalendar.Api.Models.Options;

namespace WindesheimCalendar.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public CalendarController(IOptions<Settings> options)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(options.Value.SourceUrl)
            };
        }

        [HttpGet("klas/{classCode}/ics")]
        public async Task<ActionResult> GetClassIcs([FromRoute] string classCode)
        {
            var response = await _httpClient.GetAsync($"klas/{classCode}/les");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var result = await response.Content.ReadAsStringAsync();
            var calendar = FormatCalendarString(result);

            return File(Encoding.UTF8.GetBytes(calendar), "text/calendar", $"Windesheim-{classCode}.ics");
        }

        [HttpGet("vak/{subjectCode}/ics")]
        public async Task<ActionResult> GetSubjectIcs([FromRoute] string subjectCode)
        {
            var response = await _httpClient.GetAsync($"vak/{subjectCode}/les");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var result = await response.Content.ReadAsStringAsync();
            var calendar = FormatCalendarString(result);

            return File(Encoding.UTF8.GetBytes(calendar), "text/calendar", $"Windesheim-{subjectCode}.ics");
        }

        [HttpGet("docent/{docentCode}/ics")]
        public async Task<ActionResult> GetTeacherIcs([FromRoute] string docentCode)
        {
            var response = await _httpClient.GetAsync($"docent/{docentCode}/les");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var result = await response.Content.ReadAsStringAsync();
            var calendar = FormatCalendarString(result);

            return File(Encoding.UTF8.GetBytes(calendar), "text/calendar", $"Windesheim-{docentCode}.ics");
        }

        private string FormatCalendarString(string json)
        {
            var calendarItems = JsonSerializer.Deserialize<List<CalendarItem>>(json);

            var calendar = new Calendar();
            calendar.Method = "PUBLISH";
            calendar.AddProperty("X-WR-CALNAME", "Unofficial Windesheim Calendar");
            calendar.AddTimeZone(new VTimeZone("Europe/Amsterdam"));

            foreach (var item in calendarItems)
            {
                var start = DateTimeOffset.FromUnixTimeMilliseconds(item.Starttijd).DateTime;
                var end = DateTimeOffset.FromUnixTimeMilliseconds(item.Eindtijd).DateTime;

                var calendarEvent = new CalendarEvent
                {
                    Uid = item.Id,
                    Start = new CalDateTime(start, "Europe/Amsterdam"),
                    End = new CalDateTime(end, "Europe/Amsterdam"),
                    Location = item.Lokaal,
                    Description = item.Commentaar,
                    Class = "Public",
                    Summary = item.Vaknaam,
                    Transparency = TransparencyType.Transparent
                };

                calendar.Events.Add(calendarEvent);
            }

            var serializer = new CalendarSerializer();

            return serializer.SerializeToString(calendar);
        }
    }
}
