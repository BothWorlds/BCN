using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.AllAdminActivityModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "AllAdminActivityModule",
            Description = "All Admin Activity Module",
            PackageName = "BCN",
            PageTemplates = new List<PageTemplate>
            {
                new PageTemplate
                {
                    AliasName = "*",
                    Path = "admin-activity",
                    Name = "Admin Activity",
                    ThemeType = "BCN.MyTheme.Theme, BCN.Client.Oqtane",
                    PermissionList = new List<Permission>
                    {
                        new Permission(PermissionNames.View, RoleNames.Admin, true),
                        new Permission(PermissionNames.View, RoleNames.Host, true)
                    },
                    PageTemplateModules = new List<PageTemplateModule>
                    {
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = null,
                            Pane = "Default",
                            ModuleDefinitionName = "BCN.AllAdminActivityModule, BCN.Client.Oqtane",
                            PermissionList = new List<Permission>
                            {
                                new Permission(PermissionNames.View, RoleNames.Admin, true),
                                new Permission(PermissionNames.View, RoleNames.Host, true)
                            },
                        }
                    },
                    Update = true,
                    Order = 6,
                    IsNavigation = true
                }
            },
            Resources = new List<Resource>
            {
                new Stylesheet("~/Module.css")
            }
        };
    }
}
