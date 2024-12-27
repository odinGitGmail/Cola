using System.Linq.Expressions;
using Cola.Utils.Models.ExtensionModels;

namespace Cola.Utils.Extensions;

/// <summary>
/// ExpressionExtension
/// </summary>
public static class ExpressionExtension
{
    /// <summary>
    /// 以And合并单个表达式.
    /// 此处采用AndAlso实现“最短路径”，避免掉额外且不需要的比较运算式.
    /// </summary>
    /// <param name="leftExpress">左侧表达式.</param>
    /// <param name="rightExpress">右侧表达式.</param>
    /// <typeparam name="T">表达式对象类型.</typeparam>
    /// <returns>and合并后的表达式.</returns>
    public static Expression<Func<T, bool>> MergeAnd<T>(
        this Expression<Func<T, bool>> leftExpress,
        Expression<Func<T, bool>> rightExpress)
    {
        // 声明传递参数（也就是表达式树里面的参数别名s）
        ParameterExpression parameter = Expression.Parameter(typeof(T), "s");

        // 统一管理参数，保证参数一致，否则会报错
        var visitor = new PredicateExpressionVisitor(parameter);

        // 表达式树内容
        Expression left = visitor.Visit(leftExpress.Body);
        Expression right = visitor.Visit(rightExpress.Body);

        // 合并表达式
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }

    /// <summary>
    /// 以Or合并单个表达式.
    /// 此处采用OrElse实现“最短路径”，避免掉额外且不需要的比较运算式.
    /// </summary>
    /// <param name="leftExpress">左侧表达式.</param>
    /// <param name="rightExpress">右侧表达式.</param>
    /// <typeparam name="T">表达式对象类型.</typeparam>
    /// <returns>and合并后的表达式.</returns>
    public static Expression<Func<T, bool>> MergeOr<T>(
        this Expression<Func<T, bool>> leftExpress,
        Expression<Func<T, bool>> rightExpress)
    {
        // 声明传递参数（也就是表达式树里面的参数别名s）
        ParameterExpression parameter = Expression.Parameter(typeof(T), "s");

        // 统一管理参数，保证参数一致，否则会报错
        var visitor = new PredicateExpressionVisitor(parameter);

        // 表达式树内容
        Expression left = visitor.Visit(leftExpress.Body);
        Expression right = visitor.Visit(rightExpress.Body);

        // 合并表达式
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
    }
}