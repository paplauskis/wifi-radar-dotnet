using System.Net;

namespace API.Domain.Dto;

public class ApiResponseDto
{
    public HttpStatusCode StatusCode { get; set; }
    public required string Message { get; set; }
    public long ResponseTimeMs { get; set; }
}