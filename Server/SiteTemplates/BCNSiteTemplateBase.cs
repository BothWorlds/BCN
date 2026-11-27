using System.Collections.Generic;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Shared;

namespace BCN.Server.SiteTemplates
{
    public abstract class BCNSiteTemplateBase
    {
        public abstract string SiteName { get; }

        public string Name => $"BCN - {SiteName} ({Prefix})";

        public abstract string Prefix { get; }

        public virtual string ThemeType => $"BCN.{Prefix}Theme.Theme, BCN.Client.Oqtane";
        public virtual string ContainerType => $"BCN.{Prefix}Theme.Container, BCN.Client.Oqtane";

        protected abstract IEnumerable<PageTemplate> Pages();

        public List<PageTemplate> CreateSite(Site site)
        {
            return new List<PageTemplate>(Pages());
        }

        protected string Module(string name) => $"BCN.{Prefix}{name}Module, BCN.Client.Oqtane";

        protected PageTemplate Home(string moduleName, string moduleTitle = null)
        {
            var page = Page("home", "Home", moduleName, order: 1, navigation: true, moduleTitle: moduleTitle);
            page.Icon = "oi oi-home";

            return page;
        }

        protected PageTemplate Page(string path, string name, string moduleName, int order,
                                    bool navigation = false, string moduleTitle = null, bool seedOnly = false)
        {
            var page = NewPage(path, name, order, navigation, seedOnly);

            page.PageTemplateModules.Add(new PageTemplateModule
            {
                ModuleDefinitionName = Module(moduleName),
                Title = moduleTitle ?? name,
                Pane = PaneNames.Default,
                ContainerType = ContainerType,
                PermissionList = PublicPermissions()
            });

            return page;
        }

        protected PageTemplate Removed(string path, string name)
        {
            var page = NewPage(path, name, order: 99, navigation: false, seedOnly: false);
            page.IsDeleted = true;
            
            return page;
        }

        private PageTemplate NewPage(string path, string name, int order, bool navigation, bool seedOnly)
        {
            return new PageTemplate
            {
                AliasName = "*",
                Version = "*",
                Update = !seedOnly,
                Path = path.ToLower(),
                Name = name,
                Order = order,
                IsNavigation = navigation,
                ThemeType = ThemeType,
                DefaultContainerType = ContainerType,
                PermissionList = PublicPermissions(),
                PageTemplateModules = new List<PageTemplateModule>()
            };
        }

        private static List<Permission> PublicPermissions() => new List<Permission>
        {
            new Permission(PermissionNames.View, RoleNames.Everyone, true),
            new Permission(PermissionNames.View, RoleNames.Admin, true),
            new Permission(PermissionNames.Edit, RoleNames.Admin, true)
        };
    }
}
