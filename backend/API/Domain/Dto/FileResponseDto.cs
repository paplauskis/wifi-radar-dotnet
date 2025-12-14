namespace API.Domain.Dto;

public class FileResponseDto
{
    public required byte[] FileContent { get; set; }
    public required string FileName { get; set; }
    public required string HttpHeader { get; set; }
}