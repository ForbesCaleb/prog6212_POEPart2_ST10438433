using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace POE_Part2_PROG6212.Models
{
    public enum ClaimStatus { Draft, Submitted, UnderReview, Approved, Rejected }

    // Form model
    public class SubmitClaimViewModel
    {
        [Required, DataType(DataType.Date)]
        public DateTime DateWorked { get; set; } = DateTime.Today;

        [Required, Range(0.25, 24)]
        public decimal HoursWorked { get; set; } = 1m;

        [Required, StringLength(60)]
        public string Activity { get; set; } = "Lecture";

        [Required, Range(0, 10000)]
        public decimal HourlyRate { get; set; } = 350m;

        public decimal TotalAmount => Math.Round(HourlyRate * HoursWorked, 2);

        public IFormFile? Document { get; set; }
        public string? Notes { get; set; }
    }

    // File metadata
    public class SupportingDocumentDto
    {
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = default!;
        public string RelativePath { get; set; } = default!;
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }

    // Saved claim (in-memory)
    public class ClaimDto
    {
        public Guid ClaimId { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = default!;
        public string LecturerName { get; set; } = default!;
        public DateTime DateWorked { get; set; }
        public decimal HoursWorked { get; set; }
        public string Activity { get; set; } = default!;
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
        public string? Notes { get; set; }
        public List<SupportingDocumentDto> Documents { get; set; } = new();
    }

    public interface IClaimStore
    {
        void Add(ClaimDto claim);
        ClaimDto? Get(Guid id);
        IReadOnlyList<ClaimDto> GetByUser(string userId, int take = 50);
        IReadOnlyList<ClaimDto> GetPending(int take = 100);
        bool Approve(Guid claimId, string approverName, string? comment = null);
        bool Reject(Guid claimId, string approverName, string? comment = null);
        bool AddDocument(Guid claimId, SupportingDocumentDto doc);
    }

    public class InMemoryClaimStore : IClaimStore
    {
        private readonly List<ClaimDto> _claims = new();
        private readonly object _lock = new();

        public void Add(ClaimDto claim) { lock (_lock) _claims.Add(claim); }
        public ClaimDto? Get(Guid id) { lock (_lock) return _claims.FirstOrDefault(c => c.ClaimId == id); }

        public IReadOnlyList<ClaimDto> GetByUser(string userId, int take = 50)
        {
            lock (_lock)
            {
                return _claims.Where(c => c.UserId == userId)
                              .OrderByDescending(c => c.SubmittedAt)
                              .Take(take).ToList();
            }
        }

        public IReadOnlyList<ClaimDto> GetPending(int take = 100)
        {
            lock (_lock)
            {
                return _claims
                    .Where(c => c.Status == ClaimStatus.Submitted || c.Status == ClaimStatus.UnderReview)
                    .OrderBy(c => c.SubmittedAt)
                    .Take(take).ToList();
            }
        }

        public bool Approve(Guid id, string approver, string? comment = null)
        {
            lock (_lock)
            {
                var c = _claims.FirstOrDefault(x => x.ClaimId == id);
                if (c == null) return false;
                c.Status = ClaimStatus.Approved;
                if (!string.IsNullOrWhiteSpace(comment)) c.Notes = comment;
                return true;
            }
        }

        public bool Reject(Guid id, string approver, string? comment = null)
        {
            lock (_lock)
            {
                var c = _claims.FirstOrDefault(x => x.ClaimId == id);
                if (c == null) return false;
                c.Status = ClaimStatus.Rejected;
                if (!string.IsNullOrWhiteSpace(comment)) c.Notes = comment;
                return true;
            }
        }

        public bool AddDocument(Guid claimId, SupportingDocumentDto doc)
        {
            lock (_lock)
            {
                var c = _claims.FirstOrDefault(x => x.ClaimId == claimId);
                if (c == null) return false;
                c.Documents.Add(doc);
                return true;
            }
        }
    }
}


