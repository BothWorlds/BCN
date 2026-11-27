using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BCN.Server.SiteTemplates;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;

namespace BCN.Server.SiteMigrations
{
    [SiteMigration("*", "*")]
    public class EnsureBCNSitesMigration : ISiteMigration
    {
        private const string PrefixSetting = "BCN:SitePrefix";

        private readonly IServiceProvider _serviceProvider;

        public EnsureBCNSitesMigration(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Up(Site site, Alias alias)
        {
            var siteRepo = _serviceProvider.GetRequiredService<ISiteRepository>();
            var settingRepo = _serviceProvider.GetRequiredService<ISettingRepository>();
            var logger = _serviceProvider.GetRequiredService<ILogManager>();

            var templates = typeof(BCNSiteTemplateBase).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BCNSiteTemplateBase)) && !t.IsAbstract)
                .Select(t => (BCNSiteTemplateBase)ActivatorUtilities.CreateInstance(_serviceProvider, t))
                .ToList();

            if (site.SiteId == siteRepo.GetSites().Min(s => s.SiteId))
            {
                var config = _serviceProvider.GetRequiredService<IConfiguration>();

                try
                {
                    EnsureHostSite(site, templates, config["BCN:HostSite"], siteRepo, settingRepo, logger);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, this, LogFunction.Other, ex, "BCN: setting up the host site failed");
                }

                EnsureSites(site, alias, templates, config, siteRepo, settingRepo, logger);
            }

            try
            {
                var template = FindTemplate(templates, GetPrefix(settingRepo, site.SiteId));

                if (template != null)
                {
                    siteRepo.CreatePages(site, template.CreateSite(site), null);
                }
            }
            catch (Exception ex)
            {
                logger.Log(site.SiteId, LogLevel.Error, this, LogFunction.Other, ex, "BCN: applying pages for {Site} failed", site.Name);
            }
        }

        private void EnsureHostSite(Site site, List<BCNSiteTemplateBase> templates, string hostPrefix,
                                    ISiteRepository siteRepo, ISettingRepository settingRepo, ILogManager logger)
        {
            var current = FindTemplate(templates, GetPrefix(settingRepo, site.SiteId));

            if (current == null && !string.IsNullOrWhiteSpace(hostPrefix))
            {
                var hostTemplate = FindTemplate(templates, hostPrefix);

                if (hostTemplate == null)
                {
                    logger.Log(LogLevel.Error, this, LogFunction.Other, "BCN: no site template with Prefix {Prefix} (BCN:HostSite)", hostPrefix);
                    
                    return;
                }

                site.DefaultThemeType = hostTemplate.ThemeType;
                site.DefaultContainerType = hostTemplate.ContainerType;

                if (site.Name == Constants.DefaultSite)
                {
                    site.Name = hostTemplate.SiteName;
                }

                siteRepo.UpdateSite(site);

                SetPrefix(settingRepo, site.SiteId, hostTemplate.Prefix);

                logger.Log(site.SiteId, LogLevel.Information, this, LogFunction.Update,
                    "BCN: site {Site} uses template {Template}", site.Name, hostTemplate.Name);
            }
            else if (current != null && site.Name == Constants.DefaultSite)
            {
                site.Name = current.SiteName;

                siteRepo.UpdateSite(site);
            }
        }

        private void EnsureSites(Site host, Alias alias, List<BCNSiteTemplateBase> templates, IConfiguration config,
                                 ISiteRepository siteRepo, ISettingRepository settingRepo, ILogManager logger)
        {
            var aliasRepo = _serviceProvider.GetRequiredService<IAliasRepository>();

            foreach (var entry in config.GetSection("BCN:Sites").GetChildren())
            {
                var prefix = entry.Key;
                var aliasName = (entry.Value ?? "").Trim().ToLower();

                try
                {
                    var template = FindTemplate(templates, prefix);

                    if (template == null)
                    {
                        logger.Log(LogLevel.Error, this, LogFunction.Other, "BCN: no site template with Prefix {Prefix}", prefix);

                        continue;
                    }

                    if (string.IsNullOrEmpty(aliasName))
                    {
                        continue;
                    }

                    var target = siteRepo.GetSites().FirstOrDefault(s =>
                        string.Equals(GetPrefix(settingRepo, s.SiteId), template.Prefix, StringComparison.OrdinalIgnoreCase));

                    if (target == null)
                    {
                        target = AdoptByAlias(host, aliasName, aliasRepo, siteRepo, settingRepo);

                        if (target != null)
                        {
                            SetPrefix(settingRepo, target.SiteId, template.Prefix);

                            logger.Log(target.SiteId, LogLevel.Information, this, LogFunction.Update,
                                "BCN: site {Site} linked to template {Template}", target.Name, template.Name);
                        }
                    }

                    if (target == null)
                    {
                        target = siteRepo.AddSite(new Site
                        {
                            Name = template.SiteName,
                            TenantId = alias.TenantId,
                            SiteTemplateType = TypeName(template),
                            DefaultThemeType = template.ThemeType,
                            DefaultContainerType = template.ContainerType,
                            AdminContainerType = Constants.DefaultAdminContainer,
                            RenderMode = host.RenderMode,
                            Runtime = host.Runtime,
                            Prerender = host.Prerender,
                            EnhancedNavigation = true,
                            VisitorTracking = true,
                            CaptureBrokenUrls = true,
                            AllowRegistration = false
                        });

                        SetPrefix(settingRepo, target.SiteId, template.Prefix);

                        logger.Log(target.SiteId, LogLevel.Information, this, LogFunction.Create, "BCN: site created {Site}", target.Name);
                    }

                    EnsureAlias(target, aliasName, aliasRepo, logger);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, this, LogFunction.Other, ex, "BCN: creating site {Prefix} failed", prefix);
                }
            }
        }

        private static Site AdoptByAlias(Site host, string aliasName, IAliasRepository aliasRepo,
                                         ISiteRepository siteRepo, ISettingRepository settingRepo)
        {
            var owner = aliasRepo.GetAliases().FirstOrDefault(a => a.Name == aliasName);

            if (owner == null || owner.SiteId == host.SiteId)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(GetPrefix(settingRepo, owner.SiteId)))
            {
                return null;
            }

            return siteRepo.GetSites().FirstOrDefault(s => s.SiteId == owner.SiteId);
        }

        private void EnsureAlias(Site target, string aliasName, IAliasRepository aliasRepo, ILogManager logger)
        {
            var existing = aliasRepo.GetAliases().FirstOrDefault(a => a.Name == aliasName);

            if (existing == null)
            {
                var hasDefault = aliasRepo.GetAliases().Any(a => a.SiteId == target.SiteId && a.IsDefault);

                aliasRepo.AddAlias(new Alias
                {
                    Name = aliasName,
                    TenantId = target.TenantId,
                    SiteId = target.SiteId,
                    IsDefault = !hasDefault
                });
            }
            else if (existing.SiteId != target.SiteId)
            {
                logger.Log(LogLevel.Error, this, LogFunction.Other,
                    "BCN: alias {Alias} already belongs to SiteId {SiteId}, not {Site}", aliasName, existing.SiteId, target.Name);
            }
        }

        private static string GetPrefix(ISettingRepository settingRepo, int siteId) =>
            settingRepo.GetSetting(EntityNames.Site, siteId, PrefixSetting)?.SettingValue;

        private static void SetPrefix(ISettingRepository settingRepo, int siteId, string prefix)
        {
            var setting = settingRepo.GetSetting(EntityNames.Site, siteId, PrefixSetting);

            if (setting == null)
            {
                settingRepo.AddSetting(new Setting
                {
                    EntityName = EntityNames.Site,
                    EntityId = siteId,
                    SettingName = PrefixSetting,
                    SettingValue = prefix,
                    IsPrivate = true
                });
            }
            else if (setting.SettingValue != prefix)
            {
                setting.SettingValue = prefix;

                settingRepo.UpdateSetting(setting);
            }
        }

        private static BCNSiteTemplateBase FindTemplate(List<BCNSiteTemplateBase> templates, string prefix) =>
            string.IsNullOrEmpty(prefix) ? null :
            templates.FirstOrDefault(t => t.Prefix.Equals(prefix, StringComparison.OrdinalIgnoreCase));

        private static string TypeName(object o) => $"{o.GetType().FullName}, {o.GetType().Assembly.GetName().Name}";

        public void Down(Site site, Alias alias)
        {
        }
    }
}
