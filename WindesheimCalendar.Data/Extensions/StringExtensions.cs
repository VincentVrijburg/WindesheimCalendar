using System;
using System.Collections.Generic;
using System.Text.Json;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using WindesheimCalendar.Data.Models;

namespace WindesheimCalendar.Data.Extensions
{
    public static class StringExtensions
    {
        public static string ToCalendarString(this string value)
        {
            var calendarItems = JsonSerializer.Deserialize<List<CalendarItem>>(value);

            var calendar = new Calendar
            {
                Method = "PUBLISH"
            };
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
