namespace API.Domain.Dto.IpapiDto;

public class IpapiGetResponseDto
{
    public string? Ip { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? LatLon { get; set; } //coordinates
}