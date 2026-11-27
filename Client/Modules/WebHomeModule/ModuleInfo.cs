using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.WebHomeModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "WebHomeModule",
            Description = "Home module",
            Version = "1.0.1",
            PackageName = "BCN",
            ReleaseVersions = "1.0.0,1.0.1",
            ServerManagerType = "BCN.Manager.ApplicationModuleManager, BCN.Server.Oqtane",
            PermissionList = new List<Permission>
            {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
            }
        };
    }
}
