using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.AllNotificationViewModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "AllNotificationViewModule",
            Description = "All Notification View module",
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
                    Path = "notification-view",
                    Name = "Notification View",
                    PageTemplateModules = new List<PageTemplateModule>
                    {
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = null,
                            Pane = "Default",
                            ModuleDefinitionName = "BCN.AllNotificationViewModule, BCN.Client.Oqtane",
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
