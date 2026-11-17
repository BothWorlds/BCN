using System.Linq;
using System.Threading.Tasks;
using BCN.Client.Services;
using BCN.Server.Repository;
using BCN.Shared.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Oqtane.Models;

namespace BCN.Server.Services
{
    public class LogService : ILogService
    {
        private readonly IDbContextFactory<MainContext> _dbContextFactory;

        public LogService(IDbContextFactory<MainContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<GridResult<Log>> GetLogsAllAsync(int siteId)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            IQueryable<Log> query = db.Logs
                .Where(x => x.SiteId == siteId);
            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.LogDate)
                .ToListAsync();

            return new GridResult<Log>
            {
                Data = data,
                Total = total
            };
        }

        public async Task<GridResult<Log>> GetLogsAsync(int siteId, LogFilterModel filter)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            IQueryable<Log> query = db.Logs
                .Where(x => x.SiteId == siteId);

            if (!string.IsNullOrWhiteSpace(filter.Level))
            {
                query = query.Where(x => x.Level == filter.Level);
            }

            if (!string.IsNullOrWhiteSpace(filter.Feature))
            {
                query = query.Where(x => x.Feature == filter.Feature);
            }

            if (!string.IsNullOrWhiteSpace(filter.Function))
            {
                query = query.Where(x => x.Function == filter.Function);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(x =>
                    x.Level.Contains(filter.SearchTerm) ||
                    x.Feature.Contains(filter.SearchTerm) ||
                    x.Function.Contains(filter.SearchTerm));
            }

            if (filter.LogDate.HasValue)
            {
                var date = filter.LogDate.Value.Date;

                query = query.Where(x =>
                    x.LogDate >= date &&
                    x.LogDate < date.AddDays(1));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.LogDate)
                .Skip(filter.PageNumber * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new GridResult<Log>
            {
                Data = data,
                Total = total
            };
        }
    }
}