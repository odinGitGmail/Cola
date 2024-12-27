using System.Reflection;
using Cola.Utils.Models.ExtensionModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cola.Utils.Helper;

public class DynamicCompilationHelper
{
    /// <summary>
    /// LoadFileCreateDynamicCompilation
    /// </summary>
    /// <param name="compilationFilePath">compilationFilePath</param>
    /// <param name="compilationTypeName">compilationTypeName</param>
    /// <returns>DynamicCompilation</returns>
    /// <exception cref="Exception">编译出错</exception>
    public DynamicCompilation? LoadFileCreateDynamicCompilation(string compilationFilePath, string compilationTypeName)
    {
        string transfTxt = File.ReadAllText(compilationFilePath);
        // 1. 创建语法树
        var syntaxTree = CSharpSyntaxTree.ParseText(transfTxt);

        // 2. 获取当前运行时的所有程序集引用
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location)) // 排除动态程序集
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        // 显式添加核心程序集（如果自动获取的引用不够）
        var coreAssemblyLocation = typeof(object).Assembly.Location; // System.Private.CoreLib.dll
        if (references.All(r => r.Display != coreAssemblyLocation))
        {
            references.Add(MetadataReference.CreateFromFile(coreAssemblyLocation));
        }

        // 3. 创建编译器
        var compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // 4. 编译代码
        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            // 输出编译错误信息
            foreach (var diagnostic in result.Diagnostics)
            {
                Console.WriteLine(diagnostic.ToString());
            }

            throw new Exception($"{compilationTypeName}编译出错");
        }

        ms.Seek(0, SeekOrigin.Begin);

        var assembly = Assembly.Load(ms.ToArray());
        var type = assembly.GetType(compilationTypeName);
        if (type == null) return null;
        var instance = Activator.CreateInstance(type);
        return new DynamicCompilation()
        {
            DynamicType = type,
            Instance = instance,
        };
    }

    /// <summary>
    /// HasPublicMethod
    /// </summary>
    /// <param name="dynamicCompilation">dynamicCompilation</param>
    /// <param name="methodName">methodName</param>
    /// <param name="isStatic">isStaticMethod default false</param>
    /// <returns>has is true,otherwise false</returns>
    /// <exception cref="Exception">dynamicCompilation.DynamicType or dynamicCompilation.Instance is null</exception>
    public bool HasPublicMethod(DynamicCompilation dynamicCompilation, string methodName, bool isStatic=false)
    {
        if (dynamicCompilation.DynamicType == null)
        {
            throw new Exception("dynamicCompilation.DynamicType is null");
        }

        var method = isStatic
            ? dynamicCompilation.DynamicType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
            : dynamicCompilation.DynamicType.GetMethod(methodName);
        return method != null;
    }

    /// <summary>
    /// HasPublicProperty
    /// </summary>
    /// <param name="dynamicCompilation">dynamicCompilation</param>
    /// <param name="propertyName">propertyName</param>
    /// <param name="isStatic">isStaticMethod default false</param>
    /// <returns>has is true,otherwise false</returns>
    /// <exception cref="Exception">dynamicCompilation.DynamicType or dynamicCompilation.Instance is null</exception>
    public bool HasPublicProperty(DynamicCompilation dynamicCompilation, string propertyName, bool isStatic=false)
    {
        if (dynamicCompilation.DynamicType == null)
        {
            throw new Exception("dynamicCompilation.DynamicType is null");
        }

        var property = isStatic
            ? dynamicCompilation.DynamicType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static)
            : dynamicCompilation.DynamicType.GetProperty(propertyName);
        return property != null;
    }

    /// <summary>
    /// GetPropertyValue
    /// </summary>
    /// <param name="dynamicCompilation">dynamicCompilation</param>
    /// <param name="propertyName">propertyName</param>
    /// <param name="isStatic">isStaticMethod default false</param>
    /// <typeparam name="T">property value type</typeparam>
    /// <returns>property value</returns>
    /// <exception cref="Exception">dynamicCompilation.DynamicType or dynamicCompilation.Instance is null</exception>
    public T? GetPropertyValue<T>(DynamicCompilation dynamicCompilation, string propertyName, bool isStatic=false)
    {
        if (dynamicCompilation.DynamicType == null || dynamicCompilation.Instance == null)
        {
            throw new Exception("DynamicType is null or Instance is null");
        }

        var property = isStatic
            ? dynamicCompilation.DynamicType!.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static)
            : dynamicCompilation.DynamicType!.GetProperty(propertyName);
        if (property == null)
        {
            return default(T);
        }

        var obj = isStatic
            ?property.GetValue(null)
            :property.GetValue(dynamicCompilation.Instance!);
        if (obj == null)
        {
            return default(T);
        }

        return (T)Convert.ChangeType(obj, typeof(T));
    }

    /// <summary>
    /// InvokeMethod
    /// </summary>
    /// <param name="dynamicCompilation">dynamicCompilation</param>
    /// <param name="methodName">methodName</param>
    /// <param name="isStatic">isStaticMethod default false</param>
    /// <param name="args">method args</param>
    /// <typeparam name="T">method return object Type</typeparam>
    /// <returns>Invoke Method return result</returns>
    /// <exception cref="Exception">dynamicCompilation.DynamicType or dynamicCompilation.Instance is null</exception>
    public T? InvokeMethod<T>(
        DynamicCompilation dynamicCompilation,
        string methodName, 
        bool isStatic=false,
        params object[] args)
    {
        if (dynamicCompilation.DynamicType == null || dynamicCompilation.Instance == null)
        {
            throw new Exception("DynamicType is null or Instance is null");
        }

        var method = isStatic
            ? dynamicCompilation.DynamicType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
            : dynamicCompilation.DynamicType.GetMethod(methodName);
        if (method != null)
        {
            var methodParams = method.GetParameters();
            List<object> methodNeedParamValues = [];
            object? output = null;
            if (methodParams.Length != 0)
            {
                var parameters = method.GetParameters();
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(int))
                    {
                        methodNeedParamValues.Add(Convert.ToInt32(args[i]));
                    }
                    else if (parameters[i].ParameterType == typeof(decimal))
                    {
                        methodNeedParamValues.Add(Convert.ToDecimal(args[i]));
                    }
                    else
                    {
                        methodNeedParamValues.Add(args[i]);
                    }
                }

                output = isStatic
                    ?method.Invoke(null, methodNeedParamValues.ToArray())
                    :method.Invoke(dynamicCompilation.Instance, methodNeedParamValues.ToArray());
            }
            else
            {
                output = isStatic
                    ?method.Invoke(null, null)
                    :method.Invoke(dynamicCompilation.Instance, null);
            }

            return output != null ? (T)Convert.ChangeType(output, typeof(T)) : default(T);
        }

        return default(T);
    }

    /// <summary>
    /// InvokeMethod
    /// </summary>
    /// <param name="dynamicCompilation">dynamicCompilation</param>
    /// <param name="methodName">methodName</param>
    /// <param name="objectParams">method object params</param>
    /// <param name="isStatic">isStaticMethod default false</param>
    /// <typeparam name="T">method return object Type</typeparam>
    /// <returns>Invoke Method return result</returns>
    /// <exception cref="Exception">dynamicCompilation.DynamicType or dynamicCompilation.Instance is null</exception>
    public T? InvokeMethod<T>(
        DynamicCompilation dynamicCompilation,
        string methodName, 
        Dictionary<string, object>? objectParams,
        bool isStatic=false)
    {
        if (dynamicCompilation.DynamicType == null || dynamicCompilation.Instance == null)
        {
            throw new Exception("DynamicType is null or Instance is null");
        }
        
        var method = isStatic
            ? dynamicCompilation.DynamicType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
            : dynamicCompilation.DynamicType.GetMethod(methodName);
        
        if (method != null)
        {
            object? output = null;
            if (objectParams == null || objectParams.Count == 0)
            {
                output = isStatic
                    ?method.Invoke(null, null)
                    :method.Invoke(dynamicCompilation.Instance, null);
            }
            else
            {
                output = isStatic
                    ?method.Invoke(null, (from mParam in method.GetParameters() 
                            where objectParams.ContainsKey(mParam.Name) 
                            select objectParams[mParam.Name])
                        .ToArray())
                    :method.Invoke(
                        dynamicCompilation.Instance, 
                        (from mParam in method.GetParameters() 
                            where objectParams.ContainsKey(mParam.Name) 
                            select objectParams[mParam.Name])
                        .ToArray());
            }

            return output != null ? (T)Convert.ChangeType(output, typeof(T)) : default(T);
        }

        return default(T);
    }
}