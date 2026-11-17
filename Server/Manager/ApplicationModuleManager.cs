using BCN.Server.Repository;
using BCN.Services;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Repository;

namespace BCN.Manager
{
    public class ApplicationModuleManager : MigratableModuleBase, IInstallable
    {
        private readonly IDBContextDependencies _dBContextDependencies;
        private readonly ICurrentUserService _currentUserService;
        public ApplicationModuleManager(IDBContextDependencies dBContextDependencies, ICurrentUserService currentUserService)
        {
            _dBContextDependencies = dBContextDependencies;
            _currentUserService = currentUserService;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new MainContext(_dBContextDependencies, _currentUserService), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new MainContext(_dBContextDependencies, _currentUserService), tenant, MigrationType.Down);
        }
    }
}
