﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users
{
    [Table("ManagementCompanySuperuser")]
    public partial class ManagementCompanySuperuser
    {
        [Key]
        public int UserId { get; set; }
        public int ManagingAgencyId { get; set; }
    }
}