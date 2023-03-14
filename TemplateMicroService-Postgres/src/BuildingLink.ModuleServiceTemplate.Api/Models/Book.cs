using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Models
{
    /// <summary>
    /// Book model is using by entity framework to create the database object model.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Book Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Book Title
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string Title { get; set; }

        /// <summary>
        /// Book Author
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Author { get; set; }

        /// <summary>
        /// Book Status
        /// </summary>
        public BookStatus Status { get; set; }

        /// <summary>
        /// Active
        /// </summary>
        public bool Active { get; set; }
    }
}
