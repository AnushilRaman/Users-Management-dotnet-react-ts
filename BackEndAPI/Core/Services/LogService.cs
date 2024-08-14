using BackEndAPI.Core.DbContext;
using BackEndAPI.Core.Dtos.Log;
using BackEndAPI.Core.Entities;
using BackEndAPI.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackEndAPI.Core.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveNewLog(string userName, string Description)
        {
            var newLog = new Log()
            {
                UserName = userName,
                Description = Description
            };
            await _context.Logs.AddAsync(newLog);
            await _context.SaveChangesAsync();

        }
        public async Task<IEnumerable<GetLogDto>> GetLogAsync()
        {
            var logs = await _context.Logs.Select(x => new GetLogDto()
            {
                UserName = x.UserName,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
            }).OrderByDescending(q => q.CreatedAt).ToListAsync();

            return logs;
        }

        public async Task<IEnumerable<GetLogDto>> GetMyLogsAsync(ClaimsPrincipal user)
        {
            var logs = await _context.Logs.Where(x => x.UserName == user.Identity.Name)
                .Select(x => new GetLogDto()
                {
                    UserName = x.UserName,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                }).OrderByDescending(q => q.CreatedAt).ToListAsync();

            return logs;
        }


    }
}
