using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class RegulatoryCircular
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string? Summary { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Body { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EffectiveDate { get; set; }

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [MaxLength(260)]
        public string? AttachmentFileName { get; set; }

        [MaxLength(260)]
        public string? AttachmentPath { get; set; }
    }
}
