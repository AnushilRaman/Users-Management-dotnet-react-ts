using BackEndAPI.Core.Dtos.Log;
using System.Security.Claims;

namespace BackEndAPI.Core.Interfaces
{
    public interface ILogService
    {
        Task SaveNewLog(string userName, string Description);
        Task<IEnumerable<GetLogDto>> GetLogAsync();
        Task<IEnumerable<GetLogDto>> GetMyLogsAsync(ClaimsPrincipal user);
    }
}
