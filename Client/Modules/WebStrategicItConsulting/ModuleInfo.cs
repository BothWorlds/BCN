using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.WebStrategicItConsultingModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "WebStrategicItConsultingModule",
            Description = "Strategic IT Consulting module",
            PackageName = "BCN",
            PermissionList = new List<Permission>
            {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
            }
        };
    }
}
