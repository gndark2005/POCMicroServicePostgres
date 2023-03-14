using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Events
{
    public class BookStatusChanged
    {
        public Guid Id { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
    }
}
