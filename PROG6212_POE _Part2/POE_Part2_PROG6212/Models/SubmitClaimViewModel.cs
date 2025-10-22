using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CMCS.Web.Models
{
    // ===== ViewModel for form submission =====
    public class SubmitClaimViewModel
    {
        [Required, DataType(DataType.Date)]
        public DateTime DateWorked { get; set; } = DateTime.Today;

        [Required, Range(0.25, 24)]
        public decimal HoursWorked { get; set; } = 1m;

        [Required, StringLength(60)]
        public string Activity { get; set; } = "Lecture";

        [Required, Range(0, 10000)]
        public decimal HourlyRate { get; set; } = 350m; // demo default

        public decimal TotalAmount => Math.Round(HourlyRate * HoursWorked, 2);
    }

    // ===== Data object for saved claims =====
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
    }

    // ===== Interface for a claim store =====
    public interface IClaimStore
    {
        void Add(ClaimDto claim);
        IReadOnlyList<ClaimDto> GetByUser(string userId, int take = 10);
    }

    // ===== In-memory implementation =====
    public class InMemoryClaimStore : IClaimStore
    {
        private readonly List<ClaimDto> _claims = new();
        private readonly object _lock = new();

        public void Add(ClaimDto claim)
        {
            lock (_lock)
            {
                _claims.Add(claim);
            }
        }

        public IReadOnlyList<ClaimDto> GetByUser(string userId, int take = 10)
        {
            lock (_lock)
            {
                return _claims
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.SubmittedAt)
                    .Take(take)
                    .ToList();
            }
        }
    }
}

