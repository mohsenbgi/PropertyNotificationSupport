using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PropertyNotificationSupportPackage
{
    internal class MemberAccessVisitor : ExpressionVisitor
    {
        private readonly Type _declaringType;
        public readonly IList<string> PropertyNames = new List<string>();

        public MemberAccessVisitor(Type declaringType)
        {
            _declaringType = declaringType;
        }

        public override Expression Visit(Expression expr)
        {
            if (expr != null && expr.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpr = (MemberExpression) expr;
                if (memberExpr.Member.DeclaringType == _declaringType)
                {
                    PropertyNames.Add(memberExpr.Member.Name);
                }
            }
            return base.Visit(expr);
        }
    }
}