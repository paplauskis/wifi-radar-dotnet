namespace API.Helpers;

public static class IpapiApi
{
    private static readonly string ApiUrl = "https://ipapi.co/";

    //format can be json, xml, csv and yaml
    public static string GetRequestUrlFormat(string ip, string format = "json")
    {
        return $"{ApiUrl}{ip}/{format}/";
    }

    //gets specified property from API (e.g. city, country, lat/lon etc)
    //more info here: https://ipapi.co/api/?csharp#specific-location-field
    public static string GetRequestUrlProperty(string ip, string property)
    {
        return $"{ApiUrl}{ip}/{property}/";
    }
}