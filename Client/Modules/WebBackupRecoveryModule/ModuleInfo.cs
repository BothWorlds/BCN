using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.WebBackupRecoveryModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "WebBackupRecoveryModule",
            Description = "Backup Recovery module",
            PackageName = "BCN",
            PermissionList = new List<Permission>
            {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
            },
            PageTemplates = new List<PageTemplate>
            {
                new PageTemplate
                {
                    AliasName = "blackcatnetworks.com",
                    Path = "backup-recovery",
                    Name = "Backup Recovery",
                    ThemeType = "BCN.WebTheme.Theme, BCN.Client.Oqtane",
                    PageTemplateModules = new List<PageTemplateModule>
                    {
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = null,
                            Pane = "Default",
                            ModuleDefinitionName = "BCN.WebBackupRecoveryModule, BCN.Client.Oqtane",
                            PermissionList = new List<Permission>
                            {
                                new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                                new Permission(PermissionNames.View, RoleNames.Everyone, true)
                            }
                        },
                    },
                    Update = true,
                    IsNavigation = false,
                    Order = 1,
                    PermissionList = new List<Permission>
                    {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
                    }
                },
            }
        };
    }
}
