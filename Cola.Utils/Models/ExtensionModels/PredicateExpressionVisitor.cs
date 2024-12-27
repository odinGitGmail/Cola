using System.Linq.Expressions;

namespace Cola.Utils.Models.ExtensionModels;

public class PredicateExpressionVisitor : ExpressionVisitor
{
    public ParameterExpression _parameter { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateExpressionVisitor"/> class.
    /// </summary>
    /// <param name="parameter"></param>
    public PredicateExpressionVisitor(ParameterExpression parameter)
    {
        _parameter = parameter;
    }

    /// <inheritdoc/>
    protected override Expression VisitParameter(ParameterExpression p)
    {
        return _parameter;
    }

    /// <inheritdoc/>
    public override Expression Visit(Expression expression)
    {
        // Visit会根据VisitParameter()方法返回的Expression进行相关变量替换
        return base.Visit(expression);
    }
}