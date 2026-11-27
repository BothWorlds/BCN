using System.Collections.Generic;
using Oqtane.Infrastructure;
using Oqtane.Models;

namespace BCN.Server.SiteTemplates
{
    public class BDTSiteTemplate : BCNSiteTemplateBase, ISiteTemplate
    {
        public override string SiteName => "Black Dog Tally";
        public override string Prefix => "BDT";

        protected override IEnumerable<PageTemplate> Pages()
        {
            yield return Home("Home");
            yield return Page("contact-us", "Contact Us", "ContactUs", order: 9, navigation: true);
        }
    }
}
