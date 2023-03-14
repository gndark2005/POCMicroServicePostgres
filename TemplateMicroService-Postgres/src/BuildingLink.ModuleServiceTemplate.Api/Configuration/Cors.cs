using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Configuration
{
    public class Cors
    {
        public const string CorsConfigurationKey = "Cors";

        public const string CorsPolicyName = "BuildingLinkCorsPolicy";

        public string CorsPolice { get; set; }

        public ICollection<string> Origins { get; set; }

        public ICollection<string> Methods { get; set; }

        public ICollection<string> Headers { get; set; }
    }
}
