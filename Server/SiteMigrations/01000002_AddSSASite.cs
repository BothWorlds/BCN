using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;

namespace BWS.CV.Server.SiteMigrations
{
    [SiteMigration("*", "01.00.02")]
    public class AddSSASiteMigration : ISiteMigration
    {
        private readonly IServiceProvider _serviceProvider;

        public AddSSASiteMigration(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Up(Site site, Alias alias)
        {
            // Use the Default/Host site as the trigger to run this once
            if (alias.IsDefault && site.SiteId == 1)
            {
                var siteRepo = _serviceProvider.GetRequiredService<ISiteRepository>();
                var aliasRepo = _serviceProvider.GetRequiredService<IAliasRepository>();

                const string newSiteName = "Severance Sign Art";
                const string newAliasPath = "localhost:5002";

                if (!siteRepo.GetSites().Any(s => s.Name == newSiteName))
                {
                    // 1. Create the Site record
                    var newSite = new Site
                    {
                        Name = newSiteName,
                        TenantId = alias.TenantId, // Multi-tenant: change if needed
                        DefaultThemeType = "BCN.SSATheme.Theme, BCN.Client.Oqtane",
                        DefaultContainerType = "BCN.SSATheme.Container, BCN.Client.Oqtane",
                        AdminContainerType = "Oqtane.Themes.AdminContainer, Oqtane.Client",
                        Runtime = "Server",
                        RenderMode = "Interactive",
                        VisitorTracking = true,
                        CaptureBrokenUrls = true,
                        Prerender = true
                    };
                    newSite = siteRepo.AddSite(newSite);

                    // 2. Create the Alias record
                    var newAlias = new Alias
                    {
                        Name = newAliasPath,
                        SiteId = newSite.SiteId,
                        TenantId = newSite.TenantId,
                        IsDefault = true
                    };
                    aliasRepo.AddAlias(newAlias);
                }
            }
        }

        public void Down(Site site, Alias alias)
        {
        }
    }
}