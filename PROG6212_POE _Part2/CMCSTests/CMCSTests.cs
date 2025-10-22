using Xunit;
using POE_Part2_PROG6212.Models;

namespace CMCSTests
{
    public class ClaimTests
    {
        [Fact]
        public void CalculatedTotalAmount()
        {
            // arrange phase
            var claim = new Claim();

            claim.Hours = 20;
            claim.Rate = 670;

            // act phase
            var getResult = claim.CalculateTotalAmount();

            // assert phase
            Assert.Equal(13400, getResult);
        }

        [Fact]
        public void AdditionalNotes_Simulation()
        {
            var claim = new Claim();
            claim.Notes = "Additional Notes submitted.";

            var notes = claim.Notes;
            Assert.Equal("Additional Notes submitted.", notes);
        }

        [Fact]
        public void FileProperties_IsStoredCorrectly()
        {
            var claim = new Claim();

            claim.FileName = "invoice.pdf";
            claim.FilePath = "/uploads/invoice.pdf";

            Assert.Equal("invoice.pdf", claim.FileName);
            Assert.Equal("/uploads/invoice.pdf", claim.FilePath);
        }

        [Fact]
        public void ClaimStatus_ChangesWhenApprovedOrRejected()
        {
            // Arrange
            var claim = new Claim();
            string approver = "Manager";

            // Act
            claim.Approve(approver);

            // Assert
            Assert.Equal("Approved", claim.Status);
            Assert.Equal("Manager", claim.ApprovedBy);

            // Act again
            claim.Reject(approver);

            // Assert again
            Assert.Equal("Rejected", claim.Status);
            Assert.Equal("Manager", claim.ApprovedBy);
        }

        [Fact]
        public void CalculateTotalAmount_ReturnsCorrectValue()
        {
            // Arrange
            var claim = new Claim { Hours = 5, Rate = 200 };

            // Act
            double result = claim.CalculateTotalAmount();

            // Assert
            Assert.Equal(1000, result);
            Assert.Equal(1000, claim.TotalAmount);
        }



    }
}
