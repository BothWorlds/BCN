using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;

namespace BCN.Server.SiteMigrations
{
    [SiteMigration("*", "01.00.01")]
    public class AddWebSiteMigration : ISiteMigration
    {
        private readonly IServiceProvider _serviceProvider;

        public AddWebSiteMigration(IServiceProvider serviceProvider)
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

                const string newSiteName = "Black Cat Networks";
                //const string newAliasPath = "localhost:5001";

                site.Name = newSiteName;
                site.DefaultThemeType = "BCN.WebTheme.Theme, BCN.Client.Oqtane";
                site.DefaultContainerType = "BCN.WebTheme.Container, BCN.Client.Oqtane";
                site.AdminContainerType = "Oqtane.Themes.AdminContainer, Oqtane.Client";
                site.Runtime = "Server";
                site.RenderMode = "Interactive";
                site.VisitorTracking = true;
                site.CaptureBrokenUrls = true;
                site.Prerender = true;
                siteRepo.UpdateSite(site);

                // 2. Create the Alias record
                // Not done for initial site
                //var newAlias = new Alias
                //{
                //    Name = newAliasPath,
                //    SiteId = newSite.SiteId,
                //    TenantId = newSite.TenantId,
                //    IsDefault = true
                //};
                //aliasRepo.AddAlias(newAlias);
            }
        }

        public void Down(Site site, Alias alias)
        {
        }
    }
}