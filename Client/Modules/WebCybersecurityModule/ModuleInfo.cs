using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.WebCybersecurityModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "WebCybersecurityModule",
            Description = "Cybersecurity module",
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
                    Path = "Cybersecurity",
                    Name = "Cybersecurity",
                    ThemeType = "BCN.WebTheme.Theme, BCN.Client.Oqtane",
                    PageTemplateModules = new List<PageTemplateModule>
                    {
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = null,
                            Pane = "Default",
                            ModuleDefinitionName = "BCN.WebCybersecurityModule, BCN.Client.Oqtane",
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
