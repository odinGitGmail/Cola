using System.Data;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Cola.Utils.Extensions;
/// <summary>
/// ConvertExtensions
/// </summary>
public static class ConvertExtensions
{
    /// <summary>
    /// ConvertDateTableToDataRowList
    /// </summary>
    /// <param name="dt">DataTable</param>
    /// <returns>DataRow List</returns>
    public static List<DataRow> ConvertDateTableToDataRowList(this DataTable dt)
    {
        return dt.AsEnumerable().ToList();
    }
    
    /// <summary>
    /// ConvertDateTableToJsonString
    /// </summary>
    /// <param name="dt">DataTable</param>
    /// <returns>string</returns>
    public static string ConvertDateTableToJsonString(this DataTable dt)
    {
        return JsonConvert.SerializeObject(dt, Formatting.Indented);
    }
    
    /// <summary>
    /// ConvertDataRowListToDataTable
    /// </summary>
    /// <param name="drLst">DataRow List</param>
    /// <returns>DataTable</returns>
    public static DataTable ConvertDataRowListToDataTable(this List<DataRow> drLst)
    {
        return drLst.CopyToDataTable();
    }
    
    /// <summary>
    /// ConvertListToJsonString
    /// </summary>
    /// <param name="lst">List</param>
    /// <typeparam name="T">T</typeparam>
    /// <returns>string</returns>
    public static string ConvertListToJsonString<T>(this List<T> lst)
    {
        return JsonConvert.SerializeObject(lst, Formatting.Indented);
    }
    
    /// <summary>
    /// ConvertListToXmlString
    /// </summary>
    /// <param name="lst">List</param>
    /// <typeparam name="T">T</typeparam>
    /// <returns>string</returns>
    public static string ConvertListToXmlString<T>(this List<T> lst)
    {
        using StringWriter stringWriter = new StringWriter();
        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
        serializer.Serialize(stringWriter, lst);
        string xml = stringWriter.ToString();
        return xml;
    }
    
    /// <summary>
    /// ConvertDictionaryToList
    /// </summary>
    /// <param name="dictionary">dictionary</param>
    /// <typeparam name="TKt">Key Type</typeparam>
    /// <typeparam name="TVt">Value Type</typeparam>
    /// <returns>KeyValuePair List</returns>
    public static List<KeyValuePair<TKt, TVt>> ConvertDictionaryToList<TKt,TVt>(this Dictionary<TKt, TVt> dictionary) where TKt : notnull
    {
        return dictionary.ToList();
    }
    
    /// <summary>
    /// ConvertDictionaryToJsonString
    /// </summary>
    /// <param name="dictionary">dictionary</param>
    /// <typeparam name="TK">Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    /// <returns>string</returns>
    public static string ConvertDictionaryToJsonString<TK,TV>(this Dictionary<TK, TV> dictionary) where TK : notnull
    {
        return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
    }
    
    /// <summary>
    /// ConvertJsonStringToDataTable
    /// </summary>
    /// <param name="jsonString">jsonString</param>
    /// <returns>DataTable</returns>
    public static DataTable? ConvertJsonStringToDataTable(this string jsonString)
    {
        return JsonConvert.DeserializeObject<DataTable>(jsonString);
    }
    
    /// <summary>
    /// ConvertXmlStringToList
    /// </summary>
    /// <param name="xmlString">xmlString</param>
    /// <typeparam name="T">T</typeparam>
    /// <returns>List</returns>
    public static List<T>? ConvertXmlStringToList<T>(this string xmlString)
    {
        using (StringReader stringReader = new StringReader(xmlString))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            return (List<T>)serializer.Deserialize(stringReader)!;
        }
    }
    
    /// <summary>
    /// ConvertXmlStringToDictionary
    /// </summary>
    /// <param name="xmlString">xmlString</param>
    /// <typeparam name="TK">Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    /// <returns>Dictionary</returns>
    public static Dictionary<TK,TV>? ConvertXmlStringToDictionary<TK,TV>(this string xmlString) where TK : notnull
    {
        Dictionary<string, object> dictionary;
        using StringReader stringReader = new StringReader(xmlString);
        XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<string, object>));
        return (Dictionary<TK, TV>)serializer.Deserialize(stringReader)!;
    }
    
    /// <summary>
    /// ConvertXmlStringToDataTable
    /// </summary>
    /// <param name="xmlString">xmlString</param>
    /// <returns>DataTable</returns>
    public static DataTable ConvertXmlStringToDataTable(this string xmlString)
    {
        DataSet dataSet = new DataSet();
        dataSet.ReadXml(new StringReader(xmlString));
        return dataSet.Tables[0];
    }
    
    /// <summary>
    /// ConvertXmlStringToJsonString
    /// </summary>
    /// <param name="xmlString">xmlString</param>
    /// <returns>string</returns>
    public static string ConvertXmlStringToJsonString(this string xmlString)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlString);
        return  JsonConvert.SerializeXmlNode(xmlDoc);
    }
}