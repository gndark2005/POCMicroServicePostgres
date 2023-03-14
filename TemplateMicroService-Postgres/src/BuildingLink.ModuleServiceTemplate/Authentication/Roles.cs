using System;

namespace BuildingLink.ModuleServiceTemplate.Authentication
{
    /// <summary>
    /// Module Service roles mapping for V2 user types.
    /// </summary>
    [Flags]
    public enum Roles
    {
        /// <summary>
        /// Management and SecurityOfficer User Types
        /// </summary>
        Creator = 1,

        /// <summary>
        /// FrontDesk, Maintenance, Resident, Public Display User Types
        /// </summary>
        Reader = 2,

        /// <summary>
        /// Any
        /// </summary>
        Any = Creator | Reader,
    }
}
