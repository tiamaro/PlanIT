using PlanIT.API.Models.Entities;

namespace PlanIT.API.Repositories.Interfaces;

public interface IInviteRepository : IRepository<Invite>
{
    Task<ICollection<Invite>> GetInvitesByEventIdAsync(int eventId, int pageNr, int pageSize);
}
