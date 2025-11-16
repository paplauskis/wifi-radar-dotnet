using API.Domain.Models;

namespace Tests.Helpers;

internal class WifiNetworkPropertyHelper : PropertyHelper
{
    public static IEnumerable<object[]> GetNonNullableStringProperties()
        => GetProperties<string, WifiNetwork>(false, ["", "   ", null], false);
    
    public static IEnumerable<object[]> GetNullableStringProperties()
        => GetProperties<string, WifiNetwork>(true, ["", "   "], false);
    
    public static IEnumerable<object[]> GetNonNullableIntProperties()
        => GetProperties<int, WifiNetwork>(false, [0, -1, -10, -67, int.MinValue, int.MaxValue], true);
    
    public static IEnumerable<object[]> GetNullableIntProperties()
        => GetProperties<int?, WifiNetwork>(true, [0, -1, -10, -67, int.MinValue, int.MaxValue], true);

}