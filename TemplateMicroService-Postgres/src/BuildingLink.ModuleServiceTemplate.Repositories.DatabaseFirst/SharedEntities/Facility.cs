// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.SharedEntities
{
    [Table("tblFacilities")]
    [Index(nameof(Id), Name = "_dta_index_tblFacilities_9_1125331519__K1")]
    [Index(nameof(Id), Name = "_dta_index_tblFacilities_9_734781825__K1_154")]
    public partial class Facility
    {
        [Key]
        [Column("FacID")]
        public int Id { get; set; }

        [Column("FacName")]
        [Required]
        public string Name { get; set; }
        [Column("MAID")]
        public int? ManagingAgentsId { get; set; }
        [Column("CRMBuildingTypeId")]
        public int? CRMBuildingTypeId { get; set; }
        [InverseProperty(nameof(Unit.Facility))]

        public virtual ICollection<Unit> Units { get; set; }
    }
}