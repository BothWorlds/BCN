using Oqtane.Models;
using Oqtane.Shared;
using Oqtane.Themes;
using System.Collections.Generic;

namespace BCN.MyTheme
{
    public class ThemeInfo : ITheme
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "MyTheme",
            Version = "1.0.0",
            PackageName = "BCN",
            ThemeSettingsType = "BCN.MyTheme.ThemeSettings, BCN.Client.Oqtane",
            ContainerSettingsType = "BCN.MyTheme.ContainerSettings, BCN.Client.Oqtane",
            Resources = new List<Resource>()
            {
                new Stylesheet(Constants.BootstrapStylesheetUrl, Constants.BootstrapStylesheetIntegrity, "anonymous"),
                new Resource { ResourceType = ResourceType.Script, Url = "~/Theme.js", Level = ResourceLevel.Site },
                new Stylesheet("~/bootstrap.min.css"),
                new Resource { ResourceType = ResourceType.Script, Url = Constants.BootstrapScriptUrl, Level = ResourceLevel.Site },
                new Stylesheet("https://fonts.googleapis.com"),
                new Stylesheet("https://fonts.googleapis.com/icon?family=Material+Icons"),
                new Stylesheet("https://fonts.gstatic.com"),
                new Stylesheet("https://use.typekit.net/pfm6upz.css"),
                new Stylesheet("~/GoogleFontscss2.css"),
                new Stylesheet("~/styles.css"),
                new Stylesheet("~/Theme.css"),
                new Stylesheet("_content/Radzen.Blazor/css/default-base.css"),
                new Resource { ResourceType = ResourceType.Script, Level = ResourceLevel.Site, Url = "_content/Radzen.Blazor/Radzen.Blazor.js" },
                new Resource { ResourceType = ResourceType.Script, Url = "https://code.jquery.com/jquery-3.6.4.min.js", Level = ResourceLevel.Site },
                new Resource { ResourceType = ResourceType.Script, Url = "~/script.js", Level = ResourceLevel.Site },
            }
        };
    }
}