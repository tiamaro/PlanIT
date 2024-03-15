using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Services.Interfaces;

public interface IInviteService : IService<InviteDTO>
{
    Task<InviteDTO?> CreateInviteAsync(InviteDTO newInviteDTO);
}