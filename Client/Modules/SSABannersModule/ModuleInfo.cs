using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.SSABannersModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "SSABannersModule",
            Description = "Banners and Displays module",
            PackageName = "BCN",
            PermissionList = new List<Permission>
            {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
            }
        };
    }
}
