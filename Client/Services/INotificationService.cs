using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCN.Shared.Models.DataModels;
using Oqtane.Models;

namespace BCN.Client.Services
{
    public interface INotificationService
    {
        Task<GridResult<Notification>> GetNotificationsAsync(int siteId, NotificationFilterModel filter);

        Task<IEnumerable<Notification>> GetNotificationsAllAsync(int siteId);
    }
}