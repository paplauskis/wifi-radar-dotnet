namespace API.Domain.Dto;

public class ApiStatusDto
{
    public bool IsStable { get; set; }
    public IReadOnlyCollection<ApiResponseDto>? Responses { get; set; }
}