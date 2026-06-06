using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models;

public class ClinicalNote
{
    public int ClinicalNoteId { get; set; }

    public int VisitId { get; set; }
    public Visit Visit { get; set; } = null!;

    public string? AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    [Required, MaxLength(3000)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Diagnosis { get; set; }

    [MaxLength(1000)]
    public string? Recommendations { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
