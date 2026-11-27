using System.Collections.Generic;
using Oqtane.Infrastructure;
using Oqtane.Models;

namespace BCN.Server.SiteTemplates
{
    public class WebSiteTemplate : BCNSiteTemplateBase, ISiteTemplate
    {
        public override string SiteName => "Black Cat Networks";
        public override string Prefix => "Web";

        protected override IEnumerable<PageTemplate> Pages()
        {
            yield return Home("Home");
            yield return Page("managed-it",              "Managed IT Services",     "ManagedIt",             order: 2);
            yield return Page("cybersecurity",           "Cybersecurity",           "Cybersecurity",         order: 3);
            yield return Page("cloud-services",          "Cloud Services",          "CloudServices",         order: 4);
            yield return Page("network-support",         "Network Infrastructure",  "NetworkInfrastructure", order: 5);
            yield return Page("backup-recovery",         "Backup Recovery",         "BackupRecovery",        order: 6);
            yield return Page("compliance-risk",         "Compliance Risk",         "ComplianceRisk",        order: 7);
            yield return Page("strategic-it-consulting", "Strategic IT Consulting", "StrategicItConsulting", order: 8);
            yield return Page("contact-us",              "Contact Us",              "ContactUs",             order: 9, navigation: true);
        }
    }
}
