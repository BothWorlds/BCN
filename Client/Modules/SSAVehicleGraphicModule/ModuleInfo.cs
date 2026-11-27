using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.SSAVehicleGraphicModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "SSAVehicleGraphicModule",
            Description = "Vehicle Graphics module",
            PackageName = "BCN",
            PermissionList = new List<Permission>
            {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
            }
        };
    }
}
