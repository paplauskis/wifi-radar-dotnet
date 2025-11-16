namespace API.Domain.Dto;

public class WifiReviewDto
{
    public string? WifiId { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public int? BuildingNumber { get; set; }
    public string? UserId { get; set; }
    public int? Rating { get; set; }
    public string? Text { get; set; }
}