using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.WebContactUsModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "WebContactUsModule",
            Description = "WebContact Us Module",
            PackageName = "BCN",
            Resources = new List<Resource>
            {
                new Stylesheet("~/Module.css")
            }
        };
    }
}