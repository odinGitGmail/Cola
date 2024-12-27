namespace Cola.Utils.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// ListDictionaryFilter
    /// </summary>
    /// <param name="lstDic">List Dictionary</param>
    /// <param name="filterDic">filterDic</param>
    /// <returns>filterResult</returns>
    public static Dictionary<string, string> ListDictionaryFilter(this List<Dictionary<string, string>> lstDic,
        Dictionary<string, string> filterDic)
    {
        var filterResult = lstDic.FirstOrDefault(dict =>
            filterDic.All(kvp => dict.ContainsKey(kvp.Key) && dict[kvp.Key] == kvp.Value));

        return filterResult;
    }
}