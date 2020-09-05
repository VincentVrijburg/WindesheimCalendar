using System.Net.Http;
using System.Threading.Tasks;

namespace WindesheimCalendar.Functions.Clients
{
    public class WindesheimApiClient : IWindesheimApiClient
    {
        private readonly HttpClient _client;

        public WindesheimApiClient(HttpClient client)
        {
            _client = client;
        }

        public Task<string> GetClassCalendarAsync(string classCode)
        {
            return _client.GetStringAsync($"klas/{classCode}/les?$orderby=starttijd&_=1577836800");
        }

        public Task<string> GetSubjectCalendarAsync(string subjectCode)
        {
            return _client.GetStringAsync($"klas/{subjectCode}/les?$orderby=starttijd&_=1577836800");
        }

        public Task<string> GetTeacherCalendarAsync(string teacherCode)
        {
            return _client.GetStringAsync($"klas/{teacherCode}/les?$orderby=starttijd&_=1577836800");
        }
    }
}