using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.WebContactUsModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "WebContactUsModule",
            Description = "WebContact Us Module",
            PackageName = "BCN",
            PageTemplates = new List<PageTemplate>
            {
                new PageTemplate
                {
                    AliasName = "blackcatnetworks.com",
                    Path = "contact-us",
                    Name = "Contact Us",
                    ThemeType = "BCN.WebTheme.Theme, BCN.Client.Oqtane",
                    PermissionList = new List<Permission>
                    {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
                    },
                    PageTemplateModules = new List<PageTemplateModule>
                    {
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = null,
                            Pane = "Default",
                            ModuleDefinitionName = "BCN.WebContactUsModule, BCN.Client.Oqtane",
                            PermissionList = new List<Permission>
                            {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
                            }
                        }
                    },
                    Update = true,
                    Order = 5,
                    IsNavigation = true
                },
            },
            Resources = new List<Resource>
            {
                new Stylesheet("~/Module.css")
            }
        };
    }
}