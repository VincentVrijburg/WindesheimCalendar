using System.Threading.Tasks;
using Microsoft.Build.Utilities;

namespace WindesheimCalendar.Functions.Clients
{
    public interface IWindesheimApiClient
    {
        Task<string> GetClassCalendarAsync(string classCode);
        Task<string> GetSubjectCalendarAsync(string subjectCode);
        Task<string> GetTeacherCalendarAsync(string teacherCode);
    }
}