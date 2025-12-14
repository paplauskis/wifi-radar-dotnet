namespace API.Helpers.Extensions;

public static class StringExtensions
{
    public static string GetFileType(this string fileType)
    {
        return fileType.Split('/').Last();
    }
}