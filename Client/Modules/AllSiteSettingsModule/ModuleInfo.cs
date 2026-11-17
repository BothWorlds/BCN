using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.AllSiteSettingsModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "AllSiteSettingsModule",
            Description = "All Site Settings Module",
            PackageName = "BCN",
            PageTemplates = new List<PageTemplate>
            {
                new PageTemplate
                {
                    AliasName = "*",
                    Path = "site-settings",
                    Name = "Site Settings",
                    PermissionList = new List<Permission>
                    {
                        new Permission(PermissionNames.View, RoleNames.Host, true)
                    },
                    PageTemplateModules = new List<PageTemplateModule>
                    {
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = null,
                            Pane = "Default",
                            ModuleDefinitionName = "BCN.AllSiteSettingsModule, BCN.Client.Oqtane"
                        }
                    },
                    Update = true,
                    IsNavigation = false
                }
            },
        };
    }
}
