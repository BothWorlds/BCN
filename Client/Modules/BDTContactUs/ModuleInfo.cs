using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;

namespace BCN.BDTContactUsModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "BDTContactUsModule",
            Description = "BDT Contact Us Module",
            PackageName = "BCN",
            Resources = new List<Resource>
            {
                new Stylesheet("~/Module.css")
            }
        };
    }
}