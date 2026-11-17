using Microsoft.AspNetCore.Http;
using Oqtane.Extensions;

namespace BCN.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public string UserName => _contextAccessor?.HttpContext?.User?.Username();

        public int? UserId => _contextAccessor?.HttpContext?.User?.UserId();
    }
}