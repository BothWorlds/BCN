using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCN.Client.Services;
using BCN.Server.Repository;
using BCN.Shared.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Oqtane.Models;

namespace BCN.Server.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDbContextFactory<MainContext> _dbContextFactory;

        public NotificationService(IDbContextFactory<MainContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<IEnumerable<Notification>> GetNotificationsAllAsync(int siteId)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            IQueryable<Notification> query = db.Notifications
                .Where(x => x.SiteId == siteId);

            var data = await query
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

            return data;
        }

        public async Task<GridResult<Notification>> GetNotificationsAsync(int siteId, NotificationFilterModel filter)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            IQueryable<Notification> query = db.Notifications
                .Where(x => x.SiteId == siteId);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(x =>
                    x.FromEmail.Contains(filter.SearchTerm) ||
                    x.ToEmail.Contains(filter.SearchTerm) ||
                    x.Subject.Contains(filter.SearchTerm));
            }

            if (filter.CreatedOn.HasValue)
            {
                var date = filter.CreatedOn.Value.Date;

                query = query.Where(x =>
                    x.CreatedOn >= date &&
                    x.CreatedOn < date.AddDays(1));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedOn)
                .Skip(filter.PageNumber * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new GridResult<Notification>
            {
                Data = data,
                Total = total
            };
        }
    }
}