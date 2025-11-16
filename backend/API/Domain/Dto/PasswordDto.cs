namespace API.Domain.Dto;

public class PasswordDto
{
    public string? UserId { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public int? BuildingNumber { get; set; }
    public string? Password { get; set; }
}