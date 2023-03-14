using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Repositories
{
    /// <summary>
    /// Possible book status.
    /// </summary>
    public enum BookStatus
    {
        /// <summary>
        /// It is available to lend
        /// </summary>
        Available = 0,

        /// <summary>
        /// It is being sent to the library
        /// </summary>
        AvailableSoon,

        /// <summary>
        /// Currently another client is using it
        /// </summary>
        InUse
    }
}
