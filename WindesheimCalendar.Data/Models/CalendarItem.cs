using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WindesheimCalendar.Data.Models
{
    public class CalendarItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("lokaal")]
        public string Lokaal { get; set; }

        [JsonPropertyName("starttijd")]
        public long Starttijd { get; set; }

        [JsonPropertyName("eindtijd")]
        public long Eindtijd { get; set; }

        [JsonPropertyName("changed")]
        public bool Changed { get; set; }

        [JsonPropertyName("docentcode")]
        public object Docentcode { get; set; }

        [JsonPropertyName("roosterdatum")]
        public DateTime Roosterdatum { get; set; }

        [JsonPropertyName("commentaar")]
        public string Commentaar { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("groepcode")]
        public string Groepcode { get; set; }

        [JsonPropertyName("vaknaam")]
        public string Vaknaam { get; set; }

        [JsonPropertyName("vakcode")]
        public string Vakcode { get; set; }

        [JsonPropertyName("docentnamen")]
        public List<string> Docentnamen { get; set; }
    }
}
