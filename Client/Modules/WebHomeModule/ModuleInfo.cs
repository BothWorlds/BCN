using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.WebHomeModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "WebHomeModule",
            Description = "Home module",
            Version = "1.0.1",
            PackageName = "BCN",
            ReleaseVersions = "1.0.0,1.0.1",
            ServerManagerType = "BCN.Manager.ApplicationModuleManager, BCN.Server.Oqtane",
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
                    Path = "home",
                    Name = "Home",
                    ThemeType = "BCN.WebTheme.Theme, BCN.Client.Oqtane",
                    PageTemplateModules = new List<PageTemplateModule>
                    {
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = null,
                            Pane = "Default",
                            ModuleDefinitionName = "BCN.WebHomeModule, BCN.Client.Oqtane",
                            PermissionList = new List<Permission>
                            {
                                new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                                new Permission(PermissionNames.View, RoleNames.Everyone, true)
                            }
                        },
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = "Welcome To Oqtane...",
                            Pane = "Default",
                            IsDeleted = true,
                            ModuleDefinitionName = "Oqtane.Modules.HtmlText, Oqtane.Client"
                        },
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = "MIT License",
                            Pane = "Default",
                            IsDeleted = true,
                            ModuleDefinitionName = "Oqtane.Modules.HtmlText, Oqtane.Client"
                        },
                        new PageTemplateModule
                        {
                            Header = null,
                            Title = "Secure Content",
                            Pane = "Default",
                            IsDeleted = true,
                            ModuleDefinitionName = "Oqtane.Modules.HtmlText, Oqtane.Client"
                        }
                    },
                    Update = true,
                    IsNavigation = true,
                    Order = 0,
                    PermissionList = new List<Permission>
                    {
                        new Permission(PermissionNames.View, RoleNames.Unauthenticated, true),
                        new Permission(PermissionNames.View, RoleNames.Everyone, true)
                    }
                },
                new PageTemplate
                {
                    AliasName = "*",
                    Path = "private",
                    Name = "Private",
                    IsDeleted = true,
                    Update = true
                },
                new PageTemplate
                {
                    AliasName = "*",
                    Path = "mypage",
                    Name = "My Page",
                    IsDeleted = true,
                    Update = true
                }
            }
        };
    }
}
