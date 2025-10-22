using CMCS.Web.Models;

namespace CMCS.Web;

public interface IClaimStore
{
    void Add(ClaimDto claim);
    IReadOnlyList<ClaimDto> GetByUser(string userId, int take = 10);
}

public class InMemoryClaimStore : IClaimStore
{
    // Thread-safe in-memory list
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

