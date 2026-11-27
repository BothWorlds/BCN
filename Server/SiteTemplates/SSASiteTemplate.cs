using System.Collections.Generic;
using Oqtane.Infrastructure;
using Oqtane.Models;

namespace BCN.Server.SiteTemplates
{
    public class SSASiteTemplate : BCNSiteTemplateBase, ISiteTemplate
    {
        public override string SiteName => "Severance Sign Art";
        public override string Prefix => "SSA";

        protected override IEnumerable<PageTemplate> Pages()
        {
            yield return Home("Home");
            yield return Page("custom-signs",           "Custom Signs",             "CustomSigns",      order: 2);
            yield return Page("banners-displays",       "Banners & Displays",       "Banners",          order: 3);
            yield return Page("vehicle-graphics",       "Vehicle Graphics",         "VehicleGraphic",   order: 4);
            yield return Page("lettering-decals",       "Lettering & Decals",       "LetteringDecals",  order: 5);
            yield return Page("yard-event-signs",       "Yard & Event Signs",       "YardEventSigns",   order: 6);
            yield return Page("special-projects",       "Special Projects",         "SpecialProjects",  order: 7);
            yield return Page("design-project-support", "Design & Project Support", "DesignSupport",    order: 8);
            yield return Page("contact-us",             "Contact Us",               "ContactUs",        order: 9, navigation: true);
        }
    }
}
