using API.Domain.Dto;
using API.Domain.Dto.OverpassDto;

namespace API.Services.Interfaces.Map;

public interface IMapService
{ 
    Task<List<OverpassResponseElementDto>> Search(string city, int? radius);
    Task<FileResponseDto> SearchAndSaveToFile(string city, string? fileType, int? radius);
    Task<List<OverpassResponseElementDto>> SearchWifisFromNearestCity(string ip);
    Task<CoordinateDto> GetCoordinates(string city, string street, int buildingNumber);
}