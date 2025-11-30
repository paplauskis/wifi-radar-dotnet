using API.Domain.Dto;

namespace API.Services.Interfaces.Status;

public interface IOverpassApiStatusService
{
    Task<ApiStatusDto> CheckStatus();
}