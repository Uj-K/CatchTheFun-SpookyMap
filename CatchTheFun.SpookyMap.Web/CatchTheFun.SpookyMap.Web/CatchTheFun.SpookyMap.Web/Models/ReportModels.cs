using System.ComponentModel.DataAnnotations;
using CatchTheFun.SpookyMap.Web.Models;

namespace CatchTheFun.SpookyMap.Web.Models
{
    public class ReportCreateViewModel
    {
        public int? EventLocationId { get; set; }

        [Display(Name = "Your name (optional)")]
        [StringLength(100)]
        public string? ReporterName { get; set; }

        [EmailAddress]
        [Display(Name = "Email (optional)")]
        public string? ReporterEmail { get; set; }

        [Required]
        [Display(Name = "What seems inaccurate?")]
        [StringLength(2000, MinimumLength = 5)]
        public string Message { get; set; } = string.Empty;

        // For display only
        public EventLocation? SelectedLocation { get; set; }
    }

    public class StoredReport
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public int? EventLocationId { get; set; }
        public string? ReporterName { get; set; }
        public string? ReporterEmail { get; set; }
        public string Message { get; set; }
        public string? SnapshotName { get; set; }
        public string? SnapshotAddress { get; set; }
    }
}
