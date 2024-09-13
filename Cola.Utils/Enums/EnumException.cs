using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Cola.Utils.Enums;

/// <summary>
/// 系统异常定义
/// </summary>
/// <param name="id">错误id， 0x000001 以x分割，前一部分 100以内表示系统框架内部错误，100后表示框架扩展类库或框架内部应用错误</param>
/// <param name="description">错误描述</param>
public class EnumException(string id, string description) : Enumeration<string>(id, description)
{
    /// <summary>
    /// 无错误，正确
    /// </summary>
    public static readonly EnumException Zero = new EnumException("0","");
    
    #region 系统定义相关 SYS 开头    id 0x 开头  SYS=0

    /// <summary>
    /// Cola框架内部错误
    /// </summary>
    public static readonly EnumException SyS000000 = new EnumException("0x000000","Cola框架内部错误");
    /// <summary>
    /// 参数不能为空
    /// </summary>
    public static readonly EnumException SyS000001 = new EnumException("0x000001","参数不能为空");
    /// <summary>
    /// 参数无法转换为枚举类型
    /// </summary>
    public static readonly EnumException SyS000002 = new EnumException("0x000002","参数无法转换为枚举类型");
    /// <summary>
    /// 参数验证失败
    /// </summary>
    public static readonly EnumException SyS000003 = new EnumException("0x000003","参数验证失败");
    /// <summary>
    /// 参数类型不正确
    /// </summary>
    public static readonly EnumException SyS000004 = new EnumException("0x000004","参数类型不正确");
    /// <summary>
    /// 参数个数超出定义范围
    /// </summary>
    public static readonly EnumException SyS000005 = new EnumException("0x000005","参数个数超出定义范围");
    /// <summary>
    /// 参数个数不正确
    /// </summary>
    public static readonly EnumException SyS000006 = new EnumException("0x000006","参数个数不正确");

    #endregion
    
    #region OdinModels.Core相关  CORE  开头    id 1x 开头    CORE=1
     
    /// <summary>
    /// 配置文件没有配置url
    /// </summary>
    public static readonly EnumException Core000001 = new EnumException("1x000001","配置文件没有配置url");
    
    #endregion
    
    #region OdinModels.OdinUtils相关  UTIL  开头    id 2x 开头    UTIL=2
     
    /// <summary>
    /// 找不到RSA key
    /// </summary>
    public static readonly EnumException Util000001 = new EnumException("2x000001","找不到 RSA key");
    /// <summary>
    /// RsaEncrypt 公钥不正确
    /// </summary>
    public static readonly EnumException Util000002 = new EnumException("2x000002","RsaEncrypt 公钥不正确");
    /// <summary>
    /// RsaDecrypt 私钥不正确
    /// </summary>
    public static readonly EnumException Util000003 = new EnumException("2x000003","RsaDecrypt 私钥不正确");
    /// <summary>
    /// key 需要 32 位长度
    /// </summary>
    public static readonly EnumException Util000004 = new EnumException("2x000004","key 需要 32 位长度");
    /// <summary>
    /// 如要使用 aes_ai 偏移量参数，必须要有加密盐值参数 key
    /// </summary>
    public static readonly EnumException Util000005 = new EnumException("2x000005","如要使用 aes_ai 偏移量参数，必须要有加密盐值参数 key");
    /// <summary>
    /// aes_ai 需要 16 位长度
    /// </summary>
    public static readonly EnumException Util000006 = new EnumException("2x000006","aes_ai 需要 16 位长度");
    
    #endregion

    #region regex相关  REG  开头    id 10x 开头    REG=10
     
    /// <summary>
    /// 邮箱格式不正确
    /// </summary>
    public static readonly EnumException Reg000001 = new EnumException("10x000001","邮箱格式不正确");
    /// <summary>
    /// 身份证格式不正确
    /// </summary>
    public static readonly EnumException Reg000002 = new EnumException("10x000002","身份证格式不正确");
    /// <summary>
    /// 移动电话号码格式不正确
    /// </summary>
    public static readonly EnumException Reg000003 = new EnumException("10x000003","移动电话号码格式不正确");
    
    #endregion
    
    #region token相关  TOK 开头    id 20x 开头    TOK=20
     
    /// <summary>
    /// token 不能为空
    /// </summary>
    public static readonly EnumException Tok000001 = new EnumException("20x000001","token 不能为空");
    /// <summary>
    /// token 过期
    /// </summary>
    public static readonly EnumException Tok000002 = new EnumException("20x000002","token 过期");
    /// <summary>
    /// 移动电话号码格式不正确
    /// </summary>
    public static readonly EnumException Tok000003 = new EnumException("20x000003","token 无效");
    
    #endregion
    
    #region snowflake雪花ID相关 SNF 开头    id 11x 开头  SN=101

    /// <summary>
    /// datacenter Id 不正确
    /// </summary>
    public static readonly EnumException Snf000001 = new EnumException("101x000001","datacenter Id 不正确");
    /// <summary>
    /// worker Id 不正确
    /// </summary>
    public static readonly EnumException Snf000002 = new EnumException("101x000001","worker Id 不正确");

    #endregion
    
    
}