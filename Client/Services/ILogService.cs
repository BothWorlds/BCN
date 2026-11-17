using System.Threading.Tasks;
using BCN.Shared.Models.DataModels;
using Oqtane.Models;

namespace BCN.Client.Services
{
    public interface ILogService
    {
        Task<GridResult<Log>> GetLogsAsync(int siteId, LogFilterModel filter);
    }
}