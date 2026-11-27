using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.AllLogViewModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "AllLogViewModule",
            Description = "All Log View module",
            PackageName = "BCN",
            PermissionList = new List<Permission>
            {
                        new Permission(PermissionNames.View, RoleNames.Admin, true),
                        new Permission(PermissionNames.View, RoleNames.Host, true)
            },
            PageTemplates = new List<PageTemplate>
            {
                new PageTemplate
                {
                    AliasName = "*",
                    Path = "log-view",
                    Name = "LogView",
                    PageTemplateModules = new List<PageTemplateModule>
                    {
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = null,
                            Pane = "Default",
                            ModuleDefinitionName = "BCN.AllLogViewModule, BCN.Client.Oqtane",
                            PermissionList = new List<Permission>
                            {
                                new Permission(PermissionNames.View, RoleNames.Admin, true),
                                new Permission(PermissionNames.View, RoleNames.Host, true)
                            }
                        },
                    },
                    Update = true,
                    IsNavigation = false,
                    PermissionList = new List<Permission>
                    {
                        new Permission(PermissionNames.View, RoleNames.Admin, true),
                        new Permission(PermissionNames.View, RoleNames.Host, true)
                    }
                },
            },
            Resources = new List<Resource>
            {
                new Stylesheet("~/Module.css")
            }
        };
    }
}
