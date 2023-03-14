using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingLink.ModuleServiceTemplate.Repositories
{
    /// <summary>
    /// Book model is using by entity framework to create the database object model.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Gets or sets book Id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets book Title.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets book Author.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets book Status.
        /// </summary>
        public BookStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }
    }
}
