using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.SSAContactUsModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "SSAContactUsModule",
            Description = "SSA Contact Us Module",
            PackageName = "BCN",
            PageTemplates = new List<PageTemplate>
            {
                new PageTemplate
                {
                    AliasName = "severancesignart.com",
                    Path = "contact-us",
                    Name = "Contact Us",
                    ThemeType = "BCN.SSATheme.Theme, BCN.Client.Oqtane",
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
                            ModuleDefinitionName = "BCN.SSAContactUsModule, BCN.Client.Oqtane",
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