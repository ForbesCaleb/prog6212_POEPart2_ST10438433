using System;
using System.ComponentModel.DataAnnotations;

namespace POE_Part2_PROG6212.Models
{
    public class Claim
    {
        [Key]
        public int Id { get; set; }

        // Lecturer and claim metadata
        public string LecturerName { get; set; } = string.Empty;
        public DateTime DateWorked { get; set; } = DateTime.Now;
        public string Activity { get; set; } = string.Empty;

        // Core claim details
        public double Hours { get; set; }
        public double Rate { get; set; }
        public double TotalAmount { get; set; }

        // Claim workflow
        public string Status { get; set; } = "Pending"; // Default state
        public DateTime DateSubmitted { get; set; } = DateTime.Now;
        public string ApprovedBy { get; set; } = string.Empty;

        // Supporting documents
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        // Notes and comments
        public string Notes { get; set; } = string.Empty;

        // -------------------------
        // BUSINESS LOGIC METHODS
        // -------------------------

        /// <summary>
        /// Calculates total amount based on hours and rate.
        /// </summary>
        public double CalculateTotalAmount()
        {
            if (Hours < 0 || Rate < 0)
                throw new ArgumentException("Hours and Rate must be positive values.");

            TotalAmount = Hours * Rate;
            return TotalAmount;
        }

        /// <summary>
        /// Approves this claim.
        /// </summary>
        /// <param name="approver">Name of the person approving the claim.</param>
        public void Approve(string approver)
        {
            Status = "Approved";
            ApprovedBy = approver;
        }

        /// <summary>
        /// Rejects this claim.
        /// </summary>
        /// <param name="approver">Name of the person rejecting the claim.</param>
        public void Reject(string approver)
        {
            Status = "Rejected";
            ApprovedBy = approver;
        }

        /// <summary>
        /// Attaches a supporting document to this claim.
        /// </summary>
        /// <param name="fileName">The uploaded file’s name.</param>
        /// <param name="filePath">The server path where the file is stored.</param>
        public void AttachDocument(string fileName, string filePath)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Both file name and file path are required.");

            FileName = fileName;
            FilePath = filePath;
        }

        /// <summary>
        /// Adds additional notes to the claim.
        /// </summary>
        /// <param name="note">The lecturer’s note or comment.</param>
        public void AddNotes(string note)
        {
            if (string.IsNullOrWhiteSpace(note))
                throw new ArgumentException("Note cannot be empty.");

            Notes = note;
        }

        /// <summary>
        /// Returns formatted string for debugging or logs.
        /// </summary>
        public override string ToString()
        {
            return $"{LecturerName} | {Activity} | {Hours}h x R{Rate} = R{TotalAmount} | Status: {Status}";
        }
    }
}



